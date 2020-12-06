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

using System.Linq;
using UnityEngine;
using KeyBinding = JSSoft.Unity.Terminal.KeyBinding<JSSoft.Unity.Terminal.ITerminalGrid>;

namespace JSSoft.Unity.Terminal.KeyBindings
{
    public static class TerminalGridKeyBindings
    {
        public static IKeyBindingCollection GetDefaultBindings()
        {
            if (TerminalEnvironment.IsMac == true)
                return TerminalGridKeyBindings.TerminalOnMacOS;
            else if (TerminalEnvironment.IsWindows == true)
                return TerminalGridKeyBindings.TerminalOnWindows;
            else if (TerminalEnvironment.IsLinux == true)
                return TerminalGridKeyBindings.TerminalOnLinux;
            return TerminalGridKeyBindings.Common;
        }
        public static readonly IKeyBindingCollection Common = new KeyBindingCollection("Terminal Grid Common Key Bindings")
        {
            
        };

        public static readonly IKeyBindingCollection TerminalOnMacOS = new KeyBindingCollection("Terminal(MacOS) Grid Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageUp, (g) => g.PageUp()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageDown, (g) => g.PageDown()),
            new KeyBinding(EventModifiers.Alt | EventModifiers.Command, KeyCode.PageUp, (g) => g.LineUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Command, KeyCode.PageDown, (g) => g.LineDown()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Command, KeyCode.Home, (g) => g.ScrollToTop()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Command, KeyCode.End, (g) => g.ScrollToBottom()),
            new KeyBinding(EventModifiers.Command, KeyCode.C, (g) => GUIUtility.systemCopyBuffer = g.Copy()),
            new KeyBinding(EventModifiers.Command, KeyCode.V, (g) => g.Paste(GUIUtility.systemCopyBuffer)),
            new KeyBinding(EventModifiers.Command, KeyCode.A, (g) => g.SelectAll()),
            new KeyBinding(EventModifiers.Control, KeyCode.C, (g) => Cancel(g), (g) => CanCancel(g)) { IsPreview = true}
        };

        public static readonly IKeyBindingCollection TerminalOnWindows = new KeyBindingCollection("Terminal(Windows) Grid Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageUp, (g) => g.PageUp()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageDown, (g) => g.PageDown()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Control, KeyCode.PageUp, (g) => g.LineUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Control, KeyCode.PageDown, (g) => g.LineDown()),
            new KeyBinding(EventModifiers.Control, KeyCode.Home, (g) => g.ScrollToTop()),
            new KeyBinding(EventModifiers.Control, KeyCode.End, (g) => g.ScrollToBottom()),
            new KeyBinding(EventModifiers.Control, KeyCode.C, (g) => GUIUtility.systemCopyBuffer = g.Copy()),
            new KeyBinding(EventModifiers.Control, KeyCode.V, (g) => g.Paste(GUIUtility.systemCopyBuffer)),
            new KeyBinding(EventModifiers.Control, KeyCode.A, (g) => g.SelectAll()),
            new KeyBinding(EventModifiers.Control, KeyCode.C, (g) => Cancel(g), (g) => CanCancel(g)) { IsPreview = true}
        };

        public static readonly IKeyBindingCollection TerminalOnLinux = new KeyBindingCollection("Terminal(Linux) Grid Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.PageUp, (g) => g.PageUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.PageDown, (g) => g.PageDown()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Control | EventModifiers.Shift, KeyCode.DownArrow, (g) => g.LineDown()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Control | EventModifiers.Shift, KeyCode.UpArrow, (g) => g.LineUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.Home, (g) => g.ScrollToTop()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Shift, KeyCode.End, (g) => g.ScrollToBottom()),
            new KeyBinding(EventModifiers.Control | EventModifiers.Shift, KeyCode.C, (g) => GUIUtility.systemCopyBuffer = g.Copy()),
            new KeyBinding(EventModifiers.Control | EventModifiers.Shift, KeyCode.V, (g) => g.Paste(GUIUtility.systemCopyBuffer)),
            new KeyBinding(EventModifiers.Control, KeyCode.C, (g) => Cancel(g), (g) => CanCancel(g)) { IsPreview = true}
        };

        private static void Cancel(ITerminalGrid grid)
        {
            var terminal = grid.Terminal;
            terminal.Cancel();
        }

        private static bool CanCancel(ITerminalGrid grid)
        {
            var terminal = grid.Terminal;
            if (terminal.IsExecuting == true && grid.Selections.Any() == false)
                return true;
            return false;
        }
    }
}
