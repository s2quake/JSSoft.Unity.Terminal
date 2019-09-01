using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private string commandText = string.Empty;
        private string completion;
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
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.LeftArrow,
                            (t) => true, (t) => t.caretPosition <= t.outputText.Length + t.prompt.Length));
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
            if (IsWindows == true)
            {
                AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Escape,
                            (t) => t.Clear(), (t) => t.isReadOnly == false));
                AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.Home,
                            (t) => t.MoveToFirst()));
                AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.End,
                            (t) => t.MoveToLast()));
            }
        }

        public void Execute()
        {
            var commandText = this.commandText;
            var promptText = this.promptText;
            if (this.histories.Contains(commandText) == false)
            {
                this.histories.Add(commandText);
                this.historyIndex = this.histories.Count;
            }
            else
            {
                this.historyIndex = this.histories.LastIndexOf(commandText) + 1;
            }

            this.promptText = string.Empty;
            this.inputText = string.Empty;
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.AppendLine(promptText);

            if (this.executedEvent != null)
            {
                this.readOnly = true;
                this.executedEvent.Invoke(commandText);
            }
            else
            {
                this.InsertPrompt();
            }
        }

        public void Clear()
        {
            this.inputText = string.Empty;
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.prompt;
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

        public void ResetOutput()
        {
            this.outputText = string.Empty;
            this.inputText = string.Empty;
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.prompt;
            this.text = this.outputText + this.promptText;
            this.caretPosition = this.text.Length;
            // this.caretBlinkRate = 1;
            // CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            // await Task.Delay(400);
            // this.caretBlinkRate = 0;
            // Debug.Log(this.isFocused);
            // Debug.Log(stringPositionInternal != stringSelectPositionInternal);
            // this.UpdateLabel();
        }

        public new void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        // public string Text { get; private set; }

        public string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.prompt = value;
                this.promptText = this.prompt + this.commandText;
                this.text = this.outputText + this.promptText;
                if (this.isReadOnly == false)
                {
                    this.caretPosition = this.text.Length;
                }
            }
        }

        public void NextCompletion()
        {
            this.CompletionImpl(NextCompletion);
            this.caretPosition = this.text.Length;
        }

        public void PrevCompletion()
        {
            this.CompletionImpl(PrevCompletion);
            this.caretPosition = this.text.Length;
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.commandText = this.histories[this.historyIndex + 1];
                this.MoveToLast();
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.commandText = this.histories[this.historyIndex - 1];
                this.MoveToLast();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.commandText = this.histories[0];
                this.MoveToLast();
                this.historyIndex = 0;
            }
        }

        public void InsertPrompt()
        {
            this.promptText = string.Empty;
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.Prompt;
            this.text = this.outputText + this.prompt;
            this.caretPosition = this.text.Length;
            this.readOnly = false;
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

            var consumedEvent = false;
            var promptText = this.promptText;
            while (Event.PopEvent(this.processingEvent))
            {
                if (this.processingEvent.rawType == EventType.KeyDown)
                {
                    var keyCode = this.processingEvent.keyCode;
                    var modifiers = this.processingEvent.modifiers;
                    var key = $"{modifiers}+{keyCode}";
                    if (this.OnPreviewKeyDown(this.processingEvent) == true)
                    {
                        Debug.Log($"{key}: true");
                        continue;
                    }
                    consumedEvent = true;
                    Debug.Log($"{key}: false");
                    var shouldContinue = KeyPressed(this.processingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        //SendOnSubmit();
                        //DeactivateInputField();
                        break;
                    }
                }

                switch (this.processingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (this.processingEvent.commandName)
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
                this.promptText = this.text.Substring(this.outputText.Length);
                if (this.promptText != string.Empty)
                {
                    this.inputText = this.commandText = this.promptText.Substring(this.prompt.Length);
                }
                this.completion = string.Empty;
                Debug.Log($"InputText: \"{inputText}\"");
                UpdateLabel();
            }
            this.isReadOnly = this.caretPosition < this.outputText.Length + this.prompt.Length;
            eventData.Use();
        }

        public override void Rebuild(CanvasUpdate update)
        {
            base.Rebuild(update);

            // if (this.textComponent.textInfo)
            // {

            // }

            //switch (update)
            {
                // case CanvasUpdate.LatePreRender:
                {
                    // if (this.textComponent.textInfo.characterInfo.Length > 0)
                    // {
                    //     this.textComponent.textInfo.characterInfo[0].color = new Color(255, 0, 0);
                    //     this.textComponent.textInfo.characterInfo[0].highlightColor = new Color(1, 0, 0);
                    // }
                }
                // break;
            }

            
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
            // Debug.Log($"{modifiers}+{keyCode}");
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
                    this.commandText = leftText + "\"" + this.completion + "\"";
                }
                else
                {
                    this.commandText = leftText + this.completion;
                }
                this.promptText = this.prompt + this.commandText;
                this.inputText = inputText;
                this.text = this.outputText + this.promptText;
                this.caretPosition = this.text.Length;
            }
        }

        private void AppendInternal(string text)
        {
            var isEnd = this.caretPosition == this.text.Length;
            this.outputText += text;
            this.text = this.outputText + this.promptText;
            if (isEnd == true)
                this.caretPosition = this.text.Length;
        }

        private static void AddBinding(IKeyBinding binding)
        {
            bindingByKey.Add($"{binding.Modifiers}+{binding.KeyCode}", binding);
        }

        private static bool IsMac => (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);

        private static bool IsWindows => (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);

        public delegate string[] OnCompletion(string[] items, string find);

        public class ExecutedEent : UnityEvent<string>
        {

        }

        #region ITerminal

        void ITerminal.Reset() => this.ResetOutput();

        string ITerminal.Command => this.commandText;

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
