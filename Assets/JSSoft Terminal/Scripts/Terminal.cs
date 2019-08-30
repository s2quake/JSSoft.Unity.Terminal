using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();
        private int historyIndex;

        private string promptText = string.Empty;
        private string outputText = string.Empty;
        private string prompt = string.Empty;
        private string inputText = string.Empty;
        private int refStack = 0;
        private string completion;
        private bool isReadOnly;

        private bool isChanged;

        private ExecutedEent executedEvent = new ExecutedEent();

        public ExecutedEent onExecuted
        {
            get => executedEvent;
            set => executedEvent = value;
        }

        public OnCompletion onCompletion { get; set; }

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
            var text = this.text ?? string.Empty;
            var promptText = this.prompt + this.inputText;
            text = text.Substring(0, text.Length - promptText.Length);
            //this.promptText = this.Prompt;
            this.text = text + this.prompt;
            this.inputText = string.Empty;
            this.completion = string.Empty;
        }

        public void MoveToFirst()
        {
            this.caretPosition = this.text.Length - this.promptText.Length;
        }

        public void MoveToLast()
        {
            this.caretPosition = this.text.Length;
        }

        // public void Reset()
        // {
        //     this.refStack++;
        //     try
        //     {
        //         // this.output = null;
        //         // this.outputBlock = null;
        //         // this.outputLeftText = string.Empty;
        //         // this.Document.Blocks.Clear();
        //         // this.promptBlock = new Paragraph();
        //         // this.Document.Blocks.Add(this.promptBlock);
        //         // this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
        //         // this.CaretPosition = this.promptBlock.Inlines.LastInline.ContentEnd;
        //     }
        //     finally
        //     {
        //         this.refStack--;
        //     }
        // }

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

        public class ExecutedEent : UnityEvent<string>
        {

        }

        protected virtual bool OnPreviewKeyDown(Event e)
        {
            var key = e.keyCode;
            var modifiers = e.modifiers;
            if (e.character == '\n' || e.character == '\t')
            {
                return true;
            }
            if (key == KeyCode.Backspace)
            {
                //if (modifiers == EventModifiers.None)
                {
                    var inputPosition = this.outputText.Length + this.prompt.Length;
                    if (this.caretPosition <= inputPosition)
                        return true;
                }
            }
            else if (key == KeyCode.UpArrow)
            {
                if (this.isReadOnly == false)
                {
                    this.PrevHistory();
                    return true;
                }
            }
            else if (key == KeyCode.DownArrow)
            {
                if (this.isReadOnly == false)
                {
                    this.NextHistory();
                    return true;
                }
            }
            else if (key == KeyCode.Return || key == KeyCode.KeypadEnter)
            {
                if (this.isReadOnly == false)
                {
                    this.Execute();
                    return true;
                }
            }
            else if (key == KeyCode.Tab)
            {
                var shift = modifiers.HasFlag(EventModifiers.Shift);
                if (shift == false)
                {
                    this.NextCompletion();
                    return true;
                }
                else if (shift == true)
                {
                    this.PrevCompletion();
                    return true;
                }
            }
            return false;
        }

        private Event m_ProcessingEvent = new Event();

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!isFocused)
                return;

            bool consumedEvent = false;
            while (Event.PopEvent(m_ProcessingEvent))
            {
                if (m_ProcessingEvent.rawType == EventType.KeyDown)
                {
                    if (this.OnPreviewKeyDown(m_ProcessingEvent) == true)
                        continue;
                    consumedEvent = true;
                    var shouldContinue = KeyPressed(m_ProcessingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        //SendOnSubmit();
                        //DeactivateInputField();
                        break;
                    }
                }

                switch (m_ProcessingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (m_ProcessingEvent.commandName)
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

        protected override bool IsValidChar(char c)
        {
            if (this.isReadOnly == true)
                return false;
            return base.IsValidChar(c);
        }

        public delegate string[] OnCompletion(string[] items, string find);

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
