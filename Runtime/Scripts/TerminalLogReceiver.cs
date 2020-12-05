////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text.RegularExpressions;
using JSSoft.Library.Threading;
using JSSoft.Unity.Terminal;
using UnityEngine;

namespace JSSoft.Unity.Terminal
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
        [SerializeField]
        private string pattern = string.Empty;
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

        [FieldName(nameof(pattern))]
        public string Pattern
        {
            get => this.pattern;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.pattern != value)
                {
                    this.pattern = value;
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

        protected virtual bool VerifyPattern(string message, string pattern)
        {
            return Regex.IsMatch(message, pattern);
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
            if (this.logType != type)
                return;
            if (this.pattern != string.Empty && this.VerifyPattern(condition, this.pattern) == false)
                return;

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