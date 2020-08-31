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

using JSSoft.Library.Threading;
using JSSoft.Terminal;
using UnityEngine;

namespace JSSoft.Communication.Services
{
    [RequireComponent(typeof(TerminalBase))]
    class TerminalLogReceiver : MonoBehaviour
    {
        private TerminalBase terminal;
        [SerializeField]
        private LogType logType = LogType.Log;
        [SerializeField]
        private bool useForegroundColor;
        [SerializeField]
        private bool useBackgroundColor;
        [SerializeField]
        private TerminalColor foregroundColor;
        [SerializeField]
        private TerminalColor backgroundColor;
        private Dispatcher dispatcher;

        public TerminalLogReceiver()
        {
        }

        [FieldName(nameof(logType))]
        public LogType LogType
        {
            get => this.logType;
            set
            {
                if (this.logType != value)
                {
                    this.logType = value;
                }
            }
        }

        [FieldName(nameof(useForegroundColor))]
        public bool UseForegroundColor
        {
            get => this.useForegroundColor;
            set
            {
                if (this.useForegroundColor != value)
                {
                    this.useForegroundColor = value;
                }
            }
        }

        [FieldName(nameof(useBackgroundColor))]
        public bool UseBackgroundColor
        {
            get => this.useBackgroundColor;
            set
            {
                if (this.useBackgroundColor != value)
                {
                    this.useBackgroundColor = value;
                }
            }
        }

        [FieldName(nameof(foregroundColor))]
        public TerminalColor ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                if (this.foregroundColor != value)
                {
                    this.foregroundColor = value;
                }
            }
        }

        [FieldName(nameof(backgroundColor))]
        public TerminalColor BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                if (this.backgroundColor != value)
                {
                    this.backgroundColor = value;
                }
            }
        }

        protected virtual void OnEnable()
        {
            this.dispatcher = Dispatcher.Current;
            this.terminal = this.GetComponent<TerminalBase>();
            Application.logMessageReceivedThreaded += Application_LogMessageReceivedThreaded;
        }

        protected virtual void OnDisable()
        {
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
            if (this.dispatcher.CheckAccess() == true)
                this.SendMessage(condition, stackTrace, type);
            else
                await this.dispatcher.InvokeAsync(() => this.SendMessage(condition, stackTrace, type));
        }

        private void SendMessage(string condition, string stackTrace, LogType type)
        {
            if (this.logType == type)
            {
                var foregroundColor = this.terminal.ForegroundColor;
                var backgroundColor = this.terminal.BackgroundColor;
                if (this.useForegroundColor == true)
                    this.terminal.ForegroundColor = this.foregroundColor;
                if (this.useBackgroundColor == true)
                    this.terminal.BackgroundColor = this.backgroundColor;
                this.terminal.AppendLine(condition);
                this.terminal.ForegroundColor = foregroundColor;
                this.terminal.BackgroundColor = backgroundColor;
            }
        }
    }
}