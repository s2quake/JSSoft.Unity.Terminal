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

using UnityEngine;
using JSSoft.Library.Commands;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandContextHost : TerminalHostBase, ICommandCompletor
    {
        private CommandContext commandContext;
        [TextArea(5, 10)]
        [SerializeField]
        private string text = "type 'help' to help.";
        [SerializeField]
        private bool isTest = false;
        private TextWriter consoleWriter;
        private ICommandProvider commandProvider;
        private ICommandConfigurationProvider configurationProvider;

        public string Text
        {
            get => this.text;
            set => this.text = value ?? throw new ArgumentNullException(nameof(value));
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
                        where this.isTest == true ||
                              Attribute.GetCustomAttribute(item.GetType(), typeof(TestCommandAttribute)) is null
                        select item;
            this.commandContext = new CommandContext(this.Terminal, query.ToArray());
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
            Console.SetOut(this.consoleWriter);
            base.OnDisable();
        }

        protected override void OnException(Exception e, string message)
        {
            base.OnException(e, message);
            UnityEngine.Debug.LogError(message);
        }

        protected virtual IEnumerable<ICommand> CollectCommands()
        {
            foreach (var item in (this.CommandProvider ?? new CommandProvider()).Provide(this.Terminal))
            {
                yield return item;
            }
            yield return new ConfigCommand(this.Terminal, this.ConfigurationProvider ?? new CommandConfigurationProvider());
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            return this.commandContext.GetCompletion(items, find);
        }

        protected override void OnRun(string command)
        {
            this.commandContext.Execute(this.commandContext.Name, command);
        }

        protected override Task OnRunAsync(string command)
        {
            return this.commandContext.ExecuteAsync(this.commandContext.Name, command);
        }

        #region ICommandCompletor

        string[] ICommandCompletor.Complete(string[] items, string find)
        {
            return this.GetCompletion(items, find);
        }

        #endregion

        [System.Serializable]
        public class CollectCommandsEvent : UnityEvent<int>
        {
        }
    }
}
