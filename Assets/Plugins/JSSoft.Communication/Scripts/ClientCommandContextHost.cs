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
using System.Collections.Generic;
using Ntreev.Library.Commands;
using JSSoft.Communication.Commands;

namespace JSSoft.Communication
{
    public class ClientCommandContextHost : CommandContextHost
    {
        [SerializeField]
        private ClientContextHost clientContext;

        protected override IEnumerable<ICommand> CollectCommands()
        {
            foreach (var item in base.CollectCommands())
            {
                yield return item;
            }
            yield return new CloseCommand(this.clientContext);
            yield return new DataCommand(this.clientContext);
            yield return new LoginCommand(this.clientContext);
            yield return new LogoutCommand(this.clientContext);
            yield return new OpenCommand(this.clientContext);
            yield return new UserCommand(this.clientContext);
        }
    }
}