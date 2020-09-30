// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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

using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    [ExecuteAlways]
    public class Terminal : TerminalBase, INotifyValidated, IPropertyChangedNotifyable
    {
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();
        private readonly PropertyNotifier notifier;
        private readonly TerminalBlock outputBlock = new TerminalBlock();
        private readonly TerminalBlock progressBlock = new TerminalBlock();
        private readonly TerminalBlock promptBlock = new TerminalBlock();
        private readonly TerminalBlock commandBlock = new TerminalBlock();

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
        private string progressText = string.Empty;
        private int historyIndex;
        private bool isExecuting;
        [SerializeField]
        private bool isReadOnly;
        private bool isChanged;
        [SerializeField]
        private bool isVerbose;
        private int cursorPosition;
        private int outputIndex = 0;
        private int progressIndex;
        private int promptIndex;
        private int commandIndex;
        private TerminalColor? foregroundColor;
        private TerminalColor? backgroundColor;
        private IKeyBindingCollection keyBindings;
        private ICommandCompletor commandCompletor;
        private ISyntaxHighlighter syntaxHighlighter;
        private IProgressGenerator progressGenerator;
        internal IProgressGenerator defaultProgressGenerator;
        [SerializeField]
        private TerminalDispatcher dispatcher;

        private EventHandler<TerminalExecuteEventArgs> executing;
        private EventHandler<TerminalExecutedEventArgs> executed;

        static Terminal()
        {
            // var cultureInfo = CultureInfo.CreateSpecificCulture("ko-KR");
            // CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            // CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
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
            this.RefreshIndex();
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
            this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.RefreshIndex();
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

        public override void Cancel()
        {
            this.OnCancellationRequested(EventArgs.Empty);
        }

        public override void ResetOutput()
        {
            var length = this.outputText.Length;
            this.notifier.Begin();
            this.notifier.SetField(ref this.outputText, string.Empty, nameof(OutputText));
            this.notifier.SetField(ref this.command, string.Empty, nameof(Command));
            this.notifier.SetField(ref this.promptText, this.prompt, nameof(PromptText));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.outputBlock.Text = this.outputText;
            this.RefreshIndex();
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
                this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
                this.inputText = this.command;
                this.RefreshIndex();
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
                this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
                this.inputText = this.command;
                this.RefreshIndex();
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
            completions = completions.ToArray();
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
            completions = completions.ToArray();
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

        public override string Progress(string message, float value)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (this.progressText != message)
            {
                var progressMessage = string.Empty;
                if (message != string.Empty || value != 0)
                    progressMessage = this.ProgressGenerator.Generate(message, value);
                var index = this.outputText.Length;
                var length = this.progressText.Length;
                var removeChange = new TextChange(index, length);
                var addChange = new TextChange(index, progressMessage.Length);
                this.notifier.Begin();
                this.notifier.SetField(ref this.progressText, progressMessage, nameof(Progress));
                this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
                this.RefreshIndex();
                this.progressBlock.Text = this.progressText;
                this.progressBlock.Highlight(this.SyntaxHighlighter, TerminalTextType.Progress);
                this.InvokeTextChangedEvent(removeChange, addChange);
                this.notifier.End();
            }
            return this.progressText;
        }

        public override TerminalData Save()
        {
            var data = new TerminalData()
            {
                Text = this.outputText,
                Prompt = this.prompt,
                Histories = this.histories.ToArray(),
                HistoryIndex = this.historyIndex
            };
            data.Foregrounds = new TerminalColor?[this.outputText.Length];
            data.Backgrounds = new TerminalColor?[this.outputText.Length];
            for (var i = 0; i < data.Foregrounds.Length; i++)
            {
                data.Foregrounds[i] = this.outputBlock.GetForegroundColor(i);
                data.Backgrounds[i] = this.outputBlock.GetBackgroundColor(i);
            }
            return data;
        }

        public override void Load(TerminalData data)
        {
            var length = this.outputText.Length;
            this.notifier.Begin();
            this.notifier.SetField(ref this.outputText, data.Text, nameof(OutputText));
            this.notifier.SetField(ref this.command, string.Empty, nameof(Command));
            this.notifier.SetField(ref this.prompt, data.Prompt, nameof(Prompt));
            this.notifier.SetField(ref this.promptText, this.prompt, nameof(PromptText));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
            this.notifier.SetField(ref this.cursorPosition, 0, nameof(CursorPosition));
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.outputBlock.Text = this.outputText;
            for (var i = 0; i < data.Text.Length; i++)
            {
                this.outputBlock.SetForegroundColor(i, data.Foregrounds[i]);
                this.outputBlock.SetBackgroundColor(i, data.Backgrounds[i]);
            }
            this.histories.Clear();
            this.histories.AddRange(data.Histories);
            this.historyIndex = data.HistoryIndex;
            this.RefreshIndex();
            this.InvokeTextChangedEvent(new TextChange(0, length));
            this.notifier.End();
        }

        public void InsertCharacter(params char[] characters)
        {
            if (this.isReadOnly == true)
                throw new InvalidOperationException();
            if (characters.Any() == true)
            {
                var text = new string(characters);
                var index = CombineLength(this.outputText, this.progressText, this.prompt) + this.cursorPosition;
                this.notifier.Begin();
                this.notifier.SetField(ref this.text, this.text.Insert(index, text), nameof(Text));
                this.notifier.SetField(ref this.promptText, this.text.Substring(CombineLength(this.outputText, this.progressText, string.Empty)), nameof(PromptText));
                this.notifier.SetField(ref this.command, this.promptText.Substring(this.prompt.Length), nameof(Command));
                this.notifier.SetField(ref this.cursorPosition, this.cursorPosition + text.Length, nameof(CursorPosition));
                this.inputText = this.command;
                this.completion = string.Empty;
                this.RefreshIndex();
                this.commandBlock.Text = this.command;
                this.commandBlock.Highlight(this.SyntaxHighlighter, TerminalTextType.Command);
                this.InvokeTextChangedEvent(new TextChange(index, text.Length));
                this.notifier.End();
            }
        }

        public bool ProcessKeyEvent(EventModifiers modifiers, KeyCode keyCode, bool isPreview)
        {
            return this.KeyBindings.Process(this, modifiers, keyCode, isPreview);
        }

        public void SetDispatcher(TerminalDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.Unity.Terminal.KeyBindings.TerminalKeyBindings.GetDefaultBindings();
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

        public int CursorIndex => this.cursorPosition + CombineLength(this.outputText, this.progressText, this.prompt);

        public override string Text => this.text;

        public override string ProgressText => this.progressText;

        [FieldName(nameof(outputText))]
        public override string OutputText => this.outputText;

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
                    this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
                    this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                    this.promptIndex = CombineLength(this.outputText, this.progressText);
                    this.commandIndex = CombineLength(this.outputText, this.progressText, this.prompt);
                    this.promptBlock.Text = this.prompt;
                    this.promptBlock.Highlight(this.SyntaxHighlighter, TerminalTextType.Prompt);
                    this.InvokeTextChangedEvent(removeChange, addChange);
                    this.notifier.End();
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
                    this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
                    this.notifier.SetField(ref this.cursorPosition, this.command.Length, nameof(CursorPosition));
                    this.commandBlock.Text = this.command;
                    this.progressIndex = CombineLength(this.outputText);
                    this.promptIndex = CombineLength(this.outputText, this.progressText);
                    this.commandIndex = CombineLength(this.outputText, this.progressText, this.prompt);
                    this.inputText = this.command;
                    this.completion = string.Empty;
                    this.commandBlock.Highlight(this.SyntaxHighlighter, TerminalTextType.Command);
                    this.InvokeTextChangedEvent(removeChange, addChange);
                    this.notifier.End();
                }
            }
        }

        public override ICommandCompletor CommandCompletor
        {
            get => this.commandCompletor ?? JSSoft.Unity.Terminal.CommandCompletor.Default;
            set
            {
                if (this.commandCompletor != value)
                {
                    this.commandCompletor = value;
                    this.InvokePropertyChangedEvent(nameof(CommandCompletor));
                }
            }
        }

        public override ISyntaxHighlighter SyntaxHighlighter
        {
            get => this.syntaxHighlighter ?? JSSoft.Unity.Terminal.SyntaxHighlighter.Default;
            set
            {
                if (this.syntaxHighlighter != value)
                {
                    this.syntaxHighlighter = value;
                    this.InvokePropertyChangedEvent(nameof(SyntaxHighlighter));
                }
            }
        }

        public override IProgressGenerator ProgressGenerator
        {
            get => this.progressGenerator ?? this.defaultProgressGenerator;
            set
            {
                if (this.progressGenerator != value)
                {
                    this.progressGenerator = value;
                    this.InvokePropertyChangedEvent(nameof(ProgressGenerator));
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

        public override event EventHandler CancellationRequested;

        internal TerminalColor? GetForegroundColor(int index)
        {
            if (this.outputBlock.Contains(index - this.outputIndex) == true)
            {
                return this.outputBlock.GetForegroundColor(index - this.outputIndex);
            }
            else if (this.progressBlock.Contains(index - this.progressIndex) == true)
            {
                return this.progressBlock.GetForegroundColor(index - this.progressIndex);
            }
            else if (this.promptBlock.Contains(index - this.promptIndex) == true)
            {
                return this.promptBlock.GetForegroundColor(index - this.promptIndex);
            }
            else if (this.commandBlock.Contains(index - this.commandIndex) == true)
            {
                return this.commandBlock.GetForegroundColor(index - this.commandIndex);
            }
            return null;
        }

        internal TerminalColor? GetBackgroundColor(int index)
        {
            if (this.outputBlock.Contains(index - this.outputIndex) == true)
            {
                return this.outputBlock.GetBackgroundColor(index - this.outputIndex);
            }
            else if (this.progressBlock.Contains(index - this.progressIndex) == true)
            {
                return this.progressBlock.GetBackgroundColor(index - this.progressIndex);
            }
            else if (this.promptBlock.Contains(index - this.promptIndex) == true)
            {
                return this.promptBlock.GetBackgroundColor(index - this.promptIndex);
            }
            else if (this.commandBlock.Contains(index - this.commandIndex) == true)
            {
                return this.commandBlock.GetBackgroundColor(index - this.commandIndex);
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

        protected virtual void OnCancellationRequested(EventArgs e)
        {
            this.CancellationRequested?.Invoke(this, e);
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            return this.commandCompletor.Complete(items, find);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.inputText = this.command;
            this.promptText = this.prompt + this.command;
            this.text = Combine(this.outputText, this.progressText, this.promptText);
            this.cursorPosition = this.command.Length;
            this.outputBlock.Text = this.outputText;
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
            this.text = Combine(this.outputText, this.progressText, this.promptText);
            this.cursorPosition = this.command.Length;
            this.outputBlock.Text = this.outputText;
            this.OnValidated(EventArgs.Empty);
        }
#endif

        protected virtual bool IsValidCharacter(char character)
        {
            return true;
        }

        internal static string Combine(string outputText, string progressMessage = "", string promptText = "")
        {
            var text = outputText;
            if (outputText != string.Empty && outputText.EndsWith(Environment.NewLine) == false)
            {
                text += Environment.NewLine;
            }
            if (progressMessage != string.Empty)
            {
                text += progressMessage;
                text += Environment.NewLine;
            }
            text += promptText;
            return text;
        }

        internal static int CombineLength(string outputText, string progressMessage = "", string promptText = "")
        {
            return Combine(outputText, progressMessage, promptText).Length;
        }

        private void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
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
            this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
            this.outputBlock.Text = this.outputText;
            for (var i = index; i < this.outputText.Length; i++)
            {
                this.outputBlock.SetForegroundColor(i, this.ForegroundColor);
                this.outputBlock.SetBackgroundColor(i, this.BackgroundColor);
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
            this.notifier.SetField(ref this.promptText, prompt + this.command, nameof(PromptText));
            this.notifier.SetField(ref this.text, Combine(this.outputText, this.progressText, this.promptText), nameof(Text));
            this.notifier.SetField(ref this.isExecuting, false, nameof(IsExecuting));
            this.RefreshIndex();
            this.promptBlock.Text = this.prompt;
            this.promptBlock.Highlight(this.SyntaxHighlighter, TerminalTextType.Prompt);
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
                this.InsertPrompt(this.prompt != string.Empty ? this.prompt : prompt);
                this.executed?.Invoke(this, new TerminalExecutedEventArgs(commandText, e));
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

        private void RefreshIndex()
        {
            this.progressIndex = CombineLength(this.outputText);
            this.promptIndex = CombineLength(this.outputText, this.progressText);
            this.commandIndex = CombineLength(this.outputText, this.progressText, this.prompt);
        }

        #region IPropertyChangedNotifyable

        void IPropertyChangedNotifyable.InvokePropertyChangedEvent(string propertyName)
        {
            this.InvokePropertyChangedEvent(propertyName);
        }

        #endregion
    }
}
