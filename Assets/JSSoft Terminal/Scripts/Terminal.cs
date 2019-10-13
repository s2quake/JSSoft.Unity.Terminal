﻿// MIT License
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KeyBinding = JSSoft.UI.KeyBinding<JSSoft.UI.Terminal>;

namespace JSSoft.UI
{
    [AddComponentMenu("UI/Terminal", 15)]
    public class Terminal : UIBehaviour, ITerminal
    {
        private static readonly Dictionary<string, IKeyBinding> bindingByKey = new Dictionary<string, IKeyBinding>();

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
        private string commandText = string.Empty;
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

        private EventHandler<TerminalExecuteEventArgs> executed;

        static Terminal()
        {
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.UpArrow,
                                        (t) => t.PrevHistory()));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.DownArrow,
                            (t) => t.NextHistory()));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.LeftArrow,
                            (t) => t.CursorPosition--));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.RightArrow,
                            (t) => t.CursorPosition++));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.LeftArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.RightArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.UpArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.DownArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Control, KeyCode.LeftArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Control, KeyCode.RightArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Control, KeyCode.UpArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Control, KeyCode.DownArrow,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Return,
                            (t) => t.Execute(), (t) => t.IsReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.KeypadEnter,
                            (t) => t.Execute(), (t) => t.IsReadOnly == false));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.Backspace,
                            (t) => t.Backspace()));
            // ime 입력중에 Backspace 키를 누르면 두번이 호출됨 그중 처음에는 EventModifiers.None + KeyCode.Backspace 가 호출됨.
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Backspace,
                            (t) => true));
            AddBinding(new KeyBinding(EventModifiers.FunctionKey, KeyCode.Delete,
                            (t) => t.Delete()));
            AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Tab,
                            (t) => t.NextCompletion()));
            AddBinding(new KeyBinding(EventModifiers.Shift, KeyCode.Tab,
                            (t) => t.PrevCompletion()));
            if (IsMac == true)
            {
                AddBinding(new KeyBinding(EventModifiers.Control, KeyCode.U,
                            (t) => t.Clear()));
                AddBinding(new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.LeftArrow,
                            (t) => t.MoveToFirst()));
                AddBinding(new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.RightArrow,
                            (t) => t.MoveToLast()));
            }
            if (IsWindows == true)
            {
                AddBinding(new KeyBinding(EventModifiers.None, KeyCode.Escape,
                            (t) => t.Clear()));
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
            this.commandText = string.Empty;
            this.completion = string.Empty;
            this.cursorPosition = 0;
            this.AppendLine(promptText);
            this.ExecuteEvent(commandText, prompt);
        }

        public void Clear()
        {
            this.inputText = string.Empty;
            this.commandText = string.Empty;
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
            this.CursorPosition = this.commandText.Length;
        }

        public void ResetOutput()
        {
            this.outputText = string.Empty;
            this.inputText = string.Empty;
            this.commandText = string.Empty;
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
            if (this.cursorPosition < this.commandText.Length)
            {
                this.commandText = this.commandText.Remove(this.cursorPosition, 1);
                this.promptText = this.prompt + this.commandText;
                this.text = this.outputText + this.promptText;
                this.InvokePromptTextChangedEvent();
            }
        }

        public void Backspace()
        {
            if (this.cursorPosition > 0)
            {
                this.commandText = this.commandText.Remove(this.cursorPosition - 1, 1);
                this.promptText = this.prompt + this.commandText;
                this.cursorPosition--;
                this.text = this.outputText + this.promptText;
                this.InvokePromptTextChangedEvent();
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.commandText = this.histories[this.historyIndex + 1];
                this.promptText = this.prompt + this.inputText;
                this.cursorPosition = this.commandText.Length;
                this.text = this.outputText + this.promptText;
                this.historyIndex++;
                this.InvokePromptTextChangedEvent();
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.commandText = this.histories[this.historyIndex - 1];
                this.promptText = this.prompt + this.inputText;
                this.cursorPosition = this.commandText.Length;
                this.text = this.outputText + this.promptText;
                this.historyIndex--;
                this.InvokePromptTextChangedEvent();
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.commandText = this.histories[0];
                this.promptText = this.prompt + this.inputText;
                this.cursorPosition = this.commandText.Length;
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
            this.inputText = this.commandText = this.promptText.Substring(this.prompt.Length);
            this.completion = string.Empty;
            this.cursorPosition++;
            this.InvokePromptTextChangedEvent();
        }

        public bool ProcessKeyEvent(KeyCode keyCode, EventModifiers modifiers)
        {
            var key = $"{modifiers}+{keyCode}";
            if (bindingByKey.ContainsKey(key) == true)
            {
                var binding = bindingByKey[key];
                if (binding.Verify(this) == true && binding.Action(this) == true)
                    return true;
            }
            return false;
        }

        public static bool IsMac => (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);

        public static bool IsWindows => (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);

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
                this.promptText = this.prompt + this.commandText;
                this.cursorPosition = this.commandText.Length;
                this.InvokePromptTextChangedEvent();
                if (this.onDrawPrompt != null)
                {
                    this.onDrawPrompt(this.prompt, this.promptForegroundColors, this.promptBackgroundColors);
                }
            }
        }

        public string PromptText => this.promptText;

        public string CommandText => this.commandText;

        public OnCompletion onCompletion { get; set; }

        public OnDrawPrompt onDrawPrompt { get; set; }

        public event EventHandler OutputTextChanged;

        public event EventHandler PromptTextChanged;

        public event EventHandler CursorPositionChanged;

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
            this.inputText = this.commandText;
            this.promptText = this.prompt + this.commandText;
            this.text = this.outputText + this.promptText;
            this.cursorPosition = this.commandText.Length;
            Debug.Log($"{nameof(Terminal)}.{nameof(OnEnable)}1");
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.outputText != string.Empty && this.outputText.EndsWith(Environment.NewLine) == false)
            {
                this.outputText += Environment.NewLine;
            }
            this.inputText = this.commandText;
            this.promptText = this.prompt + this.commandText;
            this.text = this.outputText + this.promptText;
            this.cursorPosition = this.commandText.Length;
            this.OnOutputTextChanged(EventArgs.Empty);
            this.OnCursorPositionChanged(EventArgs.Empty);
            // Debug.Log($"{nameof(Terminal)}.{nameof(OnValidate)}");
        }
#endif

        protected virtual bool IsValidCharacter(char character)
        {
            if (character == '\n')
                return false;
            return true;
        }

        private static void AddBinding(IKeyBinding binding)
        {
            bindingByKey.Add($"{binding.Modifiers}+{binding.KeyCode}", binding);
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
                this.cursorPosition = this.commandText.Length;
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
            var eventArgs = new TerminalExecuteEventArgs(commandText);
            this.executed?.Invoke(this, eventArgs);
            this.InsertPrompt(prompt);
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

        void ITerminal.Focus()
        {

        }

        string ITerminal.Command => this.commandText;

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
    }
}
