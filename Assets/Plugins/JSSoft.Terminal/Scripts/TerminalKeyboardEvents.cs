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
using System.ComponentModel;

namespace JSSoft.Terminal
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
