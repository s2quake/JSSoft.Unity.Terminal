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

using UnityEngine;
using Ntreev.Library.Commands;
using JSSoft.Terminal.Commands;
using System.Linq;
using JSSoft.Terminal;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace JSSoft.Terminal.Commands
{
    [RequireComponent(typeof(TerminalBase))]
    [DisallowMultipleComponent]
    public class CommandContextHost : MonoBehaviour, ICommandCompletor, IServiceProvider
    {
        private CommandContext commandContext;
        private TerminalBase terminal;
        private TerminalGridBase grid;
        [TextArea(5, 10)]
        [SerializeField]
        private string text = "type 'help' to help.";
        [SerializeField]
        private bool isTest = false;

        public string Text
        {
            get => this.text;
            set => this.text = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TerminalBase Terminal => this.terminal;

        public TerminalGridBase Grid => this.grid;

        protected virtual void Start()
        {
            var query = from item in this.CollectCommands()
                        where this.isTest == true ||
                              Attribute.GetCustomAttribute(item.GetType(), typeof(TestCommandAttribute)) is null
                        select item;
            this.commandContext = new CommandContext(query.ToArray(), Enumerable.Empty<ICommandProvider>());
            this.commandContext.Out = new CommandWriter(this.terminal);
        }

        protected virtual void OnEnable()
        {
            this.grid = this.GetComponent<TerminalGridBase>();
            this.terminal = this.GetComponent<TerminalBase>();
            this.terminal.CommandCompletor = this;
            this.terminal.ResetOutput();
            if (this.text != string.Empty)
                this.terminal.AppendLine(this.text);
            this.terminal.CursorPosition = 0;
            this.terminal.Executing += Terminal_Executing;
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual IEnumerable<ICommand> CollectCommands()
        {
            yield return new ResetCommand(this.terminal);
            yield return new ExitCommand(this.terminal);
            yield return new VerboseCommand(this.terminal);
            yield return new ResolutionCommand(this.terminal);
            yield return new StyleCommand(this.terminal);

            // yield return new TestCommand(this);
            // yield return new WidthCommand();
            // yield return new HeightCommand();
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            return this.commandContext.GetCompletion(items, find);
        }

        protected virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITerminal))
                return this.terminal;
            else if (serviceType == typeof(ITerminalGrid))
                return this.grid;
            else if (serviceType == typeof(TerminalDispatcher))
                return this.terminal.Dispatcher;
            return null;
        }

        private async void Terminal_Executing(object sender, TerminalExecuteEventArgs e)
        {
            if (this.terminal.Dispatcher != null)
            {
                await this.RunAsync(e);
            }
            else
            {
                this.terminal.AppendLine("dispatcher is null.");
            }
        }

        private async Task RunAsync(TerminalExecuteEventArgs e)
        {
            try
            {
                await Task.Run(() => commandContext.Execute(commandContext.Name + " " + e.Command));
                e.Success();
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                this.AppendException(ex);
                e.Fail(ex);
            }
            catch (Exception ex)
            {
                this.AppendException(ex);
                e.Fail(ex);
            }
        }

        private void AppendException(Exception e)
        {
            var message = GetMessage();
            this.terminal.AppendLine(message);
            Debug.LogError(message);

            string GetMessage()
            {
                if (this.terminal.IsVerbose == true)
                {
                    return $"{e}";
                }
                else
                {
                    if (e.InnerException != null)
                        return e.InnerException.Message;
                    else
                        return e.Message;
                }
            }
        }

        #region ICommandCompletor

        string[] ICommandCompletor.Complete(string[] items, string find)
        {
            return this.GetCompletion(items, find);
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(Type serviceType)
        {
            return this.GetService(serviceType);
        }

        #endregion
    }
}
