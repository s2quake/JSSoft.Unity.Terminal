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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using JSSoft.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using Ntreev.Library.Threading;

namespace JSSoft.Communication.Shells
{
    public class ClientBehaviour : MonoBehaviour
    {
        private IShell shell;
        private CommandContext commandContext;
        private CommandWriter writer;
        private DispatcherScheduler scheduler;
        private ITerminal terminal;

        static ClientBehaviour()
        {
            JSSoft.Communication.Logging.LogUtility.Logger = new DebugLogger();
        }

        public void Awake()
        {
            this.scheduler = DispatcherScheduler.Current;
            this.shell = Container.GetService<IShell>();
            this.commandContext = Container.GetService<CommandContext>();
            this.terminal = Container.GetService<Terminal>();
        }

        public async Task Start()
        {
            if (this.terminal != null)
            {
                this.terminal.Executed += Terminal_Executed;
                this.writer = new CommandWriter(this.terminal);
                this.terminal.onCompletion = this.commandContext.GetCompletion;
                this.commandContext.Out = this.writer;
                this.terminal.AppendLine($"type 'open' to connect server.");
                this.terminal.AppendLine($"example: open --host [address]");
                this.terminal.Focus();
                this.terminal.CursorPosition = 0;
            }
        }

        private async Task TestAsync()
        {
            Debug.Log("TestAsync 1");
            await Task.Delay(1000);
            Debug.Log("TestAsync 2");
            await Task.Delay(1000);
            Debug.Log("TestAsync 3");
        }

        public void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying && this.scheduler != null)
#endif
            {
                this.scheduler.ProcessAll(1000 / 60);
            }
        }

        public async void OnDestroy()
        {
            // Debug.Log("OnDestroy 1");
            if (this.shell != null)
            {
                await this.shell.StopAsync();
                // Debug.Log("OnDestroy 2");
                this.shell.Dispose();
                // Debug.Log("OnDestroy 3");
                // await TestAsync();
            }
        }

        private async void Terminal_Executed(object sender, TerminalExecuteEventArgs e)
        {
            await this.RunAsync(e.Command);
            e.Handled = true;
        }

        private async Task RunAsync(string commandLine)
        {
            try
            {
                await Task.Run(() => this.commandContext.Execute(this.commandContext.Name + " " + commandLine));
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                if (e.InnerException != null)
                    this.terminal.AppendLine(e.InnerException.Message);
                else
                    this.terminal.AppendLine(e.Message);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    this.terminal.AppendLine($"{e.InnerException}");
                else
                    this.terminal.AppendLine($"{e}");
            }
        }
    }
}
