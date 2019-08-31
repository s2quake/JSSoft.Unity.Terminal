using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ntreev.Library.Threading;
using JSSoft.UI;

namespace JSSoft.Communication.Shells
{
    class CommandWriter : StringWriter
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
            base.Write(value);
            this.dispatcher.InvokeAsync(() => this.terminal.Append(value.ToString()));
        }

        public override void WriteLine()
        {
            base.WriteLine();
            this.dispatcher.InvokeAsync(() => this.terminal.Append(Environment.NewLine));
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            this.dispatcher.InvokeAsync(() => this.terminal.Append(value + Environment.NewLine));
        }

        public override void Write(string value)
        {
            base.Write(value);
            this.dispatcher.InvokeAsync(() => this.terminal.Append(value));
        }
    }
}