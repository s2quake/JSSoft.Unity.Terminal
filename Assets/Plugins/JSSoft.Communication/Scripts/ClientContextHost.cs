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
using JSSoft.Communication.Services;
using System.Text.RegularExpressions;

namespace JSSoft.Communication
{
    public class ClientContextHost : MonoBehaviour, IPromptDrawer
    {
        [SerializeField]
        private TerminalBase terminal;
        // [SerializeField]
        // private TerminalGridBase grid = null;
        [SerializeField]
        private bool adjustResolution = false;
        private readonly Settings settings;
        private readonly UserService userService;
        private readonly DataService dataService;
        private readonly ClientContext serviceContext;
        private readonly INotifyUserService userServiceNotification;
        private bool isDisposed;
        private bool isServer = false;
        private Dispatcher dispatcher;

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
#if UNITY_STANDALONE
            Screen.SetResolution(1050, 660, false);
#endif
        }

        static ClientContextHost()
        {
            JSSoft.Communication.Logging.LogUtility.Logger = new DebugLogger();
        }

        public ClientContextHost()
        {
            this.settings = Settings.CreateFromCommandLine();
            this.userService = new UserService();
            this.dataService = new DataService();
            this.serviceContext = new ClientContext(new UserServiceHost(this.userService), new DataServiceHost(this.dataService));
            this.userServiceNotification = this.userService;
        }

        // [Inject]
        // void Construct(IShell shell, CommandContext commandContext, ITerminal terminal)
        // {
        //     this.shell = shell;
        //     this.terminal = terminal;
        //     this.terminal.Disabled += Terminal_Disabled;
        // }

        public void Awake()
        {

            this.serviceContext.Opened += ServiceContext_Opened;
            this.serviceContext.Closed += ServiceContext_Closed;
            this.userServiceNotification.LoggedIn += UserServiceNotification_LoggedIn;
            this.userServiceNotification.LoggedOut += UserServiceNotification_LoggedOut;
            this.userServiceNotification.MessageReceived += userServiceNotification_MessageReceived;
            // this.terminal = terminal;
            this.dispatcher = Dispatcher.Current;
            this.terminal.PromptDrawer = this;
            // this.terminal.CommandCompletor = this;
            this.terminal.Prompt = ">";
            // this.Title = "Server";
        }

        public void Start()
        {
            //             if (this.terminal != null)
            //             {
            //             }
            // #if UNITY_STANDALONE
            //             if (this.grid != null && this.adjustResolution == true)
            //             {
            //                 var rectangle = this.grid.Rectangle;
            //                 Screen.SetResolution((int)rectangle.width, (int)rectangle.height, false, 0);
            //             }
            // #endif // UNITY_STANDALONE
        }

        public void OnEnable()
        {
            // TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
        }

        public void OnDisable()
        {
            // TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
        }

        public async void OnDestroy()
        {
            if (this.serviceContext.IsOpened == true)
            {
                this.serviceContext.Closed -= ServiceContext_Closed;
                await this.serviceContext.CloseAsync(this.Token);
            }
        }

        private void Terminal_Disabled(object sender, EventArgs e)
        {
            this.terminal = null;
        }

        //         private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //         {
        //             if (object.Equals(sender, this.grid) == true)
        //             {
        // #if !UNITY_EDITOR
        //                 switch (e.PropertyName)
        //                 {
        //                     case nameof(ITerminalGrid.Style):
        //                     case nameof(ITerminalGrid.BufferWidth):
        //                     case nameof(ITerminalGrid.BufferHeight):
        //                         {
        //                             if (this.adjustResolution == true)
        //                             {
        //                                 var rectangle = this.grid.Rectangle;
        //                                 Screen.SetResolution((int)rectangle.width, (int)rectangle.height, false, 0);
        //                             }
        //                         }
        //                         break;
        //                 }
        // #endif
        //             }
        //         }

        public bool IsOpened { get; private set; }

        public IUserService UserService => this.userService;

        public IDataService DataService => this.dataService;

        public string Title { get; private set; }

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
                this.terminal.AppendLine("type 'help user' to execute user command.");
                this.terminal.AppendLine();
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

        private void UpdatePrompt()
        {
            this.dispatcher.VerifyAccess();
            if (this.IsOpened == true)
            {
                if (this.UserID != string.Empty)
                    this.terminal.Prompt = $"{this.serviceContext.Host}:{this.serviceContext.Port}@{this.UserID}>";
                else
                    this.terminal.Prompt = $"{this.serviceContext.Host}:{this.serviceContext.Port}>";
            }
            else
            {
                this.terminal.Prompt = ">";
            }
        }

        private async void ServiceContext_Opened(object sender, EventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = true;
                this.UpdatePrompt();
                if (this.isServer)
                {
                    this.Title = $"Server {this.serviceContext.Host}:{this.serviceContext.Port}";
                    this.terminal.AppendLine("Server has started.");
                }
                else
                {
                    this.terminal.ForegroundColor = TerminalColor.Red;
                    this.terminal.BackgroundColor = TerminalColor.Blue;
                    this.Title = $"Client {this.serviceContext.Host}:{this.serviceContext.Port}";
                    this.terminal.AppendLine("Server is connected.");
                    this.terminal.ResetColor();
                }
                this.terminal.AppendLine("type 'help' to show available commands.");
                this.terminal.AppendLine();
                this.terminal.AppendLine("type 'login admin admin' to login.");
                this.terminal.AppendLine();
            });
        }

        private async void ServiceContext_Closed(object sender, EventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.IsOpened = false;
                this.UpdatePrompt();
                this.Title = $"Client - Disconnected";
                this.terminal.AppendLine("Server is disconnected.");
                this.terminal.AppendLine("type 'open' to connect server.");
                this.terminal.AppendLine();
            });
        }

        private async void UserServiceNotification_LoggedIn(object sender, UserEventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.terminal.AppendLine($"User logged in: {e.UserID}");
            });
        }

        private async void UserServiceNotification_LoggedOut(object sender, UserEventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                this.terminal.AppendLine($"User logged out: {e.UserID}");
            });
        }

        private async void userServiceNotification_MessageReceived(object sender, UserMessageEventArgs e)
        {
            await this.dispatcher.InvokeAsync(() =>
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

        // #region IServiceProvider

        // object IServiceProvider.GetService(Type serviceType)
        // {
        //     if (serviceType == typeof(IServiceProvider))
        //         return this;
        //     throw new Exception();
        //     // return Container.GetService(serviceType);
        // }

        // #endregion

        #region IShell

        public async Task OpenAsync(string host, int port)
        {
            this.serviceContext.Host = host;
            this.serviceContext.Port = port;
            this.Token = await this.serviceContext.OpenAsync();
        }

        public async Task CloseAsync()
        {
            if (this.serviceContext.IsOpened == true)
            {
                this.serviceContext.Closed -= ServiceContext_Closed;
                await this.serviceContext.CloseAsync(this.Token);
            }
        }

        #endregion

        #region IPromptDrawer

        void IPromptDrawer.Draw(string command, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
            this.OnDrawPrompt(command, foregroundColors, backgroundColors);
        }

        #endregion

        // #region ICommandCompletor

        // string[] ICommandCompletor.Complete(string[] items, string find)
        // {
        //     return this.CommandContext.GetCompletion(items, find);
        // }

        // #endregion
    }
}
