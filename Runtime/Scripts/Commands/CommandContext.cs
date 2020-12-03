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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandContext : CommandContextBase
    {
        private readonly ITerminal terminal;
        private readonly VersionCommand versionCommand = new VersionCommand();
        private readonly CommandContextHost host;

        public CommandContext(ITerminal terminal, IEnumerable<ICommand> commands, CommandContextHost host)
            : base("UnityCommand", commands)
        {
            this.terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public new string[] GetCompletion(string[] items, string find)
        {
            return base.GetCompletion(items, find);
        }

        public CommandContextHost Host { get; }
    }
}