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
using JSSoft.Communication.Services.Commands;
using JSSoft.Communication.Services.Commands.Client;

namespace JSSoft.Communication.Services
{
    [RequireComponent(typeof(ClientContextHost))]
    public class ClientCommandContextHost : CommandContextHost
    {
        private ClientContextHost clientContext;

        protected override void Awake()
        {
            this.clientContext = this.GetComponent<ClientContextHost>();
            base.Awake();
        }

        protected override IEnumerable<ICommand> CollectCommands()
        {
            foreach (var item in base.CollectCommands())
            {
                if (item is JSSoft.Terminal.Commands.ExitCommand)
                {
                    yield return new JSSoft.Communication.Services.Commands.ExitCommand(this.clientContext);
                    continue;
                }
                yield return item;
            }
            yield return new OpenCommand(this.clientContext);
            yield return new CloseCommand(this.clientContext);
            yield return new DataCommand(this.clientContext);
            yield return new LoginCommand(this.clientContext);
            yield return new LogoutCommand(this.clientContext);
            yield return new UserCommand(this.clientContext);
        }
    }
}