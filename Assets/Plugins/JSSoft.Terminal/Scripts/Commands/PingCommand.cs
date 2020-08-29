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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSSoft.Terminal;
using JSSoft.Terminal.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Library.Threading;
using UnityEngine;

namespace JSSoft.Terminal.Commands
{
    public class PingCommand : TerminalCommandAsyncBase
    {
        public PingCommand(ITerminal terminal)
            : base(terminal)
        {

        }

        [CommandPropertyRequired]
        public string Address
        {
            get; set;
        }

        [CommandProperty]
        [DefaultValue(3)]
        public int Count
        {
            get; set;
        }

        protected override async Task OnExecuteAsync()
        {
            var address = this.Address;
            var count = this.Count;
            for (var i = 0; i < count; i++)
            {
                var ping = await this.Terminal.Dispatcher.InvokeAsync(() => new Ping(address));
                do
                {
                    await Task.Delay(1000);
                } while (await this.Terminal.Dispatcher.InvokeAsync(() => ping.isDone) == false);
                await this.Terminal.Dispatcher.InvokeAsync(() =>
                {
                    this.Terminal.AppendLine($"{address}: {ping.time}");
                    ping.DestroyPing();
                });
            }
            await this.Terminal.Dispatcher.InvokeAsync(() =>
            {
                this.Terminal.AppendLine();
            });
        }
    }
}
