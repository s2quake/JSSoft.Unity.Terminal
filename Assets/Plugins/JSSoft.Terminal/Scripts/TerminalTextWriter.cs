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

using System.IO;
using System.Text;

namespace JSSoft.Terminal
{
    public class TerminalTextWriter : TextWriter
    {
        private readonly ITerminal terminal;

        public TerminalTextWriter(ITerminal terminal)
        {
            this.terminal = terminal;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override async void Write(char value)
        {
            if (this.Dispatcher.CheckAccess() == true)
                this.terminal.Append($"{value}");
            else
                await this.Dispatcher.InvokeAsync(() => this.terminal.Append($"{value}"));
        }

        public override async void Write(string value)
        {
            if (this.Dispatcher.CheckAccess() == true)
                this.terminal.Append($"{value}");
            else
                await this.Dispatcher.InvokeAsync(() => this.terminal.Append($"{value}"));
        }

        public override async void WriteLine(string value)
        {
            if (this.Dispatcher.CheckAccess() == true)
                this.terminal.AppendLine($"{value}");
            else
                await this.Dispatcher.InvokeAsync(() => this.terminal.AppendLine($"{value}"));
        }

        private TerminalDispatcher Dispatcher => this.terminal.Dispatcher;
    }
}
