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

using System.ComponentModel;
using System.Threading.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Terminal.Tasks;

namespace JSSoft.Terminal.Commands
{
    [CommandSummary("Displays or sets the details for the message.")]
    [CommandSummary("메시지에 대한 세부 정보를 표시하거나 설정합니다.", Locale = "ko-KR")]
    public class VerboseCommand : TerminalCommandBase
    {
        public VerboseCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyRequired(DefaultValue = null)]
        public bool? Value { get; set; }

        protected override void OnExecute()
        {
            var value = this.Value;
            if (value == null)
                this.Terminal.AppendLine($"Verbose: {$"{this.Terminal.IsVerbose}".ToLower()}");
            else
                this.Terminal.IsVerbose = (bool)value;
        }
    }
}