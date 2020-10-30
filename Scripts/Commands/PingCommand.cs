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
using System;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class PingCommand : TerminalCommandAsyncBase
    {
        private const string ipPattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        private CancellationTokenSource cancellation;

        public PingCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyRequired]
        public string Address
        {
            get; set;
        }

        [CommandProperty(InitValue = 3)]
        public int Count
        {
            get; set;
        }

        [CommandProperty('w', InitValue = 4000)]
        public int Timeout
        {
            get; set;
        }

        protected override async Task OnExecuteAsync(CancellationToken cancellation)
        {
            var address = GetIPAddress(this.Address);
            var count = this.Count;
            this.cancellation = new CancellationTokenSource();
            this.Terminal.CancellationRequested += Terminal_CancellationRequested;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    if (await this.PingAsync(address, this.cancellation.Token) == false)
                        return;
                }
            }
            finally
            {
                this.Terminal.CancellationRequested -= Terminal_CancellationRequested;
                this.cancellation = null;
                await this.WriteLineAsync();
            }
        }
        
        private void Terminal_CancellationRequested(object sender, EventArgs e)
        {
            this.cancellation.Cancel();
        }

        private async Task<bool> PingAsync(string address, CancellationToken cancellation)
        {
            var ping = await this.Dispatcher.InvokeAsync(() => new Ping(address));
            var time = DateTime.Now;
            do
            {
                await Task.Delay(1);
                if (cancellation.IsCancellationRequested == true)
                {
                    await this.WriteLineAsync("The operation was canceled.");
                    return false;
                }
            } while (await this.Dispatcher.InvokeAsync(() => ping.isDone) == false && (DateTime.Now - time).TotalMilliseconds < this.Timeout);
            if (ping.isDone == true)
                await this.WriteLineAsync($"{address}: {ping.time}");
            else
                await this.WriteLineAsync($"{address}: timeout");
            await this.Dispatcher.InvokeAsync(ping.DestroyPing);
            return true;
        }

        private static string GetIPAddress(string address)
        {
            if (Regex.IsMatch(address, ipPattern) == true)
            {
                return address;
            }
            else if (Uri.TryCreate(address, UriKind.Absolute, out var uri) == true)
            {
                var ipEntry = Dns.GetHostEntry(uri.Host);
                if (ipEntry.AddressList[0] != null)
                {
                    return $"{ipEntry.AddressList[0]}";
                }
            }
            else
            {
                var ipEntry = Dns.GetHostEntry(address);
                if (ipEntry.AddressList[0] != null)
                {
                    return $"{ipEntry.AddressList[0]}";
                }
            }
            throw new NotImplementedException($"'{address}' is invalid address");
        }
    }
}
