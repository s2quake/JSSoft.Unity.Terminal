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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandContextHost : TerminalHostBase, ICommandCompletor
    {
        [TextArea(5, 10)]
        [SerializeField]
        private string text = "type 'help' to help.";

        private CommandContext commandContext;
        private TextWriter consoleWriter;
        private ICommandProvider commandProvider;
        private ICommandConfigurationProvider configurationProvider;

        [FieldName(nameof(text))]
        public string Text
        {
            get => this.text;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.text != value)
                {
                    this.text = value;
                    this.InvokePropertyChangedEvent(nameof(Text));
                }
            }
        }

        public ICommandProvider CommandProvider
        {
            get => this.commandProvider;
            set => this.commandProvider = value;
        }

        public ICommandConfigurationProvider ConfigurationProvider
        {
            get => this.configurationProvider;
            set => this.configurationProvider = value;
        }

        protected override void Awake()
        {
            base.Awake();
            this.consoleWriter = Console.Out;
        }

        protected override void Start()
        {
            base.Start();
            var query = from item in this.CollectCommands()
                        where Application.isEditor == true ||
                              Attribute.GetCustomAttribute(item.GetType(), typeof(DebugCommandAttribute)) is null
                        select item;
            this.commandContext = new CommandContext(this.Terminal, query.ToArray(), this);
            this.commandContext.Out = new CommandWriter(this.Terminal);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.Terminal.CommandCompletor = this;
            this.Terminal.ResetOutput();
            if (this.text != string.Empty)
                this.Terminal.AppendLine(this.text);
            Console.SetOut(new TerminalTextWriter(this.Terminal));
        }

        protected override void OnDisable()
        {
            if (this.consoleWriter != null)
                Console.SetOut(this.consoleWriter);
            base.OnDisable();
        }

        protected override void OnException(Exception e, string message)
        {
            base.OnException(e, message);
        }

        protected virtual IEnumerable<ICommand> CollectCommands()
        {
            var configurationProvider = this.ConfigurationProvider ?? new CommandConfigurationProvider();
            foreach (var item in (this.CommandProvider ?? new CommandProvider()).Provide(this.Terminal, configurationProvider))
            {
                yield return item;
            }
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            return this.commandContext.GetCompletion(items, find);
        }

        protected override void OnRun(string command)
        {
            var commandInstance = this.commandContext.GetCommand(this.commandContext.Name, command);
            if (commandInstance is IExecutableAsync && this.IsAsync == false)
                throw new InvalidOperationException($"Asynchronous commands are not allowed. If you want to use an asynchronous command, set the IsAsync value to true: '{commandInstance.Name}'");
            this.commandContext.Execute(this.commandContext.Name, command);
        }

        protected override async Task OnRunAsync(string command)
        {
            var cancellation = new CancellationTokenSource();
            this.Terminal.CancellationRequested += Terminal_CancellationRequested;
            try
            {
                await this.commandContext.ExecuteAsync(this.commandContext.Name, command, cancellation.Token);
            }
            finally
            {
                this.Terminal.CancellationRequested -= Terminal_CancellationRequested;
                await this.Terminal.Dispatcher.InvokeAsync(() =>
                {
                    var progressText = this.Terminal.ProgressText;
                    if (progressText != string.Empty)
                    {
                        this.Terminal.Progress(string.Empty, 0);
                        this.Terminal.AppendLine(progressText);
                    }
                });
            }

            void Terminal_CancellationRequested(object sender, EventArgs e)
            {
                if (cancellation.IsCancellationRequested == false)
                    cancellation.Cancel();
            }
        }

        #region ICommandCompletor

        string[] ICommandCompletor.Complete(string[] items, string find)
        {
            return this.GetCompletion(items, find);
        }

        #endregion
    }
}
