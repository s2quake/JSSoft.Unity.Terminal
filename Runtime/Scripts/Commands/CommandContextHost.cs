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

using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandContextHost : TerminalHostBase, ICommandCompletor
    {
        [TextArea(5, 10)]
        [SerializeField]
        private string startupText = "Type 'help' for usage.\nType 'version' for version.";

        [TextArea(5, 10)]
        [SerializeField]
        private string baseUsage = "Type 'help' for usage.\nType 'version' for version.";

        private CommandContext commandContext;
        private TextWriter consoleWriter;
        private ICommandProvider commandProvider;
        private ICommandConfigurationProvider configurationProvider;

        [FieldName(nameof(startupText), Order = int.MinValue)]
        public string StartupText
        {
            get => this.startupText;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.startupText != value)
                {
                    this.startupText = value;
                    this.InvokePropertyChangedEvent(nameof(StartupText));
                }
            }
        }

        [FieldName(nameof(baseUsage))]
        public string BaseUsage
        {
            get => this.baseUsage;
            set
            {
                if (baseUsage == null)
                    throw new ArgumentNullException(nameof(baseUsage));
                if (this.baseUsage != value)
                {
                    this.baseUsage = value;
                    this.InvokePropertyChangedEvent(nameof(BaseUsage));
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
            this.commandContext.BaseUsage = PrintBaseUsage;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.Terminal.CommandCompletor = this;
            this.Terminal.ResetOutput();
            if (this.startupText != string.Empty)
                this.Terminal.AppendLine(this.startupText);
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

        private void PrintBaseUsage(CommandContextBase commandContext)
        {
            if (this.baseUsage != string.Empty)
            {
                commandContext.Out.WriteLine(this.baseUsage);
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
