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

using System;
using System.Globalization;
using JSSoft.Library.Commands;

namespace JSSoft.Unity.Terminal.Commands
{
    [CommandSummary(CommandStrings.DateCommand.Summary)]
    [CommandSummary(CommandStrings.DateCommand.Summary_ko_KR, Locale = "ko-KR")]
    public class DateCommand : TerminalCommandBase
    {
        public DateCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandProperty]
        [CommandSummary(CommandStrings.DateCommand.Format.Summary)]
        [CommandSummary(CommandStrings.DateCommand.Format.Summary_ko_KR, Locale = "ko-KR")]
        public string Format { get; set; }

        [CommandProperty]
        [CommandSummary(CommandStrings.DateCommand.Locale.Summary)]
        [CommandSummary(CommandStrings.DateCommand.Locale.Summary_ko_KR, Locale = "ko-KR")]
        public string Locale { get; set; }

        [CommandProperty]
        [CommandSummary(CommandStrings.DateCommand.UTC.Summary)]
        [CommandSummary(CommandStrings.DateCommand.UTC.Summary_ko_KR, Locale = "ko-KR")]
        public bool UTC { get; set; }

        protected override void OnExecute()
        {
            var dateTime = this.UTC == true ? DateTime.UtcNow : DateTime.Now;
            var cultureInfo = this.Locale != null ? new CultureInfo(this.Locale) : CultureInfo.CurrentCulture;
            var format = this.Format ?? "G";
            this.WriteLine(dateTime.ToString(format, cultureInfo));
        }
    }
}