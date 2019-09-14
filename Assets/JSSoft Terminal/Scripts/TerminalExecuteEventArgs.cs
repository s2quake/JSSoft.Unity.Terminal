using System;
using System.Collections.Generic;

namespace JSSoft.UI
{
    public class TerminalExecuteEventArgs : EventArgs
    {
        private readonly Action endAction;
        private bool handled;

        public TerminalExecuteEventArgs(string command, Action endAction)
        {
            this.Command = command;
            this.endAction = endAction;
        }

        public string Command { get; }

        public bool Handled
        {
            get => this.handled;
            set
            {
                if (this.handled == false && value == true)
                {
                    this.endAction();
                    this.handled = true;
                }
            }
        }
    }
}
