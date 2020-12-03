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

namespace JSSoft.Unity.Terminal.Commands
{
    public abstract class TerminalCommandBase : CommandBase
    {
        protected TerminalCommandBase(ITerminal terminal)
            : this(terminal, new string[] { })
        {
        }

        protected TerminalCommandBase(ITerminal terminal, string[] aliases)
            : base(aliases)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        protected TerminalCommandBase(ITerminal terminal, string name)
            : this(terminal, name, new string[] { })
        {
        }

        protected TerminalCommandBase(ITerminal terminal, string name, string[] aliases)
            : base(name, aliases)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        public new CommandContext CommandContext => base.CommandContext as CommandContext;

        protected void Write(string text)
        {
            this.Write(text, null);
        }

        protected void Write(string text, TerminalColor? foregroundColor)
        {
            this.Write(text, foregroundColor, null);
        }

        protected void Write(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            if (foregroundColor != null)
                this.Terminal.ForegroundColor = foregroundColor;
            if (backgroundColor != null)
                this.Terminal.ForegroundColor = backgroundColor;
            this.Out.Write(text);
            if (foregroundColor != null || backgroundColor != null)
                this.Terminal.ResetColor();
        }

        protected void WriteLine()
        {
            this.WriteLine(string.Empty);
        }

        protected void WriteLine(string text)
        {
            this.WriteLine(text, null);
        }

        protected void WriteLine(string text, TerminalColor? foregroundColor)
        {
            this.WriteLine(text, foregroundColor, null);
        }

        protected void WriteLine(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            if (foregroundColor != null)
                this.Terminal.ForegroundColor = foregroundColor;
            if (backgroundColor != null)
                this.Terminal.ForegroundColor = backgroundColor;
            this.Out.WriteLine(text);
            if (foregroundColor != null || backgroundColor != null)
                this.Terminal.ResetColor();
        }

        protected ITerminal Terminal { get; }

        protected ITerminalGrid Grid { get; }

        protected CommandContextHost CommandContextHost => this.CommandContext.Host; 
    }
}