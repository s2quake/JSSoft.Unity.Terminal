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
        [CommandMethodStaticProperty(typeof(StyleProperties))]
        public void Style()
        {
            StyleProperties.Execute(this.Out, this.Grid);
        }

        public string[] CompleteStyle(CommandMemberDescriptor descriptor, string find)
        {
            return StyleProperties.GetCompletions(descriptor, find);
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(DockProperties))]
        public void Dock(TerminalDock? dock = null)
        {
            if (this.DockController is TerminalDockController controller && controller.enabled == true)
            {
                if (dock != null)
                    controller.Dock = dock.Value;
                if (DockProperties.Length != null)
                {
                    controller.IsRatio = false;
                    controller.Length = DockProperties.Length.Value;
                }
                else if (DockProperties.Ratio != null)
                {
                    controller.IsRatio = true;
                    controller.Ratio = DockProperties.Ratio.Value;
                }
                else if (dock == null)
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
