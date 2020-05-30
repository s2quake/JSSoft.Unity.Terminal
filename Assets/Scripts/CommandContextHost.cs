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

namespace JSSoft.Communication.Shells
{
    [RequireComponent(typeof(TerminalBase))]
    class CommandContextHost : MonoBehaviour
    {
        private CommandContext commandContext;
        private TerminalBase terminal;
        private CommandWriter writer;

        protected virtual void Awake()
        {
            
            var commands = new ICommand[]
            {
                new ResetCommand(),
                new ExitCommand(),
                new VerboseCommand()
            };
            this.commandContext = new CommandContext(commands, Enumerable.Empty<ICommandProvider>());
        }

        protected virtual void OnEnable()
        {
            this.terminal = this.GetComponent<TerminalBase>();
            this.terminal.Executing += Terminal_Executing;
            this.writer = new CommandWriter(this.terminal);
            this.commandContext.Out = this.writer;
            this.terminal.Reset();
            this.terminal.AppendLine($"type 'open' to connect server.");
            this.terminal.AppendLine($"example: open --host [address]");
            this.terminal.CursorPosition = 0;
            Debug.Log(1);
        }

        protected virtual void OnDisable()
        {
            Debug.Log(0);
        }

        private async void Terminal_Executing(object sender, TerminalExecuteEventArgs e)
        {
            if (sender is ITerminal terminal)
            {
                await this.RunAsync(terminal.GameObject, e);
            }
        }

        private async Task RunAsync(GameObject gameObject, TerminalExecuteEventArgs e)
        {
            try
            {
                await Task.Run(() => commandContext.Execute(gameObject, commandContext.Name + " " + e.Command));
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
    }
}