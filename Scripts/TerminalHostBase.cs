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

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(TerminalBase))]
    [DisallowMultipleComponent]
    public class TerminalHostBase : MonoBehaviour, IServiceProvider
    {
        [SerializeField]
        private bool isAsync = false;
        [SerializeField]
        private bool useErrorForegroundColor = true;
        [SerializeField]
        private bool useErrorBackgroundColor = false;
        [SerializeField]
        private TerminalColor errorForegroundColor = TerminalColor.Red;
        [SerializeField]
        private TerminalColor errorBackgroundColor = TerminalColor.Black;

        private TerminalBase terminal;
        private TerminalGridBase grid;

        public TerminalBase Terminal => this.terminal;

        public TerminalGridBase Grid => this.grid;

        public bool IsAsync
        {
            get => this.isAsync;
            set => this.isAsync = value;
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            this.grid = this.GetComponent<TerminalGridBase>();
            this.terminal = this.GetComponent<TerminalBase>();
            this.terminal.Executing += Terminal_Executing;
        }

        protected virtual void OnDisable()
        {
            this.terminal.Executing -= Terminal_Executing;
        }

        protected virtual void OnDestroy()
        {
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

        protected virtual void OnRun(string command)
        {
        }

        protected virtual Task OnRunAsync(string command)
        {
            return Task.Delay(1);
        }

        protected virtual void OnException(Exception e, string message)
        {
            if (this.useErrorForegroundColor == true)
                this.terminal.ForegroundColor = this.errorForegroundColor;
            if (this.useErrorBackgroundColor == true)
                this.terminal.BackgroundColor = this.errorBackgroundColor;
            this.terminal.AppendLine(message);
            this.terminal.ResetColor();
        }

        protected virtual string GetExceptionMessage(Exception e)
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


        private void Terminal_Executing(object sender, TerminalExecuteEventArgs e)
        {
            if (this.terminal.Dispatcher != null)
            {
                this.Run(e);
            }
            else
            {
                this.terminal.AppendLine("dispatcher is null.");
            }
        }

        private async void Run(TerminalExecuteEventArgs e)
        {
            try
            {
                if (this.isAsync == true)
                    await this.OnRunAsync(e.Command);
                else
                    this.OnRun(e.Command);
                e.Success();
            }
            catch (Exception ex)
            {
                var message = this.GetExceptionMessage(ex);
                this.OnException(ex, message);
                e.Fail(ex);
            }
        }

        #region IServiceProvider

        object IServiceProvider.GetService(Type serviceType)
        {
            return this.GetService(serviceType);
        }

        #endregion
    }

}
