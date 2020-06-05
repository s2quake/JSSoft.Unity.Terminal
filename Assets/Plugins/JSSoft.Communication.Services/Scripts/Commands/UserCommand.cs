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
using Ntreev.Library.Commands;
using System.Threading.Tasks;
using JSSoft.Communication.Services;
using JSSoft.Communication;

namespace JSSoft.Communication.Services.Commands
{
    class UserCommand : CommandMethodBase
    {
        private readonly ContextHostBase context;
        private readonly IUserService userService;

        public UserCommand(ContextHostBase context)
        {
            this.context = context;
            this.userService = context.UserService;
        }

        [CommandMethod]
        public Task CreateAsync(string userID, string password, Authority authority = Authority.Member)
        {
            return this.userService.CreateAsync(this.context.UserToken, userID, password, Authority.Admin);
        }

        [CommandMethod]
        public Task DeleteAsync(string userID)
        {
            return this.userService.DeleteAsync(this.context.UserToken, userID);
        }

        [CommandMethod]
        public Task RenameAsync(string userName)
        {
            return this.userService.RenameAsync(this.context.UserToken, userName);
        }

        [CommandMethod]
        public Task AuthorityAsync(string userID, Authority authority)
        {
            return this.userService.SetAuthorityAsync(this.context.UserToken, userID, authority);
        }

        [CommandMethod]
        public async Task InfoAsync(string userID)
        {
            var (userName, authority) = await this.userService.GetInfoAsync(this.context.UserToken, userID);
            this.Out.WriteLine($"UseName: {userName}");
            this.Out.WriteLine($"Authority: {authority}");
        }

        [CommandMethod]
        public async Task ListAsync()
        {
            var items = await this.userService.GetUsersAsync(this.context.UserToken);
            foreach (var item in items)
            {
                this.Out.WriteLine(item);
            }
        }

        [CommandMethod]
        public Task SendMessageAsync(string userID, string message)
        {
            return this.userService.SendMessageAsync(this.context.UserToken, userID, message);
        }

        public override bool IsEnabled => this.context.UserToken != Guid.Empty;

        protected override bool IsMethodEnabled(CommandMethodDescriptor descriptor)
        {
            return this.context.UserToken != Guid.Empty;
        }
    }
}
