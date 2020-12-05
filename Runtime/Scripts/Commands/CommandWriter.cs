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

using JSSoft.Library.Threading;
using System;
using System.IO;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandWriter : StringWriter
    {
        private readonly ITerminal terminal;
        private readonly Dispatcher dispatcher;

        public CommandWriter(ITerminal terminal)
        {
            this.terminal = terminal;
            this.dispatcher = Dispatcher.Current;
        }

        public override void Write(char value)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(value.ToString());
            }
            else
            {
                this.dispatcher.InvokeAsync(() => this.terminal.Append(value.ToString()));
            }
        }

        public override void WriteLine()
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(Environment.NewLine);
            }
            else
            {
                this.dispatcher.InvokeAsync(() => this.terminal.Append(Environment.NewLine));
            }
        }

        public override void WriteLine(string value)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(value + Environment.NewLine);
            }
            else
            {
                this.dispatcher.InvokeAsync(() => this.terminal.Append(value + Environment.NewLine));
            }
        }

        public override void Write(string value)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                this.terminal.Append(value);
            }
            else
            {
                this.dispatcher.InvokeAsync(() => this.terminal.Append(value));
            }
        }
    }
}