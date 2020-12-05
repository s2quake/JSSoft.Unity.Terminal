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

using JSSoft.Library.Commands;
using System;
using System.Threading.Tasks;

namespace JSSoft.Unity.Terminal.Commands
{
    public abstract class TerminalCommandMethodBase : CommandMethodBase
    {
        protected TerminalCommandMethodBase(ITerminal terminal)
            : this(terminal, new string[] { })
        {
        }

        protected TerminalCommandMethodBase(ITerminal terminal, string[] aliases)
            : base(aliases)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        protected TerminalCommandMethodBase(ITerminal terminal, string name)
           : this(terminal, name, new string[] { })
        {
        }

        protected TerminalCommandMethodBase(ITerminal terminal, string name, string[] aliases)
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

        protected Task WriteAsync(string text)
        {
            return this.WriteAsync(text, null);
        }

        protected Task WriteAsync(string text, TerminalColor? foregroundColor)
        {
            return this.WriteAsync(text, foregroundColor, null);
        }

        protected Task WriteAsync(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            return this.Terminal.Dispatcher.InvokeAsync(() =>
            {
                if (foregroundColor != null)
                    this.Terminal.ForegroundColor = foregroundColor;
                if (backgroundColor != null)
                    this.Terminal.ForegroundColor = backgroundColor;
                this.Out.Write(text);
                if (foregroundColor != null || backgroundColor != null)
                    this.Terminal.ResetColor();
            });
        }

        protected Task WriteLineAsync(string text)
        {
            return this.WriteLineAsync(text, null);
        }

        protected Task WriteLineAsync(string text, TerminalColor? foregroundColor)
        {
            return this.WriteLineAsync(text, foregroundColor, null);
        }

        protected Task WriteLineAsync(string text, TerminalColor? foregroundColor, TerminalColor? backgroundColor)
        {
            return this.Terminal.Dispatcher.InvokeAsync(() =>
            {
                if (foregroundColor != null)
                    this.Terminal.ForegroundColor = foregroundColor;
                if (backgroundColor != null)
                    this.Terminal.ForegroundColor = backgroundColor;
                this.Out.WriteLine(text);
                if (foregroundColor != null || backgroundColor != null)
                    this.Terminal.ResetColor();
            });
        }

        protected Task CompleteProgressAsync(string message)
        {
            return this.Terminal.Dispatcher.InvokeAsync(() =>
            {
                var progressText = this.Terminal.Progress(message, 1);
                this.Terminal.Progress(string.Empty, 0);
                this.Out.WriteLine(progressText);
            });
        }

        protected Task CancelProgressAasync(string message)
        {
            return this.Terminal.Dispatcher.InvokeAsync(() =>
            {
                var progressText = this.Terminal.ProgressText;
                this.Terminal.Progress(string.Empty, 0);
                this.Out.WriteLine(progressText);
            });
        }

        protected Task<string> SetProgressAsync(string message, float value)
        {
            return this.Terminal.Dispatcher.InvokeAsync(() =>
            {
                this.Terminal.Progress(message, value);
                return this.Terminal.ProgressText;
            });
        }

        protected ITerminal Terminal { get; }

        protected ITerminalGrid Grid { get; }

        protected TerminalDispatcher Dispatcher => this.Terminal.Dispatcher;
    }
}