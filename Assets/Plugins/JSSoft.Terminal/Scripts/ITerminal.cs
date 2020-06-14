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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Terminal
{
    public interface ITerminal
    {
        void Append(string value);

        void Reset();

        void ResetColor();

        void Delete();

        void Backspace();

        void NextCompletion();

        void PrevCompletion();

        void NextHistory();

        void PrevHistory();

        void Execute();

        void MoveToFirst();

        void MoveToLast();

        void MoveLeft();

        void MoveRight();

        string Command { get; set; }

        string Prompt { get; set; }

        string OutputText { get; set; }

        string Delimiter { get; }

        string PromptText { get; }

        string Text { get; }

        int CursorPosition { get; set; }

        bool IsReadOnly { get; set; }

        bool IsVerbose { get; set; }

        TerminalColor? ForegroundColor { get; set; }

        TerminalColor? BackgroundColor { get; set; }

        ICommandCompletor CommandCompletor { get; set; }

        IPromptDrawer PromptDrawer { get; set; }

        GameObject GameObject { get; }

        TerminalDispatcher Dispatcher { get; }

        event EventHandler Validated;

        event EventHandler<TerminalExecuteEventArgs> Executing;

        event EventHandler<TerminalExecutedEventArgs> Executed;

        event PropertyChangedEventHandler PropertyChanged;

        event EventHandler Enabled;

        event EventHandler Disabled;
    }
}
