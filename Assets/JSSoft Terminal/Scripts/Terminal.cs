using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ntreev.Library.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.UI
{
    [AddComponentMenu("UI/Terminal", 15)]
    public class Terminal : TMP_InputField, ITerminal
    {
        private static readonly Dictionary<string, IKeyBinding> bindingByKey = new Dictionary<string, IKeyBinding>();
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();

        private string promptText = string.Empty;
        private string outputText = string.Empty;
        private string prompt = string.Empty;
        private string inputText = string.Empty;
        private string completion;
        private int refStack = 0;
        private int historyIndex;
        private bool isReadOnly;
        private bool isChanged;

        private Event processingEvent = new Event();
        private ExecutedEent executedEvent = new ExecutedEent();

        public ExecutedEent onExecuted
        {
            get => executedEvent;
            set => executedEvent = value;
        }

        public OnCompletion onCompletion { get; set; }

        static Terminal()
        {
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.UpArrow, 
                            (t) => t.PrevHistory(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.DownArrow, 
                            (t) => t.NextHistory(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Return, 
                            (t) => t.Execute(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.KeypadEnter, 
                            (t) => t.Execute(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.Backspace, 
                            (t) => true, (t) => t.caretPosition <= t.outputText.Length + t.prompt.Length));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Tab, 
                            (t) => t.NextCompletion(), (t) => t.isReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.Shift, KeyCode.Tab, 
                            (t) => t.PrevCompletion(), (t) => t.isReadOnly == false));
            if (IsMac == true)
            {
                AddBinding(new KeyBinding(EventModifiers.Control, KeyCode.U, 
                            (t) => t.Clear(), (t) => t.isReadOnly == false));
                AddBinding(new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.LeftArrow, 
                            (t) => t.MoveToFirst()));
                AddBinding(new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.RightArrow, 
                            (t) => t.MoveToLast()));
            }
        }

        public void Execute()
        {
            this.refStack++;
            try
            {
                var commandText = this.CommandText;
                this.Text = commandText;
                if (this.histories.Contains(this.Text) == false)
                {
                    this.histories.Add(this.Text);
                    this.historyIndex = this.histories.Count;
                }
                else
                {
                    this.historyIndex = this.histories.LastIndexOf(this.Text) + 1;
                }

                this.promptText = string.Empty;
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.AppendLine(this.Prompt + commandText);

                this.readOnly = true;
                if (this.executedEvent != null)
                {
                    this.executedEvent.Invoke(commandText);
                }
                // this.promptText = this.Prompt;
                // this.text = this.outputText + this.Prompt;
                // this.caretPosition = this.text.Length;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void Clear()
        {
            this.promptText = this.prompt;
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.text = this.outputText + this.promptText;
            this.caretPosition = this.text.Length;
        }

        public void MoveToFirst()
        {
            this.caretPosition = this.outputText.Length + this.prompt.Length;
            this.UpdateLabel();
        }

        public void MoveToLast()
        {
            this.caretPosition = this.text.Length;
            this.UpdateLabel();
        }

        public new void Reset()
        {
            this.refStack++;
            try
            {
                this.outputText = string.Empty;
                this.inputText = string.Empty;
                this.promptText = this.prompt;
                this.text = this.outputText + this.promptText;
                this.caretPosition = this.text.Length;
                this.caretBlinkRate = 1;
                this.caretBlinkRate = 0;
            }
            finally
            {
                this.refStack--;
            }
        }

        public new void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        public string Text { get; private set; }

        public string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.prompt = value;
                this.promptText = this.prompt + this.inputText;
                this.text = this.outputText + this.prompt + this.inputText;
                if (this.isReadOnly == false)
                {
                    this.caretPosition = this.text.Length;
                }
            }
        }

        public void NextCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(NextCompletion);
                this.caretPosition = this.text.Length;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void PrevCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(PrevCompletion);
                this.caretPosition = this.text.Length;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.CommandText = this.histories[this.historyIndex + 1];
                this.MoveToLast();
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.CommandText = this.histories[this.historyIndex - 1];
                this.MoveToLast();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.CommandText = this.histories[0];
                this.MoveToLast();
                this.historyIndex = 0;
            }
        }

        public void InsertPrompt()
        {
            this.refStack++;
            try
            {
                // TMP_SelectionCaret
                // var textRange = new TextRange(this.promptBlock.ContentStart, this.promptBlock.ContentEnd);
                var isEnd = this.caretPosition == this.text.Length;
                // if (textRange.Text != string.Empty)
                //     this.AppendLine(textRange.Text);
                this.promptText = string.Empty;
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.promptText = this.Prompt;
                this.text = this.outputText + this.Prompt;
                //if (isEnd == true)
                this.caretPosition = this.text.Length;
                this.readOnly = false;
            }
            finally
            {
                this.refStack--;
            }
        }

        public static Match[] MatchCompletion(string text)
        {
            var matches = Regex.Matches(text, "\\S+");
            var argList = new List<Match>(matches.Count);
            foreach (Match item in matches)
            {
                argList.Add(item);
            }
            return argList.ToArray();
        }

        public static string NextCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i + 1 < completions.Length)
                            return completions[i + 1];
                        else
                            return completions.First();
                    }
                }
            }
            else
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public static string PrevCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i - 1 >= 0)
                            return completions[i - 1];
                        else
                            return completions.Last();
                    }
                }
            }
            else
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!isFocused)
                return;

            bool consumedEvent = false;
            while (Event.PopEvent(processingEvent))
            {
                if (processingEvent.rawType == EventType.KeyDown)
                {
                    if (this.OnPreviewKeyDown(processingEvent) == true)
                        continue;
                    consumedEvent = true;
                    var shouldContinue = KeyPressed(processingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        //SendOnSubmit();
                        //DeactivateInputField();
                        break;
                    }
                }

                switch (processingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (processingEvent.commandName)
                        {
                            case "SelectAll":
                                SelectAll();
                                consumedEvent = true;
                                break;
                        }
                        break;
                }
            }

            if (consumedEvent)
            {
                UpdateLabel();
                if (this.refStack == 0)
                {
                    this.promptText = this.text.Substring(this.outputText.Length);
                    this.inputText = this.promptText.Substring(this.prompt.Length);
                    this.completion = string.Empty;
                }
            }
            this.isReadOnly = this.caretPosition < this.outputText.Length + this.prompt.Length;
            eventData.Use();
        }

        public Dispatcher Dispatcher { get; private set; }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            if (this.onCompletion != null)
            {
                return this.onCompletion(items, find);
            }
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.Dispatcher = Dispatcher.Current;
            this.caretBlinkRate = 0;
            this.customCaretColor = true;
            this.caretColor = new Color(0.56862745098f, 0.56862745098f, 0.56862745098f);
            this.caretWidth = (int)(this.pointSize * 0.7f) - 1;
        }

        private void CompletionImpl(Func<string[], string, string> func)
        {
            var matches = new List<Match>(CommandStringUtility.MatchCompletion(this.inputText));
            var find = string.Empty;
            var prefix = false;
            var postfix = false;
            var leftText = this.inputText;
            if (matches.Count > 0)
            {
                var match = matches.Last();
                var matchText = match.Value;
                if (matchText.Length > 0 && matchText.First() == '\"')
                {
                    prefix = true;
                    matchText = matchText.Substring(1);
                }
                if (matchText.Length > 1 && matchText.Last() == '\"')
                {
                    postfix = true;
                    matchText = matchText.Remove(matchText.Length - 1);
                }
                if (matchText == string.Empty || matchText.Trim() != string.Empty)
                {
                    find = matchText;
                    matches.RemoveAt(matches.Count - 1);
                    leftText = this.inputText.Remove(match.Index);
                }
            }

            var argList = new List<string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var matchText = CommandStringUtility.TrimQuot(matches[i].Value).Trim();
                if (matchText != string.Empty)
                    argList.Add(matchText);
            }

            var completions = this.GetCompletion(argList.ToArray(), find);
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                var inputText = this.inputText;

                if (prefix == true || postfix == true)
                {
                    this.promptText = this.Prompt + leftText + "\"" + this.completion + "\"";
                }
                else
                {
                    this.promptText = this.Prompt + leftText + this.completion;
                }
                this.inputText = inputText;
                this.text = this.outputText + this.promptText;
                this.caretPosition = this.text.Length;
            }
        }

        private void TextBox_TextChanged(string arg0)
        {
            if (this.refStack > 0)
                return;

            this.promptText = this.text.Substring(this.outputText.Length);
            this.inputText = this.promptText.Substring(this.prompt.Length);
            this.completion = string.Empty;
        }

        private void RefreshPrompt()
        {
            this.refStack++;
            this.Clear();
            this.refStack--;
        }

        private void AppendInternal(string text)
        {
            this.refStack++;
            try
            {
                var isEnd = this.caretPosition == this.text.Length;
                // if (this.output == null || this.isChanged == true)
                // {
                //     var oldOutput = this.output;
                //     this.output = new Run();
                //     if (this.OutputForeground != null)
                //         this.output.Foreground = this.OutputForeground;
                //     if (this.OutputBackground != null)
                //         this.output.Background = this.OutputBackground;
                //     if (oldOutput == null)
                //     {
                //         this.outputBlock = new Paragraph(this.output);
                //         this.Document.Blocks.InsertBefore(this.promptBlock, this.outputBlock);
                //     }
                //     else
                //     {
                //         this.outputBlock.Inlinxs.InsertAfter(oldOutput, this.output);
                //     }
                // }
                // if (text.EndsWith(Environment.NewLine) == true)
                // {
                //     text = text.Substring(0, text.Length - Environment.NewLine.Length);
                //     this.output.Text += this.outputLeftText + text;
                //     this.outputLeftText = Environment.NewLine;
                // }
                // else
                // {
                //     this.output.Text += this.outputLeftText + text;
                //     this.outputLeftText = string.Empty;
                // }
                this.outputText += text;
                this.text = this.outputText + this.promptText;

                if (isEnd == true)
                    this.caretPosition = this.text.Length;
                // this.oldText = this.text;
                // this.oldPosition = this.caretPosition;
            }
            finally
            {
                this.refStack--;
            }
        }

        private string CommandText
        {
            get
            {
                return this.promptText.Substring(this.Prompt.Length);
            }
            set
            {
                this.refStack++;
                try
                {
                    this.promptText = this.Prompt + value;
                    this.text = this.outputText + this.promptText;
                }
                finally
                {
                    this.refStack--;
                }
            }
        }

        protected virtual bool OnPreviewKeyDown(Event e)
        {
            var keyCode = e.keyCode;
            var modifiers = e.modifiers;
            var key = $"{modifiers}+{keyCode}";
            if (bindingByKey.ContainsKey(key) == true)
            {
                var binding = bindingByKey[key];
                if (binding.Verify(this) == true && binding.Action(this) == true)
                    return true;
            }
            Debug.Log($"{modifiers}+{keyCode}");
            if (e.character == '\n' || e.character == '\t' || e.character == 25)
            {
                return true;
            }
            return false;
        }

        protected override bool IsValidChar(char c)
        {
            if (this.isReadOnly == true)
                return false;
            return base.IsValidChar(c);
        }

        private static void AddBinding(IKeyBinding binding)
        {
            bindingByKey.Add($"{binding.Modifiers}+{binding.KeyCode}", binding);
        }

        private static bool IsMac => (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);

        public delegate string[] OnCompletion(string[] items, string find);

        public class ExecutedEent : UnityEvent<string>
        {

        }

        #region ITerminal

        string ITerminal.Command => this.CommandText;

        string ITerminal.Prompt
        {
            get => this.Prompt;
            set => this.Prompt = value;
        }

        // event EventHandler ITerminal.Executed
        // {
        //     add {}
        //     remove {}
        // }

        #endregion
    }
}
