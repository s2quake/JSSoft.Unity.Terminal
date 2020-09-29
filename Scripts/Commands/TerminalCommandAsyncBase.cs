﻿// MIT License
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
using System.Threading.Tasks;

namespace JSSoft.Unity.Terminal.Commands
{
    public abstract class TerminalCommandAsyncBase : CommandAsyncBase
    {
        protected TerminalCommandAsyncBase(ITerminal terminal)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        protected Task WriteAsync(string text)
        {
            return this.WriteAsync(text, null);
        }

        protected Task WriteAsync(string text, TerminalColor? foregroundColor)
        {
            return this.WriteAsync(text, foregroundColor, null);
        }

        protected Task WriteAsync(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                if (foregroundColor != null)
                    this.Terminal.ForegroundColor = foregroundColor;
                if (backgroundColor != null)
                    this.Terminal.ForegroundColor = backgroundColor;
                this.Out.Write(text);
                if (foregroundColor != null || backgroundColor != null)
                    this.Terminal.ResetColor();
            });
        }

        protected Task WriteLineAsync()
        {
            return this.WriteLineAsync(string.Empty);
        }

        protected Task WriteLineAsync(string text)
        {
            return this.WriteLineAsync(text, null);
        }

        protected Task WriteLineAsync(string text, TerminalColor? foregroundColor)
        {
            return this.WriteLineAsync(text, foregroundColor, null);
        }

        protected Task WriteLineAsync(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                if (foregroundColor != null)
                    this.Terminal.ForegroundColor = foregroundColor;
                if (backgroundColor != null)
                    this.Terminal.ForegroundColor = backgroundColor;
                this.Out.WriteLine(text);
                if (foregroundColor != null || backgroundColor != null)
                    this.Terminal.ResetColor();
            });
        }

        protected Task CompleteProgressAsync(string message)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                var progressText = this.Terminal.Progress(message, 1);
                this.Terminal.Progress(string.Empty, 0);
                this.Out.WriteLine(progressText);
            });
        }

        protected Task CancelProgressAasync(string message)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                var progressText = this.Terminal.ProgressText;
                this.Terminal.Progress(string.Empty, 0);
                this.Out.WriteLine(progressText);
            });
        }

        protected Task<string> SetProgressAsync(string message, float value)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                this.Terminal.Progress(message, value);
                return this.Terminal.ProgressText;
            });
        }

        protected ITerminal Terminal { get; }

        protected ITerminalGrid Grid { get; }

        protected TerminalDispatcher Dispatcher => this.Terminal.Dispatcher;
    }
}