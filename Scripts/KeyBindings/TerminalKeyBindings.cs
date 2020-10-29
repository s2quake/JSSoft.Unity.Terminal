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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KeyBinding = JSSoft.Unity.Terminal.KeyBinding<JSSoft.Unity.Terminal.ITerminal>;

namespace JSSoft.Unity.Terminal.KeyBindings
{
    public static class TerminalKeyBindings
    {
        public static IKeyBindingCollection GetDefaultBindings()
        {
            if (TerminalEnvironment.IsMac == true)
                return TerminalKeyBindings.TerminalOnMacOS;
            else if (TerminalEnvironment.IsWindows == true)
                return TerminalKeyBindings.TerminalOnWindows;
            return TerminalKeyBindings.Common;
        }

        public static readonly IKeyBindingCollection Common = new KeyBindingCollection("Terminal Key Bindings")
        {
#if UNITY_ANDROID
            new KeyBinding(EventModifiers.None, KeyCode.UpArrow, (t) => t.PrevHistory()),
            new KeyBinding(EventModifiers.None, KeyCode.DownArrow, (t) => t.NextHistory()),
            new KeyBinding(EventModifiers.None, KeyCode.LeftArrow, (t) => t.MoveLeft()),
            new KeyBinding(EventModifiers.None, KeyCode.RightArrow, (t) => t.MoveRight()),
            new KeyBinding(EventModifiers.None, KeyCode.Backspace, (t) => t.Backspace()),
#else
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.UpArrow, (t) => t.PrevHistory()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.DownArrow, (t) => t.NextHistory()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.LeftArrow, (t) => t.MoveLeft()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.RightArrow, (t) => t.MoveRight()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.Backspace, (t) => t.Backspace()),
            // ime 입력중에 Backspace 키를 누르면 두번이 호출됨 그중 처음에는 EventModifiers.FunctionKey + KeyCode.Backspace 가 호출됨.
            new KeyBinding(EventModifiers.None, KeyCode.Backspace, (t) => true),
#endif
            new KeyBinding(EventModifiers.Shift, KeyCode.LeftArrow, (t) => true),
            new KeyBinding(EventModifiers.Shift, KeyCode.RightArrow, (t) => true),
            new KeyBinding(EventModifiers.Shift, KeyCode.UpArrow, (t) => true),
            new KeyBinding(EventModifiers.Shift, KeyCode.DownArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.LeftArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.RightArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.UpArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.DownArrow, (t) => true),

            new KeyBinding(EventModifiers.FunctionKey, KeyCode.Delete, (t) => t.Delete()),
            new KeyBinding(EventModifiers.None, KeyCode.Tab, (t) => t.NextCompletion()),
            new KeyBinding(EventModifiers.Shift, KeyCode.Tab, (t) => t.PrevCompletion()),
        };

        public static readonly IKeyBindingCollection TerminalOnMacOS = new KeyBindingCollection("Terminal(MacOS) Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.Control, KeyCode.U, (t) => DeleteToFirst(t)),
            new KeyBinding(EventModifiers.Control, KeyCode.K, (t) => DeleteToLast(t)),
            new KeyBinding(EventModifiers.Control, KeyCode.E, (t) => t.MoveToLast()),
            new KeyBinding(EventModifiers.Control, KeyCode.A, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.Control, KeyCode.W, (t) => DeletePrevWord(t)),
            new KeyBinding(EventModifiers.Alt | EventModifiers.FunctionKey, KeyCode.LeftArrow, (t) => PrevWord(t)),
            new KeyBinding(EventModifiers.Alt | EventModifiers.FunctionKey, KeyCode.RightArrow, (t) => NextWord(t)),
        };

        public static readonly IKeyBindingCollection TerminalOnWindows = new KeyBindingCollection("Terminal(Windows) Key Bindings", Common)
        {
            new KeyBinding(EventModifiers.None, KeyCode.Escape, (t) => t.Command = string.Empty),
            new KeyBinding(EventModifiers.Alt, KeyCode.U, (t) => DeleteToFirst(t)),
            new KeyBinding(EventModifiers.Alt, KeyCode.K, (t) => DeleteToLast(t)),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.Home, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.End, (t) => t.MoveToLast()),
            new KeyBinding(EventModifiers.Control | EventModifiers.FunctionKey, KeyCode.LeftArrow, (t) => PrevWord(t)),
            new KeyBinding(EventModifiers.Control | EventModifiers.FunctionKey, KeyCode.RightArrow, (t) => NextWord(t))
        };

        private static int PrevWord(ITerminal terminal)
        {
            if (terminal.CursorPosition > 0)
            {
                var index = terminal.CursorPosition - 1;
                var command = terminal.Command;
                var pattern = @"^\w|(?=\b)\w|$";
                var matches = Regex.Matches(command, pattern).Cast<Match>();
                var match = matches.Where(item => item.Index <= index).Last();
                terminal.CursorPosition = match.Index;
            }
            return terminal.CursorPosition;
        }

        private static int NextWord(ITerminal terminal)
        {
            var command = terminal.Command;
            if (terminal.CursorPosition < command.Length)
            {
                var index = terminal.CursorPosition;
                var pattern = @"\w(?<=\b)|$";
                var matches = Regex.Matches(command, pattern).Cast<Match>();
                var match = matches.Where(item => item.Index > index).First();
                terminal.CursorPosition = Math.Min(command.Length, match.Index + 1);
            }
            return terminal.CursorPosition;
        }

        private static void DeleteToLast(ITerminal terminal)
        {
            var index = terminal.CursorPosition;
            var command = terminal.Command;
            terminal.Command = command.Substring(0, index);
        }

        private static void DeleteToFirst(ITerminal terminal)
        {
            var index = terminal.CursorPosition;
            var command = terminal.Command;
            terminal.Command = command.Remove(0, index);
            terminal.CursorPosition = 0;
        }

        private static void DeletePrevWord(ITerminal terminal)
        {
            var index2 = terminal.CursorPosition;
            var command = terminal.Command;
            var index1 = PrevWord(terminal);
            var length = index2 - index1;
            terminal.Command = command.Remove(index1, length);
        }
    }
}
