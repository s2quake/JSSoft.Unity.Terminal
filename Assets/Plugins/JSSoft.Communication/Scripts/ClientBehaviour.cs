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
using System.Threading.Tasks;
using JSSoft.Terminal;
using UnityEngine;
using Ntreev.Library.Threading;
using Zenject;
using System.ComponentModel;
using JSSoft.Terminal.Commands;

namespace JSSoft.Communication.Shells
{
    public class ClientBehaviour : MonoBehaviour
    {
        private IShell shell;
        private CommandContext commandContext;
        private CommandWriter writer;
        private DispatcherScheduler scheduler;
        private ITerminal terminal;
        [SerializeField]
        private TerminalGridBase grid = null;
        [SerializeField]
        private bool adjustResolution = false;

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
#if UNITY_STANDALONE
            Screen.SetResolution(1050, 660, false);
#endif
        }

        static ClientBehaviour()
        {
            JSSoft.Communication.Logging.LogUtility.Logger = new DebugLogger();
        }

        [Inject]
        void Construct(IShell shell, CommandContext commandContext, ITerminal terminal)
        {
            this.shell = shell;
            this.commandContext = commandContext;
            this.terminal = terminal;
            this.terminal.Disabled += Terminal_Disabled;
        }

        public void Awake()
        {
            this.scheduler = DispatcherScheduler.Current;
        }

        public void Start()
        {
            if (this.terminal != null)
            {
                this.terminal.Executing += Terminal_Executing;
                this.writer = new CommandWriter(this.terminal);
                this.commandContext.Out = this.writer;
                this.terminal.Reset();
                this.terminal.AppendLine($"type 'open' to connect server.");
                this.terminal.AppendLine($"example: open --host [address]");
                this.terminal.CursorPosition = 0;
            }
#if UNITY_STANDALONE
            if (this.grid != null && this.adjustResolution == true)
            {
                var rectangle = this.grid.Rectangle;
                Screen.SetResolution((int)rectangle.width, (int)rectangle.height, false, 0);
            }
#endif // UNITY_STANDALONE
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

        public void OnEnable()
        {
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
        }

        public void OnDisable()
        {
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
        }

        public async void OnDestroy()
        {
            if (this.shell != null)
            {
                await this.shell.StopAsync();
                this.shell.Dispose();
            }
            if (this.writer != null)
            {
                this.writer.Dispose();
            }
        }

        private async void Terminal_Executing(object sender, TerminalExecuteEventArgs e)
        {
            await this.RunAsync(sender as ITerminal, e);
        }

        private async Task RunAsync(ITerminal terminal, TerminalExecuteEventArgs e)
        {
            try
            {
                await Task.Run(() => this.commandContext.Execute(terminal.GameObject, this.commandContext.Name + " " + e.Command));
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

        private void Terminal_Disabled(object sender, EventArgs e)
        {
            this.terminal = null;
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (object.Equals(sender, this.grid) == true)
            {
#if !UNITY_EDITOR
                switch (e.PropertyName)
                {
                    case nameof(ITerminalGrid.Style):
                    case nameof(ITerminalGrid.BufferWidth):
                    case nameof(ITerminalGrid.BufferHeight):
                        {
                            if (this.adjustResolution == true)
                            {
                                var rectangle = this.grid.Rectangle;
                                Screen.SetResolution((int)rectangle.width, (int)rectangle.height, false, 0);
                            }
                        }
                        break;
                }
#endif
            }
        }
    }
}
