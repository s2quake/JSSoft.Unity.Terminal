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
            this.dispatcher.VerifyAccess();
            base.Write(value);
            this.terminal.Append(value.ToString());
        }

        public override void WriteLine()
        {
            this.dispatcher.VerifyAccess();
            base.WriteLine();
            this.terminal.Append(Environment.NewLine);
        }

        public override void WriteLine(string value)
        {
            this.dispatcher.VerifyAccess();
            base.WriteLine(value);
            this.terminal.Append(value + Environment.NewLine);
        }

        public override void Write(string value)
        {
            this.dispatcher.VerifyAccess();
            base.Write(value);
            this.terminal.Append(value);
        }
    }
}