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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JSSoft.Communication.Services;
using JSSoft.Terminal;
using JSSoft.Terminal.Commands;
using Ntreev.Library.Threading;
using UnityEngine;
using Zenject;

namespace JSSoft.Communication
{
    class Shell : IShell, IServiceProvider, IPromptDrawer, ICommandCompletor
    {
        private readonly Settings settings;
        private readonly LazyInject<CommandContext> commandContext;
        private readonly IServiceContext serviceHost;
        private readonly INotifyUserService userServiceNotification;
        private bool isDisposed;
        private bool isServer = false;
        private ITerminal terminal;
        private Dispatcher dispatcher;

        public Shell(LazyInject<CommandContext> commandContext, IServiceContext serviceHost, INotifyUserService userServiceNotification, ITerminal terminal)
        {
            this.settings = Settings.CreateFromCommandLine();
            this.serviceHost = serviceHost;
            this.serviceHost.Opened += ServiceHost_Opened;
            this.serviceHost.Closed += ServiceHost_Closed;
            this.userServiceNotification = userServiceNotification;
            this.userServiceNotification.LoggedIn += UserServiceNotification_LoggedIn;
            this.userServiceNotification.LoggedOut += UserServiceNotification_LoggedOut;
            this.userServiceNotification.MessageReceived += userServiceNotification_MessageReceived;
            this.commandContext = commandContext;
            this.terminal = terminal;
            this.dispatcher = Dispatcher.Current;
            this.terminal.PromptDrawer = this;
            this.terminal.CommandCompletor = this;
            this.terminal.Prompt = ">";
            this.Title = "Server";
        }

        public void Dispose()
        {
            if (this.isDisposed == false)
            {
                this.isDisposed = true;
            }
        }

        public bool IsOpened { get; private set; }

        public string Title
        {
            get => Console.Title;
            set => Console.Title = value;
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

        internal async Task LoginAsync(string userID, Guid token)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.UserID = userID;
                this.UserToken = token;
                this.UpdatePrompt();
                this.Out.WriteLine("type 'help user' to execute user command.");
                this.Out.WriteLine();
            });
        }

        internal async Task LogoutAsync()
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.UserID = string.Empty;
                this.UserToken = Guid.Empty;
                this.UpdatePrompt();
            });
        }

        internal Guid Token { get; set; }

        internal Guid UserToken { get; private set; }

        internal string UserID { get; private set; } = string.Empty;

        private TextWriter Out => this.CommandContext.Out;

        private CommandContext CommandContext => this.commandContext.Value;

        private void UpdatePrompt()
        {
            this.dispatcher.VerifyAccess();
            if (this.IsOpened == true)
            {
                if (this.UserID != string.Empty)
                    this.terminal.Prompt = $"{this.serviceHost.Host}:{this.serviceHost.Port}@{this.UserID}>";
                else
                    this.terminal.Prompt = $"{this.serviceHost.Host}:{this.serviceHost.Port}>";
            }
            else
            {
                this.terminal.Prompt = ">";
            }
        }

        private async void ServiceHost_Opened(object sender, EventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = true;
                this.UpdatePrompt();
                if (this.isServer)
                {
                    this.Title = $"Server {this.serviceHost.Host}:{this.serviceHost.Port}";
                    this.Out.WriteLine("Server has started.");
                }
                else
                {
                    this.terminal.ForegroundColor = TerminalColor.Red;
                    this.terminal.BackgroundColor = TerminalColor.Blue;
                    this.Title = $"Client {this.serviceHost.Host}:{this.serviceHost.Port}";
                    this.Out.WriteLine("Server is connected.");
                    this.terminal.ResetColor();
                }
                this.Out.WriteLine("type 'help' to show available commands.");
                this.Out.WriteLine();
                this.Out.WriteLine("type 'login admin admin' to login.");
                this.Out.WriteLine();
            });
        }

        private async void ServiceHost_Closed(object sender, EventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = false;
                this.UpdatePrompt();
                this.Title = $"Client - Disconnected";
                this.Out.WriteLine("Server is disconnected.");
                this.Out.WriteLine("type 'open' to connect server.");
                this.Out.WriteLine();
            });
        }

        private async void UserServiceNotification_LoggedIn(object sender, UserEventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.Out.WriteLine($"User logged in: {e.UserID}");
            });
        }

        private async void UserServiceNotification_LoggedOut(object sender, UserEventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.Out.WriteLine($"User logged out: {e.UserID}");
            });
        }

        private async void userServiceNotification_MessageReceived(object sender, UserMessageEventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                if (e.Sender == this.UserID)
                {
                    this.terminal.ForegroundColor = TerminalColor.Magenta;
                    this.Out.WriteLine($"to '{e.Receiver}': {e.Message}");
                    this.terminal.ResetColor();
                }
                else if (e.Receiver == this.UserID)
                {
                    this.terminal.ForegroundColor = TerminalColor.Magenta;
                    this.Out.WriteLine($"from '{e.Receiver}': {e.Message}");
                    this.terminal.ResetColor();
                }
            });
        }

        #region IServiceProvider

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;
            throw new Exception();
            // return Container.GetService(serviceType);
        }

        #endregion

        #region IShell

        async Task IShell.StartAsync()
        {
            this.serviceHost.Host = this.settings.Host;
            this.serviceHost.Port = this.settings.Port;
            this.Token = await this.serviceHost.OpenAsync();
        }

        async Task IShell.StopAsync()
        {
            if (this.serviceHost.IsOpened == true)
            {
                this.serviceHost.Closed -= ServiceHost_Closed;
                await this.serviceHost.CloseAsync(this.Token);
            }
        }

        #endregion

        #region IPromptDrawer

        void IPromptDrawer.Draw(string command, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
            this.OnDrawPrompt(command, foregroundColors, backgroundColors);
        }

        #endregion

        #region ICommandCompletor

        string[] ICommandCompletor.Complete(string[] items, string find)
        {
            return this.CommandContext.GetCompletion(items, find);
        }

        #endregion
    }
}