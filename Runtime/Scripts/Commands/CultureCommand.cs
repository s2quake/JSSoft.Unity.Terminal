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
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class CultureCommand : TerminalCommandBase
    {
        private static readonly CultureInfo defaultCulture;
        private static readonly CultureInfo defaultUICulture;

        static CultureCommand()
        {
            defaultCulture = CultureInfo.DefaultThreadCurrentCulture;
            defaultUICulture = CultureInfo.DefaultThreadCurrentUICulture;
        }

        public CultureCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyRequired(DefaultValue = "")]
        public string Culture
        {
            get; set;
        }

        [CommandPropertySwitch("ui")]
        public bool IsUI
        {
            get; set;
        }

        [CommandPropertySwitch("reset")]
        [CommandPropertyTrigger(nameof(Culture), "")]
        public bool ResetSwitch
        {
            get; set;
        }

        protected override void OnExecute()
        {
            if (this.ResetSwitch == true)
            {
                if (this.IsUI == true)
                    CultureInfo.DefaultThreadCurrentUICulture = defaultUICulture;
                else
                    CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            }
            else if (this.Culture == string.Empty)
            {
                if (this.IsUI == true)
                    this.WriteLine($"{nameof(CultureInfo.CurrentUICulture)}: {CultureInfo.DefaultThreadCurrentUICulture}");
                else
                    this.WriteLine($"{nameof(CultureInfo.CurrentCulture)}: {CultureInfo.DefaultThreadCurrentCulture}");
            }
            else
            {
                var culture = CultureInfo.CreateSpecificCulture(this.Culture);
                if (this.IsUI == true)
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                else
                    CultureInfo.DefaultThreadCurrentCulture = culture;
            }
        }
    }
}