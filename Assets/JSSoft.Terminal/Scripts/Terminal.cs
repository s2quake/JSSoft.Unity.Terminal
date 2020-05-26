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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI
{
    [AddComponentMenu("UI/Terminal", 15)]
    [ExecuteAlways]
    public class Terminal : UIBehaviour, ITerminal, IPromptDrawer, ICommandCompletor, INotifyValidated
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

        private EventHandler<TerminalExecuteEventArgs> executing;
        private EventHandler<TerminalExecutedEventArgs> executed;

        static Terminal()
        {

        }

        public Terminal()
        {
            this.notifier = new PropertyNotifier(this.InvokePropertyChangedEvent);
        }

        public void Execute()
        {
            var commandText = this.command;
            var promptText = this.promptText;
            var prompt = this.prompt;
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
            this.notifier.End();
            this.AppendLine(promptText);
            this.ExecuteEvent(commandText, prompt);
        }

        public void Clear()
        {
            this.notifier.Begin();
            this.notifier.SetField(ref this.command, string.Empty, nameof(Command));
            this.notifier.SetField(ref this.promptText, this.prompt, nameof(PromptText));
            this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.notifier.End();
        }

        public void MoveToFirst()
        {
            this.CursorPosition = 0;
        }

        public void MoveToLast()
        {
            this.CursorPosition = this.command.Length;
        }

        public void MoveLeft()
        {
            if (this.CursorPosition > 0)
                this.CursorPosition--;
        }

        public void MoveRight()
        {
            if (this.CursorPosition < this.Command.Length)
                this.CursorPosition++;
        }

        public void ResetOutput()
        {
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
            this.notifier.End();
        }

        public void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        public void NextCompletion()
        {
            this.CompletionImpl(NextCompletion);
        }

        public void PrevCompletion()
        {
            this.CompletionImpl(PrevCompletion);
        }

        public void Delete()
        {
            if (this.cursorPosition < this.command.Length)
            {
                this.notifier.Begin();
                this.notifier.SetField(ref this.command, this.command.Remove(this.cursorPosition, 1), nameof(Command));
                this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.notifier.End();
            }
        }

        public void Backspace()
        {
            if (this.cursorPosition > 0)
            {
                this.notifier.Begin();
                this.notifier.SetField(ref this.command, this.command.Remove(this.cursorPosition - 1, 1), nameof(Command));
                this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                this.notifier.SetField(ref this.cursorPosition, this.cursorPosition - 1, nameof(CursorPosition));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.notifier.End();
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.notifier.Begin();
                this.notifier.SetField(ref this.promptText, this.prompt + this.inputText, nameof(PromptText));
                this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.inputText = this.command = this.histories[this.historyIndex + 1];
                this.historyIndex++;
                this.notifier.End();
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.notifier.Begin();
                this.notifier.SetField(ref this.promptText, this.prompt + this.inputText, nameof(PromptText));
                this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.inputText = this.command = this.histories[this.historyIndex - 1];
                this.historyIndex--;
                this.notifier.End();
            }
            else if (this.histories.Count == 1)
            {
                this.notifier.Begin();
                this.notifier.SetField(ref this.promptText, this.prompt + this.inputText, nameof(PromptText));
                this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.inputText = this.command = this.histories[0];
                this.historyIndex = 0;
                this.notifier.End();
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

        public void ResetColor()
        {
            this.notifier.Begin();
            this.notifier.SetField(ref this.foregroundColor, null, nameof(ForegroundColor));
            this.notifier.SetField(ref this.backgroundColor, null, nameof(BackgroundColor));
            this.notifier.End();
        }

        public void InsertCharacter(char character)
        {
            var index = this.outputText.Length + this.Delimiter.Length + this.prompt.Length + this.cursorPosition;
            this.notifier.Begin();
            this.notifier.SetField(ref this.text, this.text.Insert(index, $"{character}"), nameof(Text));
            this.notifier.SetField(ref this.promptText, this.text.Substring(this.outputText.Length + this.Delimiter.Length), nameof(PromptText));
            this.notifier.SetField(ref this.command, this.promptText.Substring(this.prompt.Length), nameof(Command));
            this.notifier.SetField(ref this.cursorPosition, this.cursorPosition + 1, nameof(CursorPosition));
            this.inputText = this.command;
            this.completion = string.Empty;
            this.notifier.End();
        }

        public bool ProcessKeyEvent(EventModifiers modifiers, KeyCode keyCode)
        {
            return this.KeyBindings.Process(this, modifiers, keyCode);
        }

        public IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.UI.KeyBindings.TerminalKeyBindings.GetDefaultBindings();
            set
            {
                if (this.keyBindings != value)
                {
                    this.keyBindings = value;
                    this.InvokePropertyChangedEvent(nameof(KeyBindings));
                }
            }
        }

        public TerminalColor? ForegroundColor
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

        public TerminalColor? BackgroundColor
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

        public bool IsExecuting => this.isExecuting;

        public bool IsReadOnly
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

        public bool IsVerbose
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

        public int CursorPosition
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

        public string Text => this.text;

        public string OutputText => this.outputText;

        public string Delimiter
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

        public string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.prompt != value)
                {
                    this.notifier.Begin();
                    this.notifier.SetField(ref this.prompt, value, nameof(Prompt));
                    this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                    this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                    this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                    ArrayUtility.Resize(ref this.promptForegroundColors, value.Length);
                    ArrayUtility.Resize(ref this.promptBackgroundColors, value.Length);
                    this.notifier.End();
                    this.PromptDrawer.Draw(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
                }
            }
        }

        public string PromptText => this.promptText;

        public string Command
        {
            get => this.command;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.command != value)
                {
                    this.notifier.Begin();
                    this.notifier.SetField(ref this.command, value, nameof(Command));
                    this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                    this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                    this.notifier.SetField(ref this.cursorPosition, Math.Min(this.cursorPosition, this.command.Length), nameof(CursorPosition));
                    this.inputText = value;
                    this.completion = string.Empty;
                    this.notifier.End();
                    this.PromptDrawer.Draw(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
                }
            }
        }

        public ICommandCompletor CommandCompletor
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

        public IPromptDrawer PromptDrawer
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

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
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
                this.completion = func(completions, this.completion);
                this.notifier.Begin();
                if (prefix == true || postfix == true || this.completion.IndexOf(" ") >= 0)
                {
                    this.notifier.SetField(ref this.command, leftText + "\"" + this.completion + "\"", nameof(Command));
                }
                else
                {
                    this.notifier.SetField(ref this.command, leftText + this.completion, nameof(Command));
                }
                this.notifier.SetField(ref this.promptText, this.prompt + this.command, nameof(PromptText));
                this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
                this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                this.inputText = inputText;
                this.notifier.End();
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
            this.notifier.End();
        }

        private void InsertPrompt(string prompt)
        {
            this.notifier.Begin();
            this.notifier.SetField(ref this.prompt, prompt, nameof(Prompt));
            this.notifier.SetField(ref this.promptText, prompt, nameof(PromptText));
            this.notifier.SetField(ref this.text, this.outputText + this.Delimiter + this.promptText, nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.notifier.SetField(ref this.isExecuting, false, nameof(IsExecuting));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.notifier.End();
        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
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
                this.InsertPrompt(this.prompt != string.Empty ? this.prompt : prompt);
                this.executed?.Invoke(this, new TerminalExecutedEventArgs(commandText, e));
            });
            var eventArgs = new TerminalExecuteEventArgs(commandText, action);
            this.isExecuting = true;
            this.InvokePropertyChangedEvent(nameof(IsExecuting));
            if (this.executing != null)
                this.executing.Invoke(this, eventArgs);
            else
                action(null);
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

        #region ITerminal

        void ITerminal.Reset() => this.ResetOutput();

        string ITerminal.Prompt
        {
            get => this.Prompt;
            set => this.Prompt = value;
        }

        event EventHandler<TerminalExecuteEventArgs> ITerminal.Executing
        {
            add { this.executing += value; }
            remove { this.executing -= value; }
        }

        event EventHandler<TerminalExecutedEventArgs> ITerminal.Executed
        {
            add { this.executed += value; }
            remove { this.executed -= value; }
        }

        string ITerminal.OutputText => this.outputText;

        #endregion

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
