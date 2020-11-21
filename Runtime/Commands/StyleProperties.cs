﻿// MIT License
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
using System.IO;
using System.Linq;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public static class StyleProperties
    {
        [CommandPropertySwitch("list")]
        [CommandPropertyTrigger(nameof(IsRemove), false)]
        [CommandPropertyTrigger(nameof(StyleName), "")]
        public static bool IsList { get; set; }

        [CommandPropertySwitch("remove")]
        [CommandPropertyTrigger(nameof(IsList), false)]
        [CommandPropertyTrigger(nameof(StyleName), "")]
        public static bool IsRemove { get; set; }

        [CommandPropertyRequired(DefaultValue = "")]
        public static string StyleName { get; set; }

        public static string[] GetCompletions(CommandMemberDescriptor descriptor, string find)
        {
            if (descriptor.DescriptorName == nameof(StyleName))
            {
                var query = from item in GetStyles().Keys
                            where item.StartsWith(find)
                            select item;
                return query.ToArray();
            }
            return null;
        }

        public static void Execute(TextWriter writer, ITerminalGrid grid)
        {
            if (IsList == true)
            {
                ShowStyleList(writer, grid);
            }
            else if (IsRemove == true)
            {
                RemoveStyle(writer, grid);
            }
            else if (StyleName == string.Empty)
            {
                ShowCurrentStyle(writer, grid);
            }
            else
            {
                ChangeStyle(writer, grid);
            }
        }

        private static void ShowStyleList(TextWriter writer, ITerminalGrid grid)
        {
            var styles = GetStyles();
            var styleName = grid.Style != null ? grid.Style.name : string.Empty;
            foreach (var item in styles.Keys)
            {
                var isCurrent = styleName == item ? "*" : " ";
                writer.WriteLine($"{isCurrent} {item}");
            }
        }

        private static void RemoveStyle(TextWriter writer, ITerminalGrid grid)
        {
            if (grid.Style == null)
                throw new InvalidOperationException("style is not applied.");
            grid.Style = null;
        }

        private static void ShowCurrentStyle(TextWriter writer, ITerminalGrid grid)
        {
            var style = grid.Style;
            if (style != null)
            {
                writer.WriteLine(style.name);
            }
            else
            {
                writer.WriteLine("style is not applied.");
            }
        }

        private static void ChangeStyle(TextWriter writer, ITerminalGrid grid)
        {
            var styles = GetStyles();
            if (styles.ContainsKey(StyleName) == true)
            {
                grid.Style = styles[StyleName];
                writer.WriteLine($"{StyleName} applied.");
            }
            else
            {
                writer.WriteLine($"{StyleName} style does not exits.");
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