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
using System.Linq;
using System.Text;
using JSSoft.Library.Commands;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class TerminalCommand : TerminalCommandMethodBase
    {
        public TerminalCommand(ITerminal terminal)
            : base(terminal, new string[] { "ter" })
        {
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Length), nameof(Ratio))]
        public void Dock(TerminalDock? dock = null)
        {
            if (this.DockController is TerminalDockController controller && controller.enabled == true)
            {
                if (dock != null)
                    controller.Dock = dock.Value;
                if (this.Length != null)
                {
                    controller.IsRatio = false;
                    controller.Length = this.Length.Value;
                }
                else if (this.Ratio != null)
                {
                    controller.IsRatio = true;
                    controller.Ratio = this.Ratio.Value;
                }
                else if (dock == null && this.Length == null && this.Ratio == null)
                {
                    this.WriteLine($"dock: {controller.Dock.ToString().ToLower()}");
                    if (controller.IsRatio == true)
                        this.WriteLine($"ratio: {controller.Ratio}");
                    else
                        this.WriteLine($"length: {controller.Length}");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [CommandProperty]
        [CommandPropertyTrigger(nameof(Ratio), null)]
        public int? Length
        {
            get; set;
        }

        [CommandProperty]
        [CommandPropertyTrigger(nameof(Length), null)]
        public float? Ratio
        {
            get; set;
        }

        public string[] CompleteDock(CommandMemberDescriptor descriptor, string find)
        {
            if (descriptor.DescriptorName == "dock")
            {
                var query = from item in Enum.GetNames(typeof(TerminalDock))
                            let name = item.ToLower()
                            where name.StartsWith(find)
                            select name;
                return query.ToArray();
            }
            return null;
        }

        public bool CanDock
        {
            get
            {
                if (this.DockController is TerminalDockController controller)
                {
                    return controller.enabled;
                }
                return false;
            }
        }

        private TerminalDockController DockController
        {
            get
            {
                var gameObject = this.Terminal.GameObject;
                var transform = gameObject.transform;
                if (transform.parent != null && transform.parent.GetComponent<TerminalDockController>() is TerminalDockController controller)
                {
                    return controller;
                }
                return null;
            }
        }
    }
}
