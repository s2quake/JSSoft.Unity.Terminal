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

namespace JSSoft.Unity.Terminal
{
    public class TerminalExecuteEventArgs : EventArgs
    {
        private readonly Action<Exception> action;
        private bool isHandled;
        private Guid token;

        public TerminalExecuteEventArgs(string command, Action<Exception> action)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public string Command { get; }

        public bool IsHandled => this.isHandled;

        public Guid GetToken()
        {
            if (this.token != Guid.Empty)
                throw new InvalidOperationException("Token has already been created.");
            this.token = Guid.NewGuid();
            return this.token;
        }

        public void Success(Guid token)
        {
            if (token == Guid.Empty || token != this.token)
                throw new InvalidOperationException("Invalid token.");
            if (this.isHandled == true)
                throw new InvalidOperationException("command expired.");
            this.isHandled = true;
            this.action(null);
        }

        public void Fail(Guid token, Exception exception)
        {
            if (token == Guid.Empty || token != this.token)
                throw new InvalidOperationException("Invalid token.");
            if (this.isHandled == true)
                throw new InvalidOperationException("command expired.");
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            this.isHandled = true;
            this.action(exception);
        }

        internal Guid Token => this.token;
    }
}
