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

using JSSoft.Library.Commands;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [CommandSummary(CommandStrings.PingCommand.Summary)]
    [CommandSummary(CommandStrings.PingCommand.Summary_ko_KR, Locale = "ko-KR")]
    public class PingCommand : TerminalCommandAsyncBase
    {
        public PingCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandSummary(CommandStrings.PingCommand.Address.Summary)]
        [CommandSummary(CommandStrings.PingCommand.Address.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertyRequired]
        public string Address
        {
            get; set;
        }

        [CommandSummary(CommandStrings.PingCommand.Count.Summary)]
        [CommandSummary(CommandStrings.PingCommand.Count.Summary_ko_KR, Locale = "ko-KR")]
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
                var ping = await this.Dispatcher.InvokeAsync(() => new Ping(address));
                do
                {
                    await Task.Delay(1000);
                } while (await this.Dispatcher.InvokeAsync(() => ping.isDone) == false);
                await this.WriteLineAsync($"{address}: {ping.time}");
                await this.Dispatcher.InvokeAsync(ping.DestroyPing);
            }
            await this.WriteLineAsync();
        }
    }
}
