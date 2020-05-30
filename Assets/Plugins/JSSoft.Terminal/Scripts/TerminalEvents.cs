// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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
using System.ComponentModel;

namespace JSSoft.Terminal
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
            terminal.PropertyChanged += Terminal_PropertyChanged;
        }

        public static void Unregister(ITerminal terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal));
            if (terminals.Contains(terminal) == false)
                throw new ArgumentException($"{nameof(terminal)} does not exists.");
            terminal.Validated -= Terminal_Validated;
            // terminal.OutputTextChanged += Terminal_OutputTextChanged;
            // terminal.PromptTextChanged += Terminal_PromptTextChanged;
            // terminal.CursorPositionChanged += Terminal_CursorPositionChanged;
            terminal.Enabled -= Terminal_Enabled;
            terminal.Disabled -= Terminal_Disabled;
            terminal.PropertyChanged -= Terminal_PropertyChanged;
            terminals.Remove(terminal);
        }

        public static event EventHandler Validated;

        public static event EventHandler Enabled;

        public static event EventHandler Disabled;

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Terminal_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Terminal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private static void Terminal_Enabled(object sender, EventArgs e)
        {
            Enabled?.Invoke(sender, e);
        }

        private static void Terminal_Disabled(object sender, EventArgs e)
        {
            Disabled?.Invoke(sender, e);
        }
    }
}
