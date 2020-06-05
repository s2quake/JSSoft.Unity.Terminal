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
using JSSoft.Communication.Services;
using JSSoft.Communication.Services.Server;
using System.Text.RegularExpressions;

namespace JSSoft.Communication.Services
{
    public class ServerContextHost : ContextHostBase
    {
        private readonly UserService userService;
        private readonly DataService dataService;
        private readonly ServerContext serviceContext;
        private readonly INotifyUserService userServiceNotification;

        public ServerContextHost()
        {
            this.userService = new UserService();
            this.dataService = new DataService();
            this.serviceContext = new ServerContext(new UserServiceHost(this.userService), new DataServiceHost(this.dataService));
            this.userServiceNotification = this.userService;
        }

        public override IUserService UserService => this.userService;

        public override IDataService DataService => this.dataService;

        public override IServiceContext ServiceContext => this.serviceContext;

        public override INotifyUserService UserServiceNotification => this.userService;

        public override bool IsServer => true;

        internal async Task OpenAsync()
        {
            this.Token = await this.serviceContext.OpenAsync();
        }

        internal async Task CloseAsync()
        {
            await this.serviceContext.CloseAsync(this.Token);
        }
    }
}
