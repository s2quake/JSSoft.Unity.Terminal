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
using JSSoft.Terminal.Commands;
using System.Collections.Generic;
using Ntreev.Library.Commands;
using System.Threading.Tasks;
using System;
using System.Timers;

namespace JSSoft.Communication.Services
{
    [RequireComponent(typeof(ClientContextHost))]
    public class ClientBehaviour : MonoBehaviour
    {
        private static readonly Queue<string> users = new Queue<string>();
        private ServerContextHost serverContextHost;
        private ClientContextHost clientContextHost;
        private IUserService userService;
        private string userID = string.Empty;
        private Timer timer = new Timer(1000);

        static ClientBehaviour()
        {
            for (var i = 0; i < 100; i++)
            {
                users.Enqueue($"user{i}");
            }
        }

        public ClientBehaviour()
        {

        }

        protected virtual void Awake()
        {
            this.userID = users.Dequeue();
        }

        protected virtual void OnDestroy()
        {
            users.Enqueue(this.userID);
            this.userID = string.Empty;
        }

        protected virtual void OnEnable()
        {
            ServiceContextHostEvents.Opened += ServiceContextHost_Opened;
            ServiceContextHostEvents.Closed += ServiceContextHost_Closed;
        }

        protected virtual void OnDisable()
        {
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
                this.timer.Elapsed += Timer_Elapsed;
                this.timer.Start();
            }
        }

        private void ServiceContextHost_Closed(object sender, EventArgs e)
        {
            if (sender is ServerContextHost serverContext)
            {
                this.userService = null;
                this.timer.Elapsed -= Timer_Elapsed;
                this.timer.Stop();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var userID = $"user{(int)(UnityEngine.Random.value * 20)}";
            // Debug.Log(userID);
            // var isOnline = false;
            // if (this.userService != null && userID != this.userID)
            // {
            //     isOnline = await this.userService.IsOnlineAsync(this.clientContextHost.UserToken, userID);
            // }

            // if (this.userService != null && isOnline == true)
            // {
            //     await this.userService.SendMessageAsync(this.clientContextHost.UserToken, userID, $"{UnityEngine.Random.value}");
            // }
        }
    }
}
