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

using System.Threading.Tasks;
using System.Collections.Generic;

namespace JSSoft.Unity.Terminal.Commands
{
    [TestCommand]
    class TestCommand : TerminalCommandAsyncBase
    {
        public TestCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        protected override async Task OnExecuteAsync()
        {
            var items = new Dictionary<string, int>()
            {
                { "Download1", 50 },
                { "Download2", 30 },
                { "Download3", 40 },
                { "Download4", 90 },
                { "Download5", 30 },
                { "Download6", 150 },
                { "Download7", 10 },
                { "Download8", 15 },
                { "Download9", 20 }
            };

            await this.WriteLineAsync("Start Loading.", TerminalColor.Blue);
            foreach (var item in items)
            {
                for (var i = 0; i < item.Value; i++)
                {
                    await this.SetProgressAsync(item.Key, (float)i / item.Value);
                }
                await this.CompleteProgressAsync(item.Key);
            }
            await this.WriteLineAsync("Completed");
            await Task.Delay(1);
        }
    }
}