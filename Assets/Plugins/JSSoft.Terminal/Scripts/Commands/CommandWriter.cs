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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ntreev.Library.Threading;
using JSSoft.Terminal;
using UnityEngine;

namespace JSSoft.Terminal.Commands
{
    public class CommandWriter : StringWriter
    {
        private readonly ITerminal terminal;
        private readonly Dispatcher dispatcher;

        public CommandWriter(ITerminal terminal)
        {
            this.terminal = terminal;
            this.dispatcher = Dispatcher.Current;
        }

        public override void Write(char value)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(value.ToString());
            }
            else
            {
                this.dispatcher.Invoke(() => this.terminal.Append(value.ToString()));
            }
        }

        public override void WriteLine()
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(Environment.NewLine);
            }
            else
            {
                this.dispatcher.Invoke(() => this.terminal.Append(Environment.NewLine));
            }
        }

        public override void WriteLine(string value)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(value + Environment.NewLine);
            }
            else
            {
                this.dispatcher.Invoke(() => this.terminal.Append(value + Environment.NewLine));
            }
        }

        public override void Write(string value)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(value);
            }
            else
            {
                this.dispatcher.Invoke(() => this.terminal.Append(value));
            }
        }
    }
}