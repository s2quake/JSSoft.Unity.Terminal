// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using KeyBinding = JSSoft.Terminal.KeyBinding<JSSoft.Terminal.ITerminalGrid>;

namespace JSSoft.Terminal.KeyBindings
{
    public static class TerminalGridKeyBindings
    {
        public static IKeyBindingCollection GetDefaultBindings()
        {
            if (TerminalEnvironment.IsMac == true)
                return TerminalGridKeyBindings.TerminalOnMacOS;
            else if (TerminalEnvironment.IsWindows == true)
                return TerminalGridKeyBindings.TerminalOnWindows;
            return TerminalGridKeyBindings.Common;
        }
        public static readonly IKeyBindingCollection Common = new KeyBindingCollection("Terminal Grid Common Key Bindings")
        {
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageUp, (g) => g.PageUp()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageDown, (g) => g.PageDown())
        };

        public static readonly IKeyBindingCollection TerminalOnMacOS = new KeyBindingCollection("Terminal(MacOS) Grid Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.Alt | EventModifiers.Command, KeyCode.PageUp, (g) => g.LineUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Command, KeyCode.PageDown, (g) => g.LineDown()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Command, KeyCode.Home, (g) => g.ScrollToTop()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Command, KeyCode.End, (g) => g.ScrollToBottom()),
            new KeyBinding(EventModifiers.Command, KeyCode.C, (g) => GUIUtility.systemCopyBuffer = g.Copy()),
            new KeyBinding(EventModifiers.Command, KeyCode.V, (g) => g.Paste(GUIUtility.systemCopyBuffer)),
            new KeyBinding(EventModifiers.Command, KeyCode.A, (g) => g.SelectAll()),
        };

        public static readonly IKeyBindingCollection TerminalOnWindows = new KeyBindingCollection("Terminal(Windows) Grid Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Control, KeyCode.PageUp, (g) => g.LineUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Control, KeyCode.PageDown, (g) => g.LineDown()),
            new KeyBinding(EventModifiers.Control, KeyCode.Home, (g) => g.ScrollToTop()),
            new KeyBinding(EventModifiers.Control, KeyCode.End, (g) => g.ScrollToBottom()),
            new KeyBinding(EventModifiers.Control, KeyCode.C, (g) => GUIUtility.systemCopyBuffer = g.Copy()),
            new KeyBinding(EventModifiers.Control, KeyCode.V, (g) => g.Paste(GUIUtility.systemCopyBuffer)),
            new KeyBinding(EventModifiers.Control, KeyCode.A, (g) => g.SelectAll()),
        };
    }
}
