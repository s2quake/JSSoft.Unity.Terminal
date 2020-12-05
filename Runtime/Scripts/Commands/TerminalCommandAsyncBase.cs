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
using System.Threading;
using System.Threading.Tasks;

namespace JSSoft.Unity.Terminal.Commands
{
    public abstract class TerminalCommandAsyncBase : CommandAsyncBase
    {
        protected TerminalCommandAsyncBase(ITerminal terminal)
            : this(terminal, new string[] { })
        {
        }

        protected TerminalCommandAsyncBase(ITerminal terminal, string[] aliases)
            : base(aliases)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        protected TerminalCommandAsyncBase(ITerminal terminal, string name)
            : this(terminal, name, new string[] { })
        {
        }

        protected TerminalCommandAsyncBase(ITerminal terminal, string name, string[] aliases)
            : base(name, aliases)
        {
            this.Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            this.Grid = terminal.GameObject.GetComponent<ITerminalGrid>();
        }

        public new CommandContext CommandContext => base.CommandContext as CommandContext;

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
            return this.Dispatcher.InvokeAsync(() =>
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

        protected Task WriteLineAsync()
        {
            return this.WriteLineAsync(string.Empty);
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
            return this.Dispatcher.InvokeAsync(() =>
            {
                if (foregroundColor != null)
                    this.Terminal.ForegroundColor = foregroundColor;
                if (backgroundColor != null)
                    this.Terminal.BackgroundColor = backgroundColor;
                this.Out.WriteLine(text);
                if (foregroundColor != null || backgroundColor != null)
                    this.Terminal.ResetColor();
            });
        }

        protected Task CompleteProgressAsync(string message)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                var progressText = this.Terminal.Progress(message, 1);
                this.Terminal.Progress(string.Empty, 0);
                this.Out.WriteLine(progressText);
            });
        }

        protected Task CancelProgressAasync(string message)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                var progressText = this.Terminal.ProgressText;
                this.Terminal.Progress(string.Empty, 0);
                this.Out.WriteLine(progressText);
            });
        }

        protected Task<string> SetProgressAsync(string message, float value)
        {
            return this.Dispatcher.InvokeAsync(() =>
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