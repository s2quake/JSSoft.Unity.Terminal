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
using System.Text.RegularExpressions;

namespace JSSoft.Communication.Services
{
    public abstract class ServiceContextHost : MonoBehaviour, IPromptDrawer
    {
        [SerializeField]
        protected TerminalBase terminal;

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
#if UNITY_STANDALONE_OSX
            Screen.SetResolution(2304, 1440, true);
#elif UNITY_STANDALONE_WIN
            Screen.SetResolution(3840, 2160, true);
#endif
        }

        static ServiceContextHost()
        {
            // JSSoft.Communication.Logging.LogUtility.Logger = new DebugLogger();
        }

        public abstract IUserService UserService { get; }

        public abstract IDataService DataService { get; }

        public abstract IServiceContext ServiceContext { get; }

        public abstract INotifyUserService UserServiceNotification { get; }

        public Guid Token { get; protected set; }

        public Guid UserToken { get; protected set; }

        public string UserID { get; protected set; } = string.Empty;

        public bool IsOpened { get; protected set; }

        public string Title { get; private set; }

        public abstract bool IsServer { get; }

        public TerminalDispatcher Dispatcher => this.terminal.Dispatcher;

        public ITerminal Terminal => this.terminal;

        public EventHandler Opened;

        public EventHandler Closed;

        internal async Task LoginAsync(string userID, string password)
        {
            var token = await this.UserService.LoginAsync(userID, password);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.UserID = userID;
                this.UserToken = token;
                this.UpdatePrompt();
                this.terminal.AppendLine($"User logged in: {userID}");
                this.terminal.AppendLine("type 'help user' to execute user command.");
                this.terminal.AppendLine();
            });
        }

        internal async Task LogoutAsync()
        {
            var userID = this.UserID;
            await this.UserService.LogoutAsync(this.UserToken);
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.UserID = string.Empty;
                this.UserToken = Guid.Empty;
                this.UpdatePrompt();
                this.terminal.AppendLine($"User logged out: {userID}");
            });
        }

        protected virtual void Awake()
        {
            this.ServiceContext.Opened += ServiceContext_Opened;
            this.ServiceContext.Closed += ServiceContext_Closed;
            // this.UserServiceNotification.LoggedIn += UserServiceNotification_LoggedIn;
            // this.UserServiceNotification.LoggedOut += UserServiceNotification_LoggedOut;
            this.UserServiceNotification.MessageReceived += userServiceNotification_MessageReceived;
            this.terminal.PromptDrawer = this;
            this.terminal.Prompt = ">";
        }

        protected virtual async void OnDestroy()
        {
            if (this.ServiceContext.IsOpened == true)
            {
                this.ServiceContext.Closed -= ServiceContext_Closed;
                await this.ServiceContext.CloseAsync(this.Token);
            }
        }

        protected virtual void OnEnable()
        {
            ServiceContextHostEvents.Register(this);
        }

        protected virtual void OnDisable()
        {
            ServiceContextHostEvents.Unregister(this);
        }

        protected virtual void OnOpened(EventArgs e)
        {
            this.Opened?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        private void UpdatePrompt()
        {
            this.Dispatcher.VerifyAccess();
            if (this.IsOpened == true)
            {
                if (this.UserID != string.Empty)
                    this.terminal.Prompt = $"{this.ServiceContext.Host}:{this.ServiceContext.Port}@{this.UserID}>";
                else
                    this.terminal.Prompt = $"{this.ServiceContext.Host}:{this.ServiceContext.Port}>";
            }
            else
            {
                this.terminal.Prompt = ">";
            }
        }

        private void OnDrawPrompt(string prompt, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
            if (this.UserID != string.Empty)
            {
                var pattern = $"(.+@)(.+)>";
                var match = Regex.Match(prompt, pattern);
                var p1 = prompt.TrimStart();
                var p2 = prompt.TrimEnd();
                var prefix = prompt.Substring(p1.Length);
                var postfix = prompt.Substring(p2.Length);
                var group = match.Groups[2];
                for (var i = 0; i < group.Length; i++)
                {
                    foregroundColors[group.Index + i] = TerminalColor.Green;
                }
            }
        }

        private async void ServiceContext_Opened(object sender, EventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = true;
                this.UpdatePrompt();
                // if (this.IsServer)
                // {
                //     this.Title = $"Server {this.ServiceContext.Host}:{this.ServiceContext.Port}";
                //     this.terminal.AppendLine("Server has started.");
                // }
                // else
                // {
                //     this.terminal.ForegroundColor = TerminalColor.Red;
                //     this.terminal.BackgroundColor = TerminalColor.Blue;
                //     this.Title = $"Client {this.ServiceContext.Host}:{this.ServiceContext.Port}";
                //     this.terminal.AppendLine("Server is connected.");
                //     this.terminal.ResetColor();
                // }
                // this.terminal.AppendLine("type 'help' to show available commands.");
                // this.terminal.AppendLine();
                // this.terminal.AppendLine("type 'login admin admin' to login.");
                // this.terminal.AppendLine();
                this.OnOpened(EventArgs.Empty);
            });
        }

        private async void ServiceContext_Closed(object sender, EventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = false;
                this.UpdatePrompt();
                this.Title = $"Client - Disconnected";
                // this.terminal.AppendLine("Server is disconnected.");
                // this.terminal.AppendLine("type 'open' to connect server.");
                // this.terminal.AppendLine();
                this.OnClosed(EventArgs.Empty);
            });
        }

        private async void UserServiceNotification_LoggedIn(object sender, UserEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.terminal.AppendLine($"User logged in: {e.UserID}");
            });
        }

        private async void UserServiceNotification_LoggedOut(object sender, UserEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.terminal.AppendLine($"User logged out: {e.UserID}");
            });
        }

        private async void userServiceNotification_MessageReceived(object sender, UserMessageEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (e.Sender == this.UserID)
                {
                    this.terminal.ForegroundColor = TerminalColor.Magenta;
                    this.terminal.AppendLine($"to '{e.Receiver}': {e.Message}");
                    this.terminal.ResetColor();
                }
                else if (e.Receiver == this.UserID)
                {
                    this.terminal.ForegroundColor = TerminalColor.Magenta;
                    this.terminal.AppendLine($"from '{e.Receiver}': {e.Message}");
                    this.terminal.ResetColor();
                }
            });
        }

        internal async Task ExitAsync()
        {
            if (this.ServiceContext.IsOpened == true)
            {
                this.ServiceContext.Closed -= ServiceContext_Closed;
                await this.ServiceContext.CloseAsync(this.Token);
            }
        }

        #region IPromptDrawer

        void IPromptDrawer.Draw(string command, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
            this.OnDrawPrompt(command, foregroundColors, backgroundColors);
        }

        #endregion
    }
}
