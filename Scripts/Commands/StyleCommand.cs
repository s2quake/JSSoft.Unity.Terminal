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
using System.Collections.Generic;
using System.Linq;

namespace JSSoft.Unity.Terminal.Commands
{
    [CommandSummary(CommandStrings.StyleCommand.Summary)]
    [CommandSummary(CommandStrings.StyleCommand.Summary_ko_KR, Locale = "ko-KR")]
    public class StyleCommand : TerminalCommandBase
    {
        public StyleCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor.DescriptorName == nameof(StyleName))
            {
                var query = from item in GetStyles().Keys
                            where item.StartsWith(completionContext.Find)
                            select item;
                return query.ToArray();
            }
            return null;
        }

        [CommandSummary(CommandStrings.StyleCommand.IsList.Summary)]
        [CommandSummary(CommandStrings.StyleCommand.IsList.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertySwitch("list")]
        [CommandPropertyTrigger(nameof(IsRemove), false)]
        [CommandPropertyTrigger(nameof(StyleName), "")]
        public bool IsList { get; set; }

        [CommandSummary(CommandStrings.StyleCommand.IsRemove.Summary)]
        [CommandSummary(CommandStrings.StyleCommand.IsRemove.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertySwitch("remove")]
        [CommandPropertyTrigger(nameof(IsList), false)]
        [CommandPropertyTrigger(nameof(StyleName), "")]
        public bool IsRemove { get; set; }

        [CommandSummary(CommandStrings.StyleCommand.StyleName.Summary)]
        [CommandSummary(CommandStrings.StyleCommand.StyleName.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertyRequired(DefaultValue = "")]
        public string StyleName { get; set; }

        protected override void OnExecute()
        {
            if (this.IsList == true)
            {
                this.ShowStyleList(this.Grid);
            }
            else if (this.IsRemove == true)
            {
                this.RemoveStyle(this.Grid);
            }
            else if (this.StyleName == string.Empty)
            {
                this.ShowCurrentStyle(this.Grid);
            }
            else
            {
                this.ChangeStyle(this.Grid);
            }
        }

        private void ShowStyleList(ITerminalGrid grid)
        {
            var styles = GetStyles();
            var styleName = grid.Style != null ? grid.Style.name : string.Empty;
            foreach (var item in styles.Keys)
            {
                var isCurrent = styleName == item ? "*" : " ";
                this.WriteLine($"{isCurrent} {item}");
            }
        }

        private void RemoveStyle(ITerminalGrid grid)
        {
            if (grid.Style == null)
                throw new InvalidOperationException("style is not applied.");
            grid.Style = null;
        }

        private void ShowCurrentStyle(ITerminalGrid grid)
        {
            var style = grid.Style;
            if (style != null)
            {
                this.WriteLine(style.name);
            }
            else
            {
                this.WriteLine("style is not applied.");
            }
        }

        private void ChangeStyle(ITerminalGrid grid)
        {
            var styles = GetStyles();
            if (styles.ContainsKey(this.StyleName) == true)
            {
                grid.Style = styles[this.StyleName];
                this.WriteLine($"{this.StyleName} applied.");
            }
            else
            {
                this.WriteLine($"{this.StyleName} style does not exits.");
            }
        }

        private static IDictionary<string, TerminalStyle> GetStyles()
        {
            var resources = UnityEngine.GameObject.FindObjectOfType<TerminalStyles>();
            if (resources == null)
                throw new InvalidOperationException("cannot found StyleResources.");
            var styles = new Dictionary<string, TerminalStyle>(resources.Styles.Count);
            foreach (var item in resources.Styles)
            {
                if (item != null)
                {
                    if (item.StyleName != string.Empty)
                        styles.Add(item.StyleName, item);
                    else
                        styles.Add(item.name, item);
                }
            }
            return styles;
        }
    }
}