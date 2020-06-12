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

namespace JSSoft.Communication.Services
{
    [RequireComponent(typeof(ClientContextHost))]
    public class ClientBehaviour : MonoBehaviour
    {
        private static readonly Queue<string> users = new Queue<string>();
        private ServerContextHost serverContextHost;
        private ClientContextHost clientContextHost;
        private string userID = string.Empty;

        static ClientBehaviour()
        {
            for (var i = 0; i < 100; i++)
            {
                users.Enqueue($"user{i}");
            }
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
        }

        protected virtual void OnDisable()
        {
            ServiceContextHostEvents.Opened -= ServiceContextHost_Opened;
        }

        private async void ServiceContextHost_Opened(object sender, EventArgs e)
        {
            if (sender is ServerContextHost serverContext)
            {
                this.clientContextHost = this.GetComponent<ClientContextHost>();
                await this.clientContextHost.OpenAsync("localhost", 4004);
                await this.clientContextHost.LoginAsync(this.userID, "1234");
            }
        }
    }
}
