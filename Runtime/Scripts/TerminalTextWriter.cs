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

using System.IO;
using System.Text;

namespace JSSoft.Unity.Terminal
{
    public class TerminalTextWriter : TextWriter
    {
        private readonly ITerminal terminal;

        public TerminalTextWriter(ITerminal terminal)
        {
            this.terminal = terminal;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override async void Write(char value)
        {
            if (this.Dispatcher.CheckAccess() == true)
                this.terminal.Append($"{value}");
            else
                await this.Dispatcher.InvokeAsync(() => this.terminal.Append($"{value}"));
        }

        public override async void Write(string value)
        {
            if (this.Dispatcher.CheckAccess() == true)
                this.terminal.Append($"{value}");
            else
                await this.Dispatcher.InvokeAsync(() => this.terminal.Append($"{value}"));
        }

        public override async void WriteLine(string value)
        {
            if (this.Dispatcher.CheckAccess() == true)
                this.terminal.AppendLine($"{value}");
            else
                await this.Dispatcher.InvokeAsync(() => this.terminal.AppendLine($"{value}"));
        }

        private TerminalDispatcher Dispatcher => this.terminal.Dispatcher;
    }
}
