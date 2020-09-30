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

using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    public abstract class TerminalBase : UIBehaviour, ITerminal
    {
        public abstract string Command { get; set; }

        public abstract string Prompt { get; set; }

        public abstract string OutputText { get; }

        public abstract string PromptText { get; }

        public abstract string Text { get; }

        public abstract string ProgressText { get; }

        public abstract int CursorPosition { get; set; }

        public abstract bool IsReadOnly { get; set; }

        public abstract bool IsVerbose { get; set; }

        public abstract bool IsExecuting { get; }

        public abstract TerminalColor? ForegroundColor { get; set; }

        public abstract TerminalColor? BackgroundColor { get; set; }

        public abstract ICommandCompletor CommandCompletor { get; set; }

        public abstract ISyntaxHighlighter SyntaxHighlighter { get; set; }

        public abstract IProgressGenerator ProgressGenerator { get; set; }

        public abstract TerminalDispatcher Dispatcher { get; }
     
        public abstract event EventHandler Validated;

        public abstract event EventHandler Enabled;

        public abstract event EventHandler Disabled;

        public abstract event EventHandler CancellationRequested;

        public abstract event EventHandler<TerminalExecuteEventArgs> Executing;

        public abstract event EventHandler<TerminalExecutedEventArgs> Executed;

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract event EventHandler<TextChangedEventArgs> TextChanged;

        public abstract void Append(string value);

        public abstract void Backspace();

        public abstract void Delete();

        public abstract void Execute();

        public abstract void MoveLeft();

        public abstract void MoveRight();

        public abstract void MoveToFirst();

        public abstract void MoveToLast();

        public abstract void NextCompletion();

        public abstract void NextHistory();

        public abstract void PrevCompletion();

        public abstract void PrevHistory();

        public abstract void ResetColor();

        public abstract void ResetOutput();

        public abstract void Cancel();

        public abstract string Progress(string message, float value);

        #region ITerminal

        void ITerminal.Reset() => this.ResetOutput();

        GameObject ITerminal.GameObject => this.gameObject;

        #endregion
    }
}
