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
using JSSoft.UI;
using UnityEngine;

namespace JSSoft.Communication.Shells
{
    [RequireComponent(typeof(Terminal))]
    class TerminalLogRedirector : MonoBehaviour
    {
        private Terminal terminal;
        [SerializeField]
        private LogType logType;
        [SerializeField]
        private bool useColor;
        [SerializeField]
        private TerminalColor color;
        private Dispatcher dispatcher;

        public TerminalLogRedirector()
        {
        }

        protected virtual void OnEnable()
        {
            this.dispatcher = Dispatcher.Current;
            this.terminal = this.GetComponent<Terminal>();
            Application.logMessageReceived += Application_LogMessageReceived;
            Application.logMessageReceivedThreaded += Application_LogMessageReceivedThreaded;
        }

        protected virtual void OnDisable()
        {
            Application.logMessageReceived -= Application_LogMessageReceived;
            Application.logMessageReceivedThreaded -= Application_LogMessageReceivedThreaded;
            this.dispatcher = null;
            this.terminal = null;
        }

        private void Application_LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            this.SendMessage(condition, stackTrace, type);
        }

        private async void Application_LogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            await this.dispatcher.InvokeAsync(() => this.SendMessage(condition, stackTrace, type));
        }

        private void SendMessage(string condition, string stackTrace, LogType type)
        {
            if (this.logType == type)
            {
                if (this.useColor == true)
                    this.terminal.ForegroundColor = this.color;
                this.terminal.AppendLine(condition);
                this.terminal.ForegroundColor = null;
            }
        }
    }
}