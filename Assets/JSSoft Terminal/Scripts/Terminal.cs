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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI
{
    [AddComponentMenu("UI/Terminal", 15)]
    [ExecuteAlways]
    public class Terminal : UIBehaviour, ITerminal, IPromptDrawer, ICommandCompletor
    {
        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();

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
        private bool isReadOnly;
        private bool isChanged;
        private int cursorPosition;
        private string compositionString;
        private Color32?[] foregroundColors = new Color32?[] { };
        private Color32?[] backgroundColors = new Color32?[] { };
        private Color32?[] promptForegroundColors = new Color32?[] { };
        private Color32?[] promptBackgroundColors = new Color32?[] { };
        private IKeyBindingCollection keyBindings;
        private ICommandCompletor commandCompletor;
        private IPromptDrawer promptDrawer;

        private EventHandler<TerminalExecuteEventArgs> executed;

        static Terminal()
        {

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

            this.prompt = string.Empty;
            this.promptText = string.Empty;
            this.inputText = string.Empty;
            this.command = string.Empty;
            this.completion = string.Empty;
            this.cursorPosition = 0;
            this.AppendLine(promptText);
            this.ExecuteEvent(commandText, prompt);
        }

        public void Clear()
        {
            this.inputText = string.Empty;
            this.command = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.prompt;
            this.text = this.outputText + this.promptText;
            this.cursorPosition = 0;
            this.InvokePromptTextChangedEvent();
        }

        public void MoveToFirst()
        {
            this.CursorPosition = 0;
        }

        public void MoveToLast()
        {
            this.CursorPosition = this.command.Length;
        }

        public void ResetOutput()
        {
            this.outputText = string.Empty;
            this.inputText = string.Empty;
            this.command = string.Empty;
            this.completion = string.Empty;
            this.promptText = this.prompt;
            this.cursorPosition = 0;
            this.foregroundColors = new Color32?[] { };
            this.backgroundColors = new Color32?[] { };
            this.text = this.outputText + this.promptText;
            this.cursorPosition = 0;
            this.InvokeOutputTextChangedEvent();
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
                this.command = this.command.Remove(this.cursorPosition, 1);
                this.promptText = this.prompt + this.command;
                this.text = this.outputText + this.promptText;
                this.InvokePromptTextChangedEvent();
            }
        }

        public void Backspace()
        {
            if (this.cursorPosition > 0)
            {
                this.command = this.command.Remove(this.cursorPosition - 1, 1);
                this.promptText = this.prompt + this.command;
                this.cursorPosition--;
                this.text = this.outputText + this.promptText;
                this.InvokePromptTextChangedEvent();
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.command = this.histories[this.historyIndex + 1];
                this.promptText = this.prompt + this.inputText;
                this.cursorPosition = this.command.Length;
                this.text = this.outputText + this.promptText;
                this.historyIndex++;
                this.InvokePromptTextChangedEvent();
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.command = this.histories[this.historyIndex - 1];
                this.promptText = this.prompt + this.inputText;
                this.cursorPosition = this.command.Length;
                this.text = this.outputText + this.promptText;
                this.historyIndex--;
                this.InvokePromptTextChangedEvent();
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.command = this.histories[0];
                this.promptText = this.prompt + this.inputText;
                this.cursorPosition = this.command.Length;
                this.text = this.outputText + this.promptText;
                this.historyIndex = 0;
                this.InvokePromptTextChangedEvent();
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
            this.ForegroundColor = null;
            this.BackgroundColor = null;
        }

        public void InsertCharacter(char character)
        {
            var index = this.outputText.Length + this.prompt.Length + this.cursorPosition;
            this.text = this.text.Insert(index, $"{character}");
            this.promptText = this.Text.Substring(this.outputText.Length);
            this.inputText = this.command = this.promptText.Substring(this.prompt.Length);
            this.completion = string.Empty;
            this.cursorPosition++;
            this.InvokePromptTextChangedEvent();
        }

        public bool ProcessKeyEvent(EventModifiers modifiers, KeyCode keyCode)
        {
            return this.KeyBindings.Process(this, modifiers, keyCode);
        }

        public IKeyBindingCollection KeyBindings
        {
            get => this.keyBindings ?? JSSoft.UI.KeyBindings.TerminalKeyBindings.GetDefaultBindings();
            set => this.keyBindings = value;
        }

        public Color32? ForegroundColor { get; set; }

        public Color32? BackgroundColor { get; set; }

        public bool IsReadOnly => this.isReadOnly;

        public int CursorPosition
        {
            get => this.cursorPosition;
            set
            {
                this.cursorPosition = value;
                if (this.cursorPosition < 0)
                    this.cursorPosition = 0;
                if (this.cursorPosition > this.promptText.Length - this.prompt.Length)
                    this.cursorPosition = this.promptText.Length - this.prompt.Length;
                this.OnCursorPositionChanged(EventArgs.Empty);
            }
        }

        public string Text => this.text;

        public string OutputText => this.outputText;

        public string Prompt
        {
            get => this.prompt;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.prompt = value;
                this.promptForegroundColors = new Color32?[value.Length];
                this.promptBackgroundColors = new Color32?[value.Length];
                this.promptText = this.prompt + this.command;
                this.text = this.outputText + this.promptText;
                this.cursorPosition = this.command.Length;
                this.InvokePromptTextChangedEvent();
                this.PromptDrawer.Draw(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
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
                this.command = value;
                this.promptText = this.prompt + this.command;
                this.text = this.outputText + this.promptText;
                this.cursorPosition = Math.Min(this.cursorPosition, this.command.Length);
                this.InvokePromptTextChangedEvent();
                this.PromptDrawer.Draw(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
            }
        }

        public ICommandCompletor CommandCompletor
        {
            get => this.commandCompletor ?? this;
            set
            {
                this.commandCompletor = value;
            }
        }

        public IPromptDrawer PromptDrawer
        {
            get => this.promptDrawer ?? this;
            set
            {
                this.promptDrawer = value;
            }
        }

        public event EventHandler Validated;

        public event EventHandler OutputTextChanged;

        public event EventHandler PromptTextChanged;

        public event EventHandler CursorPositionChanged;

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnOutputTextChanged(EventArgs e)
        {
            this.OutputTextChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPropmtTextChanged(EventArgs e)
        {
            this.PromptTextChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCursorPositionChanged(EventArgs e)
        {
            this.CursorPositionChanged?.Invoke(this, EventArgs.Empty);
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
            this.text = this.outputText + this.promptText;
            this.cursorPosition = this.command.Length;
            TerminalEvents.Register(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalEvents.Unregister(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.outputText != string.Empty && this.outputText.EndsWith(Environment.NewLine) == false)
            {
                this.outputText += Environment.NewLine;
            }
            this.inputText = this.command;
            this.promptText = this.prompt + this.command;
            this.text = this.outputText + this.promptText;
            this.cursorPosition = this.command.Length;
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
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                var inputText = this.inputText;
                if (prefix == true || postfix == true)
                {
                    this.command = leftText + "\"" + this.completion + "\"";
                }
                else
                {
                    this.command = leftText + this.completion;
                }
                this.promptText = this.prompt + this.command;
                this.inputText = inputText;
                this.text = this.outputText + this.promptText;
                this.cursorPosition = this.command.Length;
                this.InvokePromptTextChangedEvent();
            }
        }

        private void AppendInternal(string text)
        {
            var index = this.outputText.Length;
            this.outputText += text;
            Array.Resize(ref this.foregroundColors, this.outputText.Length);
            Array.Resize(ref this.backgroundColors, this.outputText.Length);
            for (var i = index; i < this.outputText.Length; i++)
            {
                this.foregroundColors[i] = this.ForegroundColor;
                this.backgroundColors[i] = this.BackgroundColor;
            }
            this.text = this.outputText + this.promptText;
            this.InvokeOutputTextChangedEvent();
        }

        private void InsertPrompt(string prompt)
        {
            this.prompt = prompt;
            this.inputText = string.Empty;
            this.completion = string.Empty;
            this.promptText = prompt;
            this.text = this.outputText + this.promptText;
            this.cursorPosition = 0;
            this.isReadOnly = false;
            this.InvokePromptTextChangedEvent();
        }

        private void InvokePromptTextChangedEvent()
        {
            this.OnPropmtTextChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
        }

        private void InvokeOutputTextChangedEvent()
        {
            this.OnOutputTextChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
        }

        private void ExecuteEvent(string commandText, string prompt)
        {
            var action = new Action(() => this.InsertPrompt(this.prompt != string.Empty ? this.prompt : prompt));
            var eventArgs = new TerminalExecuteEventArgs(commandText, action);
            this.isReadOnly = true;
            this.executed?.Invoke(this, eventArgs);
            if (eventArgs.IsAsync == false)
            {
                eventArgs.Handled = true;
            }
        }

        internal Color32? GetForegroundColor(int index)
        {
            if (index < this.Text.Length && this.Text[index] == ' ')
            {
                return TerminalColors.Transparent;
            }
            if (index < this.foregroundColors.Length)
            {
                return this.foregroundColors[index];
            }
            var promptIndex = index - this.outputText.Length;
            if (promptIndex >= 0 && promptIndex < this.promptForegroundColors.Length)
            {
                return this.promptForegroundColors[promptIndex];
            }
            return null;
        }

        internal Color32? GetBackgroundColor(int index)
        {
            // if (index % 2 != 0)
            //     return TerminalColors.Blue;
            // else
            //     return TerminalColors.Red;
            if (index < this.backgroundColors.Length)
            {
                return this.backgroundColors[index];
            }
            var promptIndex = index - this.outputText.Length;
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

        event EventHandler<TerminalExecuteEventArgs> ITerminal.Executed
        {
            add { this.executed += value; }
            remove { this.executed -= value; }
        }

        string ITerminal.OutputText => this.outputText;

        #endregion

        #region IPromptDrawer

        void IPromptDrawer.Draw(string command, Color32?[] foregroundColors, Color32?[] backgroundColors)
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
