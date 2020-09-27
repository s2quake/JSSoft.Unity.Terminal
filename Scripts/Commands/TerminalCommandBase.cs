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
using System.Threading.Tasks;
using JSSoft.Unity.Terminal;
using JSSoft.Unity.Terminal.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Library.Threading;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    public abstract class TerminalCommandBase : CommandBase
    {
        protected TerminalCommandBase(ITerminal terminal)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        protected TerminalCommandBase(ITerminal terminal, string name)
            : base(name)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }
        
        protected void Write(string text)
        {
            this.Write(text, null);
        }

        protected void Write(string text, TerminalColor? foregroundColor)
        {
            this.Write(text, foregroundColor, null);
        }

        protected void Write(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            if (foregroundColor != null)
                this.Terminal.ForegroundColor = foregroundColor;
            if (backgroundColor != null)
                this.Terminal.ForegroundColor = backgroundColor;
            this.Out.Write(text);
            if (foregroundColor != null || backgroundColor != null)
                this.Terminal.ResetColor();
        }

        protected void WriteLine(string text)
        {
            this.WriteLine(text, null);
        }

        protected void WriteLine(string text, TerminalColor? foregroundColor)
        {
            this.WriteLine(text, foregroundColor, null);
        }

        protected void WriteLine(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            if (foregroundColor != null)
                this.Terminal.ForegroundColor = foregroundColor;
            if (backgroundColor != null)
                this.Terminal.ForegroundColor = backgroundColor;
            this.Out.WriteLine(text);
            if (foregroundColor != null || backgroundColor != null)
                this.Terminal.ResetColor();
        }

        protected ITerminal Terminal { get; }

        protected ITerminalGrid Grid { get; }
    }
}