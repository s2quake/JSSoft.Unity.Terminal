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
using JSSoft.Terminal.Commands;
using System.Collections.Generic;
using JSSoft.Library.Commands;
using System.Threading.Tasks;
using System;
using System.Threading;
using JSSoft.Library.Random;
using JSSoft.Terminal;

namespace JSSoft.Communication.Services
{
    [RequireComponent(typeof(ClientContextHost))]
    public class ClientBehaviour : MonoBehaviour
    {
        private static readonly List<string> unusedUsers;
        private static readonly List<string> usedUsers;
        private ServerContextHost serverContextHost;
        private ClientContextHost clientContextHost;
        private IUserService userService;
        private string userID = string.Empty;
        private Timer timer;

        static ClientBehaviour()
        {
            unusedUsers = new List<string>(Server.UserService.MaxUserCount);
            for (var i = 0; i < unusedUsers.Capacity; i++)
            {
                unusedUsers.Add($"user{i}");
            }
            usedUsers = new List<string>(Server.UserService.MaxUserCount);
        }

        public ClientBehaviour()
        {
        }

        protected virtual void Awake()
        {
            var userID = unusedUsers.Random();
            unusedUsers.Remove(userID);
            usedUsers.Add(userID);
            this.userID = userID;
        }

        protected virtual void OnDestroy()
        {
            usedUsers.Remove(this.userID);
            unusedUsers.Add(this.userID);
            this.userID = string.Empty;
        }

        protected virtual void OnEnable()
        {
            ServiceContextHostEvents.Opened += ServiceContextHost_Opened;
            ServiceContextHostEvents.Closed += ServiceContextHost_Closed;
        }

        protected virtual void OnDisable()
        {
            this.timer?.Dispose();
            this.timer = null;
            ServiceContextHostEvents.Opened -= ServiceContextHost_Opened;
            ServiceContextHostEvents.Closed -= ServiceContextHost_Closed;
        }

        private async void ServiceContextHost_Opened(object sender, EventArgs e)
        {
            if (sender is ServerContextHost serverContext)
            {
                this.clientContextHost = this.GetComponent<ClientContextHost>();
                await this.clientContextHost.OpenAsync("localhost", 4004);
                await this.clientContextHost.LoginAsync(this.userID, "1234");
                this.userService = this.clientContextHost.UserService;
                this.timer = new Timer(Timer_Elapsed, null, RandomUtility.Next(1000, 2000), RandomUtility.Next(100, 1000));
                // Debug.Log("timer created");
            }
        }

        private void ServiceContextHost_Closed(object sender, EventArgs e)
        {
            if (sender is ServerContextHost serverContext)
            {
                this.userService = null;
                this.timer?.Dispose();
                this.timer = null;
            }
        }

        private async void Timer_Elapsed(object state)
        {
            // await this.clientContextHost.Terminal.Dispatcher.InvokeAsync(() =>
            // {
            //     this.clientContextHost.Terminal.AppendLine(RandomUtility.NextString());
            // });
            // await this.clientContextHost.Dispatcher.InvokeAsync(()=>
            // {
            //     this.clientContextHost.Terminal.Command = "help";
            //     this.clientContextHost.Terminal.Execute();
            // });
            var userID = usedUsers.RandomOrDefault(item => this.userID != item);
            if (userID != null)
            {
                var token = this.clientContextHost.UserToken;
                if (await this.userService.IsOnlineAsync(token, userID) == true)
                {
                    await this.userService.SendMessageAsync(token, userID, RandomUtility.NextString());
                }
            }
        }
    }
}
