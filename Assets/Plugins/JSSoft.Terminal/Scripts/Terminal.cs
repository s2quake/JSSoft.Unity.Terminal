// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Terminal
{
    [ExecuteAlways]
    public class Terminal : TerminalBase, IPromptDrawer, ICommandCompletor, INotifyValidated
    {
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();
        private readonly PropertyNotifier notifier;

        private string promptText = string.Empty;
        [SerializeField]
        [TextArea(5, 10)]
        private string outputText = string.Empty;
        [SerializeField]
        private string prompt = string.Empty;
        private string inputText = string.Empty;
        [SerializeField]
        private string command = string.Empty;
        private string completion;
        private string text = string.Empty;
        private int historyIndex;
        private bool isExecuting;
        [SerializeField]
        private bool isReadOnly;
        private bool isChanged;
        [SerializeField]
        private bool isVerbose;
        private int cursorPosition;
        public TerminalColor? foregroundColor;
        public TerminalColor? backgroundColor;
        private TerminalColor?[] foregroundColors = new TerminalColor?[] { };
        private TerminalColor?[] backgroundColors = new TerminalColor?[] { };
        private TerminalColor?[] promptForegroundColors = new TerminalColor?[] { };
        private TerminalColor?[] promptBackgroundColors = new TerminalColor?[] { };
        private IKeyBindingCollection keyBindings;
        private ICommandCompletor commandCompletor;
        private IPromptDrawer promptDrawer;
        [SerializeField]
        private TerminalDispatcher dispatcher;

        private EventHandler<TerminalExecuteEventArgs> executing;
        private EventHandler<TerminalExecutedEventArgs> executed;

        static Terminal()
        {
            // Debug.Log(CultureInfo.CurrentCulture);
            var cultureInfo = CultureInfo.CreateSpecificCulture("ko-KR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            // CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("ko-KR");
            // CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
            // CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
            // CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;
            // Debug.Log(CultureInfo.CurrentCulture);
        }

        public Terminal()
        {
            this.notifier = new PropertyNotifier(this.InvokePropertyChangedEvent);
        }

        public override void Execute()
        {
            if (this.isReadOnly == true)
                throw new InvalidOperationException("terminal is readonly.");
            if (this.isExecuting == true)
                throw new InvalidOperationException("terminal is being executed.");
            var commandText = this.command;
            var promptText = this.promptText;
            var prompt = this.prompt;
            var index = this.text.Length - this.command.Length;
            var length = this.command.Length;
            if (this.histories.Contains(commandText) == false)
            {
                this.histories.Add(commandText);
                this.historyIndex = this.histories.Count;
            }
            else
            {
                this.historyIndex = this.histories.LastIndexOf(commandText) + 1;
            }

            this.notifier.Begin();
            this.notifier.SetField(ref this.prompt, string.Empty, nameof(Prompt));
            this.notifier.SetField(ref this.promptText, string.Empty, nameof(PromptText));
            this.notifier.SetField(ref this.command, string.Empty, nameof(Command));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.InvokeTextChangedEvent(new TextChange(index, length));
            this.notifier.End();
            this.AppendLine(promptText);
            this.ExecuteEvent(commandText, prompt);
        }

        public void Clear()
        {
            var index = this.text.Length - this.command.Length;
            var length = this.command.Length;
            this.notifier.Begin();
            this.notifier.SetField(ref this.command, string.Empty, nameof(Command));
            this.notifier.SetField(ref this.promptText, this.prompt, nameof(PromptText));
            this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.InvokeTextChangedEvent(new TextChange(index, length));
            this.notifier.End();
        }

        public override void MoveToFirst()
        {
            this.CursorPosition = 0;
        }

        public override void MoveToLast()
        {
            this.CursorPosition = this.command.Length;
        }

        public override void MoveLeft()
        {
            if (this.CursorPosition > 0)
                this.CursorPosition--;
        }

        public override void MoveRight()
        {
            if (this.CursorPosition < this.Command.Length)
                this.CursorPosition++;
        }

        public override void ResetOutput()
        {
            var length = this.outputText.Length;
            this.notifier.Begin();
            this.notifier.SetField(ref this.outputText, string.Empty, nameof(OutputText));
            this.notifier.SetField(ref this.command, string.Empty, nameof(Command));
            this.notifier.SetField(ref this.promptText, this.prompt, nameof(PromptText));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.foregroundColors = new TerminalColor?[] { };
            this.backgroundColors = new TerminalColor?[] { };
            this.InvokeTextChangedEvent(new TextChange(0, length));
            this.notifier.End();
        }

        public override void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        public override void NextCompletion()
        {
            this.CompletionImpl(NextCompletion);
        }

        public override void PrevCompletion()
        {
            this.CompletionImpl(PrevCompletion);
        }

        public override void Delete()
        {
            if (this.cursorPosition < this.command.Length)
            {
                var index = this.CursorIndex + 1;
                this.notifier.Begin();
                this.notifier.SetField(ref this.command, this.command.Remove(this.cursorPosition, 1), nameof(Command));
                this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.inputText = this.command;
                this.InvokeTextChangedEvent(new TextChange(index, -1));
                this.notifier.End();
            }
        }

        public override void Backspace()
        {
            if (this.cursorPosition > 0)
            {
                var index = this.CursorIndex;
                var length = 1;
                this.notifier.Begin();
                this.notifier.SetField(ref this.command, this.command.Remove(this.cursorPosition - length, length), nameof(Command));
                this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                this.notifier.SetField(ref this.cursorPosition, this.cursorPosition - length, nameof(CursorPosition));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.inputText = this.command;
                this.InvokeTextChangedEvent(new TextChange(index, -length));
                this.notifier.End();
            }
        }

        public override void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.historyIndex++;
                this.Command = this.histories[this.historyIndex];
            }
        }

        public override void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.historyIndex--;
                this.Command = this.histories[this.historyIndex];
            }
            else if (this.histories.Count == 1)
            {
                this.historyIndex = 0;
                this.Command = this.histories[this.historyIndex];
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

        public override void ResetColor()
        {
            this.notifier.Begin();
            this.notifier.SetField(ref this.foregroundColor, null, nameof(ForegroundColor));
            this.notifier.SetField(ref this.backgroundColor, null, nameof(BackgroundColor));
            this.notifier.End();
        }

        public void InsertCharacter(char character)
        {
            if (this.isReadOnly == true || this.isExecuting == true)
                throw new InvalidOperationException();
            var index = this.outputText.Length + this.Delimiter.Length + this.prompt.Length + this.cursorPosition;
            this.notifier.Begin();
            this.notifier.SetField(ref this.text, this.text.Insert(index, $"{character}"), nameof(Text));
            this.notifier.SetField(ref this.promptText, this.text.Substring(this.outputText.Length + this.Delimiter.Length), nameof(PromptText));
            this.notifier.SetField(ref this.command, this.promptText.Substring(this.prompt.Length), nameof(Command));
            this.notifier.SetField(ref this.cursorPosition, this.cursorPosition + 1, nameof(CursorPosition));
            this.inputText = this.command;
            this.completion = string.Empty;
            this.InvokeTextChangedEvent(new TextChange(index, $"{character}".Length));
            this.notifier.End();
        }

        public bool ProcessKeyEvent(EventModifiers modifiers, KeyCode keyCode)
        {
            return this.KeyBindings.Process(this, modifiers, keyCode);
        }

        public void SetDispatcher(TerminalDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.Terminal.KeyBindings.TerminalKeyBindings.GetDefaultBindings();
            set
            {
                if (this.keyBindings != value)
                {
                    this.keyBindings = value;
                    this.InvokePropertyChangedEvent(nameof(KeyBindings));
                }
            }
        }

        public override TerminalColor? ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                if (this.foregroundColor != value)
                {
                    this.foregroundColor = value;
                    this.InvokePropertyChangedEvent(nameof(ForegroundColor));
                }
            }
        }

        public override TerminalColor? BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                if (this.backgroundColor != value)
                {
                    this.backgroundColor = value;
                    this.InvokePropertyChangedEvent(nameof(BackgroundColor));
                }
            }
        }

        public override bool IsExecuting => this.isExecuting;

        [FieldName(nameof(isReadOnly))]
        public override bool IsReadOnly
        {
            get => this.isReadOnly;
            set
            {
                if (this.isReadOnly != value)
                {
                    this.isReadOnly = value;
                    this.InvokePropertyChangedEvent(nameof(IsReadOnly));
                }
            }
        }

        [FieldName(nameof(isVerbose))]
        public override bool IsVerbose
        {
            get => this.isVerbose;
            set
            {
                if (this.isVerbose != value)
                {
                    this.isVerbose = value;
                    this.InvokePropertyChangedEvent(nameof(IsVerbose));
                }
            }
        }

        public override int CursorPosition
        {
            get => this.cursorPosition;
            set
            {
                if (value < 0 || value > this.command.Length)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.cursorPosition != value)
                {
                    this.cursorPosition = value;
                    this.inputText = this.command.Substring(0, this.cursorPosition);
                    this.InvokePropertyChangedEvent(nameof(CursorPosition));
                }
            }
        }

        public int CursorIndex => this.CursorPosition + this.OutputText.Length + this.Delimiter.Length + this.Prompt.Length;

        public override string Text => this.text;

        [FieldName(nameof(outputText))]
        public override string OutputText => this.outputText;

        public override string Delimiter
        {
            get
            {
                if (this.outputText != string.Empty && this.outputText.EndsWith(Environment.NewLine) == false)
                {
                    return Environment.NewLine;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [FieldName(nameof(prompt))]
        public override string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.prompt != value)
                {
                    var index = this.text.Length - this.promptText.Length;
                    var length = this.prompt.Length;
                    var removeChange = new TextChange(index + length, length);
                    var addChange = new TextChange(index, value.Length);
                    this.notifier.Begin();
                    this.notifier.SetField(ref this.prompt, value, nameof(Prompt));
                    this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                    this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                    this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                    ArrayUtility.Resize(ref this.promptForegroundColors, value.Length);
                    ArrayUtility.Resize(ref this.promptBackgroundColors, value.Length);
                    this.InvokeTextChangedEvent(removeChange, addChange);
                    this.notifier.End();
                    this.PromptDrawer.Draw(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
                }
            }
        }

        public override string PromptText => this.promptText;

        [FieldName(nameof(command))]
        public override string Command
        {
            get => this.command;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.command != value)
                {
                    var index = this.text.Length - this.command.Length;
                    var length = this.command.Length;
                    var removeChange = new TextChange(index + length, -length);
                    var addChange = new TextChange(index, value.Length);
                    this.notifier.Begin();
                    this.notifier.SetField(ref this.command, value, nameof(Command));
                    this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                    this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                    this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                    this.inputText = value;
                    this.completion = string.Empty;
                    this.InvokeTextChangedEvent(removeChange, addChange);
                    this.notifier.End();
                    this.PromptDrawer.Draw(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
                }
            }
        }

        public override ICommandCompletor CommandCompletor
        {
            get => this.commandCompletor ?? this;
            set
            {
                if (this.commandCompletor != value)
                {
                    this.commandCompletor = value;
                    this.InvokePropertyChangedEvent(nameof(CommandCompletor));
                }
            }
        }

        public override IPromptDrawer PromptDrawer
        {
            get => this.promptDrawer ?? this;
            set
            {
                if (this.promptDrawer != value)
                {
                    this.promptDrawer = value;
                    this.InvokePropertyChangedEvent(nameof(PromptDrawer));
                }
            }
        }

        [FieldName(nameof(dispatcher))]
        public override TerminalDispatcher Dispatcher => this.dispatcher;

        public override event EventHandler<TerminalExecuteEventArgs> Executing
        {
            add { this.executing += value; }
            remove { this.executing -= value; }
        }

        public override event EventHandler<TerminalExecutedEventArgs> Executed
        {
            add { this.executed += value; }
            remove { this.executed -= value; }
        }

        public override event EventHandler Validated;

        public override event PropertyChangedEventHandler PropertyChanged;

        public override event EventHandler<TextChangedEventArgs> TextChanged;

        public override event EventHandler Enabled;

        public override event EventHandler Disabled;

        internal void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        internal TerminalColor? GetForegroundColor(int index)
        {
            if (index < this.outputText.Length && index < this.foregroundColors.Length)
            {
                return this.foregroundColors[index];
            }
            var promptIndex = index - (this.outputText.Length + this.Delimiter.Length);
            if (promptIndex >= 0 && promptIndex < this.promptForegroundColors.Length)
            {
                return this.promptForegroundColors[promptIndex];
            }
            return null;
        }

        internal TerminalColor? GetBackgroundColor(int index)
        {
            if (index < this.outputText.Length && index < this.backgroundColors.Length)
            {
                return this.backgroundColors[index];
            }
            var promptIndex = index - (this.outputText.Length + this.Delimiter.Length);
            if (promptIndex >= 0 && promptIndex < this.promptBackgroundColors.Length)
            {
                return this.promptBackgroundColors[promptIndex];
            }
            return null;
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnTextChanged(TextChangedEventArgs e)
        {
            this.TextChanged?.Invoke(this, e);
        }

        protected virtual void OnEnabled(EventArgs e)
        {
            this.Enabled?.Invoke(this, e);
        }

        protected virtual void OnDisabled(EventArgs e)
        {
            this.Disabled?.Invoke(this, e);
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            return this.CommandCompletor.Complete(items, find);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.inputText = this.command;
            this.promptText = this.prompt + this.command;
            this.text = this.outputText + this.Delimiter + this.promptText;
            this.cursorPosition = this.command.Length;
            ArrayUtility.Resize(ref this.foregroundColors, this.outputText.Length);
            ArrayUtility.Resize(ref this.backgroundColors, this.outputText.Length);
            TerminalEvents.Register(this);
            this.OnEnabled(EventArgs.Empty);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.OnDisabled(EventArgs.Empty);
            TerminalEvents.Unregister(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.inputText = this.command;
            this.promptText = this.prompt + this.command;
            this.text = this.outputText + this.Delimiter + this.promptText;
            this.cursorPosition = this.command.Length;
            ArrayUtility.Resize(ref this.foregroundColors, this.outputText.Length);
            ArrayUtility.Resize(ref this.backgroundColors, this.outputText.Length);
            this.OnValidated(EventArgs.Empty);
        }
#endif

        protected virtual bool IsValidCharacter(char character)
        {
            if (character == '\n')
                return false;
            return true;
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
            var inputText = this.inputText;
            if (completions != null && completions.Any())
            {
                var command = string.Empty;
                var completion = func(completions, this.completion);
                if (prefix == true || postfix == true || completion.IndexOf(" ") >= 0)
                {
                    command = leftText + "\"" + completion + "\"";
                }
                else
                {
                    command = leftText + completion;
                }
                this.Command = command;
                this.inputText = inputText;
                this.completion = completion;
            }
        }

        private void AppendInternal(string text)
        {
            var index = this.outputText.Length;
            this.notifier.Begin();
            this.notifier.SetField(ref this.outputText, this.outputText + text, nameof(OutputText));
            this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
            ArrayUtility.Resize(ref this.foregroundColors, this.outputText.Length);
            ArrayUtility.Resize(ref this.backgroundColors, this.outputText.Length);
            for (var i = index; i < this.outputText.Length; i++)
            {
                this.foregroundColors[i] = this.ForegroundColor;
                this.backgroundColors[i] = this.BackgroundColor;
            }
            this.InvokeTextChangedEvent(new TextChange(index, text.Length));
            this.notifier.End();
        }

        private void InsertPrompt(string prompt)
        {
            var index = this.text.Length - this.promptText.Length;
            var length = this.prompt.Length;
            var removeChange = new TextChange(index + length, -length);
            var addChange = new TextChange(index, prompt.Length);
            this.notifier.Begin();
            this.notifier.SetField(ref this.prompt, prompt, nameof(Prompt));
            this.notifier.SetField(ref this.promptText, prompt, nameof(PromptText));
            this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.notifier.SetField(ref this.isExecuting, false, nameof(IsExecuting));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.InvokeTextChangedEvent(removeChange, addChange);
            this.notifier.End();
        }

        private IEnumerable<string> GetDependencyProperties(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Text):
                    {
                        yield return nameof(CursorPosition);
                    }
                    break;
            }
        }

        private void ExecuteEvent(string commandText, string prompt)
        {
            var action = new Action<Exception>((e) =>
            {
                this.executed?.Invoke(this, new TerminalExecutedEventArgs(commandText, e));
                this.InsertPrompt(this.prompt != string.Empty ? this.prompt : prompt);
            });
            var eventArgs = new TerminalExecuteEventArgs(commandText, action);
            this.InvokePropertyChangedEvent(nameof(IsExecuting));
            this.isExecuting = true;
            if (this.executing != null)
                this.executing.Invoke(this, eventArgs);
            else
                action(null);
        }

        private void InvokeTextChangedEvent(params TextChange[] changes)
        {
            this.OnTextChanged(new TextChangedEventArgs(changes));
        }

        #region IPromptDrawer

        void IPromptDrawer.Draw(string command, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {

        }

        #endregion

        #region ICommandCompletor

        string[] ICommandCompletor.Complete(string[] items, string find)
        {
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        #endregion
    }
}
