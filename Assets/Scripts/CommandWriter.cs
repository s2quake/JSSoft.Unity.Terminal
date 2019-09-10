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