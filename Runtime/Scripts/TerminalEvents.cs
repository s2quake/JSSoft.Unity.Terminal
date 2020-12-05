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
using System.Collections.Generic;
using System.ComponentModel;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalEvents
    {
        private static readonly HashSet<ITerminal> terminals = new HashSet<ITerminal>();

        public static void Register(ITerminal terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal));
            if (terminals.Contains(terminal) == true)
                throw new ArgumentException($"{nameof(terminal)} is already exists.");
            terminals.Add(terminal);
            terminal.Validated += Terminal_Validated;
            terminal.Enabled += Terminal_Enabled;
            terminal.Disabled += Terminal_Disabled;
            terminal.CancellationRequested += Terminal_CancellationRequested;
            terminal.PropertyChanged += Terminal_PropertyChanged;
            terminal.TextChanged += Terminal_TextChanged;
            terminal.Executing += Terminal_Executing;
            terminal.Executed += Terminal_Executed;
        }

        public static void Unregister(ITerminal terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal));
            if (terminals.Contains(terminal) == false)
                throw new ArgumentException($"{nameof(terminal)} does not exists.");
            terminal.Validated -= Terminal_Validated;
            terminal.Enabled -= Terminal_Enabled;
            terminal.Disabled -= Terminal_Disabled;
            terminal.CancellationRequested -= Terminal_CancellationRequested;
            terminal.PropertyChanged -= Terminal_PropertyChanged;
            terminal.TextChanged -= Terminal_TextChanged;
            terminal.Executing -= Terminal_Executing;
            terminal.Executed -= Terminal_Executed;
            terminals.Remove(terminal);
        }

        public static event EventHandler Validated;

        public static event EventHandler Enabled;

        public static event EventHandler Disabled;

        public static event EventHandler CancellationRequested;

        public static event PropertyChangedEventHandler PropertyChanged;

        public static event EventHandler<TextChangedEventArgs> TextChanged;

        public static event EventHandler Executing;

        public static event EventHandler<TerminalExecutedEventArgs> Executed;

        private static void Terminal_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Terminal_Enabled(object sender, EventArgs e)
        {
            Enabled?.Invoke(sender, e);
        }

        private static void Terminal_Disabled(object sender, EventArgs e)
        {
            Disabled?.Invoke(sender, e);
        }

        private static void Terminal_CancellationRequested(object sender, EventArgs e)
        {
            CancellationRequested?.Invoke(sender, e);
        }

        private static void Terminal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private static void Terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }

        private static void Terminal_Executing(object sender, TerminalExecuteEventArgs e)
        {
            Executing?.Invoke(sender, EventArgs.Empty);
        }

        private static void Terminal_Executed(object sender, TerminalExecutedEventArgs e)
        {
            Executed?.Invoke(sender, e);
        }
    }
}
