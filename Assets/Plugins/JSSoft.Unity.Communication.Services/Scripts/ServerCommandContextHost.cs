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
using JSSoft.Unity.Terminal.Commands;
using System.Collections.Generic;
using JSSoft.Library.Commands;
using JSSoft.Communication.Services.Commands;
using JSSoft.Communication.Services.Commands.Server;

namespace JSSoft.Communication.Services
{
    [RequireComponent(typeof(ServerContextHost))]
    public class ServerCommandContextHost : CommandContextHost
    {
        private ServerContextHost serverContext;

        protected override void Start()
        {
            this.serverContext = this.GetComponent<ServerContextHost>();
            base.Start();
        }

        protected override IEnumerable<ICommand> CollectCommands()
        {
            foreach (var item in base.CollectCommands())
            {
                if (item is JSSoft.Unity.Terminal.Commands.ExitCommand)
                {
                    yield return new JSSoft.Communication.Services.Commands.ExitCommand(this.serverContext);
                    continue;
                }
                yield return item;
            }
            yield return new OpenCommand(this.serverContext);
            yield return new CloseCommand(this.serverContext);
            yield return new DataCommand(this.serverContext);
            yield return new LoginCommand(this.serverContext);
            yield return new LogoutCommand(this.serverContext);
            yield return new UserCommand(this.serverContext);
        }
    }
}