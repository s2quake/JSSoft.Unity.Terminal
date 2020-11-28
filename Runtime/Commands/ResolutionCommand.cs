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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class ResolutionCommand : TerminalCommandBase
    {
        private const string fullPattern = @"(\d+)\s*x\s*(\d+)\s*@\s*(\d+)\s*(?:hz)?";
        private const string shortPattern = @"(\d+)\s*x\s*(\d+)";

        public ResolutionCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyRequired(DefaultValue = "")]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public string Resolution { get; set; }

        [CommandPropertySwitch("window")]
        [CommandPropertyTrigger(nameof(IsFullScreen), false)]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public bool IsWindowMode { get; set; }

        [CommandPropertySwitch("full")]
        [CommandPropertyTrigger(nameof(IsWindowMode), false)]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public bool IsFullScreen { get; set; }

        [CommandPropertySwitch("list")]
        public bool IsList { get; set; }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor.DescriptorName == nameof(Resolution))
            {
                var query = from item in Screen.resolutions
                            let text = $"{item}"
                            where text.StartsWith(completionContext.Find)
                            select text;
                return query.ToArray();
            }
            return null;
        }

        protected override void OnExecute()
        {
            this.Execute();
        }

        private void Execute()
        {
            if (this.IsList == true)
            {
                this.ShowResolutionList();
            }
            else if (this.Resolution == string.Empty)
            {
                if (this.IsFullScreen == true || this.IsWindowMode == true)
                {
                    this.SetFullScreenMode();
                }
                else
                {
                    this.ShowCurrentResolution();
                }
            }
            else
            {
                this.SetResolution(this.Resolution);
            }
        }

        private void ShowResolutionList()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Screen.resolutions.Length; i++)
            {
                var item = Screen.resolutions[i];
                sb.AppendLine($"{i,2}: {item}");
            }
            this.WriteLine(sb.ToString());
        }

        private void SetFullScreenMode()
        {
            var fullScreen = this.PreferredFullScreen;
            var width = Screen.width;
            var height = Screen.height;
            var hz = Screen.currentResolution.refreshRate;
            Screen.SetResolution(width, height, fullScreen, hz);
        }

        private void ShowCurrentResolution()
        {
            this.WriteLine($"{Screen.width} x {Screen.height} @ {Screen.currentResolution.refreshRate}Hz");
        }

        private void SetResolution(string resolution)
        {
            var fullScreen = this.PreferredFullScreen;
            if (int.TryParse(resolution, out var index) == true)
            {
                if (index < 0 || index >= Screen.resolutions.Length)
                    throw new ArgumentException($"invalid resolution index: '{index}'");
                var item = Screen.resolutions[index];
                Screen.SetResolution(item.width, item.height, fullScreen, item.refreshRate);
            }
            else if (Regex.Match(resolution, fullPattern, RegexOptions.IgnoreCase) is Match match1 && match1.Success == true)
            {
                var width = int.Parse(match1.Groups[1].Value);
                var height = int.Parse(match1.Groups[2].Value);
                var hz = int.Parse(match1.Groups[3].Value);
                Screen.SetResolution(width, height, fullScreen, hz);
            }
            else if (Regex.Match(resolution, shortPattern, RegexOptions.IgnoreCase) is Match match2 && match2.Success == true)
            {
                var width = int.Parse(match2.Groups[1].Value);
                var height = int.Parse(match2.Groups[2].Value);
                var hz = Screen.currentResolution.refreshRate;
                Screen.SetResolution(width, height, fullScreen, hz);
            }
            else
            {
                throw new ArgumentException($"invalid resolution format: '{resolution}'");
            }
        }

        private bool PreferredFullScreen
        {
            get
            {
                if (IsWindowMode == true)
                    return false;
                else if (IsFullScreen == true)
                    return true;
                return Screen.fullScreen;
            }
        }
    }
}
