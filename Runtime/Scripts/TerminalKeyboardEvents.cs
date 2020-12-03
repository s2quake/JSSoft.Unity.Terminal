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

using System;
using System.Collections.Generic;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalKeyboardEvents
    {
        private static readonly HashSet<ITerminalKeyboard> keyboards = new HashSet<ITerminalKeyboard>();

        public static void Register(ITerminalKeyboard keyboard)
        {
            if (keyboard == null)
                throw new ArgumentNullException(nameof(keyboard));
            if (keyboards.Contains(keyboard) == true)
                throw new ArgumentException($"{nameof(keyboard)} is already exists.");
            keyboards.Add(keyboard);
            keyboard.Opened += Terminal_Opened;
            keyboard.Done += Terminal_Done;
            keyboard.Canceled += Terminal_Canceled;
            keyboard.Changed += Terminal_Changed;
        }

        public static void Unregister(ITerminalKeyboard keyboard)
        {
            if (keyboard == null)
                throw new ArgumentNullException(nameof(keyboard));
            if (keyboards.Contains(keyboard) == false)
                throw new ArgumentException($"{nameof(keyboard)} does not exists.");
            keyboard.Opened -= Terminal_Opened;
            keyboard.Done -= Terminal_Done;
            keyboard.Canceled -= Terminal_Canceled;
            keyboard.Changed -= Terminal_Changed;
            keyboards.Remove(keyboard);
        }

        public static event EventHandler<TerminalKeyboardEventArgs> Opened;

        public static event EventHandler<TerminalKeyboardEventArgs> Done;

        public static event EventHandler Canceled;

        public static event EventHandler<TerminalKeyboardEventArgs> Changed;

        private static void Terminal_Opened(object sender, TerminalKeyboardEventArgs e)
        {
            Opened?.Invoke(sender, e);
        }

        private static void Terminal_Done(object sender, TerminalKeyboardEventArgs e)
        {
            Done?.Invoke(sender, e);
        }

        private static void Terminal_Canceled(object sender, EventArgs e)
        {
            Canceled?.Invoke(sender, e);
        }

        private static void Terminal_Changed(object sender, TerminalKeyboardEventArgs e)
        {
            Changed?.Invoke(sender, e);
        }
    }
}
