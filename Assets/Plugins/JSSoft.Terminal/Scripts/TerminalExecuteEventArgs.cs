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

namespace JSSoft.Terminal
{
    public class TerminalExecuteEventArgs : EventArgs
    {
        private readonly Action<Exception> action;
        private bool isHandled;

        public TerminalExecuteEventArgs(string command, Action<Exception> action)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public string Command { get; }

        public bool IsHandled => this.isHandled;

        public void Success()
        {
            if (this.isHandled == true)
                throw new InvalidOperationException("command expired.");
            this.isHandled = true;
            this.action(null);
        }

        public void Fail(Exception exception)
        {
            if (this.isHandled == true)
                throw new InvalidOperationException("command expired.");
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            this.isHandled = true;
            this.action(exception);
        }
    }
}
