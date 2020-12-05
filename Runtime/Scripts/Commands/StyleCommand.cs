////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
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

        [CommandPropertySwitch("list")]
        [CommandPropertyTrigger(nameof(IsRemove), false)]
        [CommandPropertyTrigger(nameof(StyleName), "")]
        public bool IsList { get; set; }

        [CommandPropertySwitch("remove")]
        [CommandPropertyTrigger(nameof(IsList), false)]
        [CommandPropertyTrigger(nameof(StyleName), "")]
        public bool IsRemove { get; set; }

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