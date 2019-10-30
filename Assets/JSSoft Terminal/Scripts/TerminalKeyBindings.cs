﻿// MIT License
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

using UnityEngine;
using KeyBinding = JSSoft.UI.KeyBinding<JSSoft.UI.Terminal>;

namespace JSSoft.UI
{
    public static class TerminalKeyBindings
    {
        public static readonly KeyBindingCollection Common = new KeyBindingCollection()
        {
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.UpArrow, (t) => t.PrevHistory()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.DownArrow, (t) => t.NextHistory()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.LeftArrow, (t) => t.CursorPosition--),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.RightArrow, (t) => t.CursorPosition++),
            new KeyBinding(EventModifiers.Shift, KeyCode.LeftArrow, (t) => true),
            new KeyBinding(EventModifiers.Shift, KeyCode.RightArrow, (t) => true),
            new KeyBinding(EventModifiers.Shift, KeyCode.UpArrow, (t) => true),
            new KeyBinding(EventModifiers.Shift, KeyCode.DownArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.LeftArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.RightArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.UpArrow, (t) => true),
            new KeyBinding(EventModifiers.Control, KeyCode.DownArrow, (t) => true),
            new KeyBinding(EventModifiers.None, KeyCode.Return, (t) => t.Execute(), (t) => t.IsReadOnly == false),
            new KeyBinding(EventModifiers.None, KeyCode.KeypadEnter, (t) => t.Execute(), (t) => t.IsReadOnly == false),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.Backspace, (t) => t.Backspace()),
            // ime 입력중에 Backspace 키를 누르면 두번이 호출됨 그중 처음에는 EventModifiers.FunctionKey + KeyCode.Backspace 가 호출됨.
            new KeyBinding(EventModifiers.None, KeyCode.Backspace, (t) => true),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.Delete, (t) => t.Delete()),
            new KeyBinding(EventModifiers.None, KeyCode.Tab, (t) => t.NextCompletion()),
            new KeyBinding(EventModifiers.Shift, KeyCode.Tab, (t) => t.PrevCompletion())
        };

        public static readonly KeyBindingCollection Mac = new KeyBindingCollection(Common)
        {
            new KeyBinding(EventModifiers.Control, KeyCode.U, (t) => DeleteToFirst(t)),
            new KeyBinding(EventModifiers.Control, KeyCode.K, (t) => DeleteToLast(t)),
            new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.LeftArrow, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.Command | EventModifiers.FunctionKey, KeyCode.RightArrow, (t) => t.MoveToLast()),
            new KeyBinding(EventModifiers.Control, KeyCode.E, (t) => t.MoveToLast()),
            new KeyBinding(EventModifiers.Control, KeyCode.A, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.Control, KeyCode.W, (t) => DeletePrevWord(t)),
            new KeyBinding(EventModifiers.Alt | EventModifiers.FunctionKey, KeyCode.LeftArrow, (t) => PrevWord(t)),
            new KeyBinding(EventModifiers.Alt | EventModifiers.FunctionKey, KeyCode.RightArrow, (t) => NextWord(t)),
        };

        public static readonly KeyBindingCollection Windows = new KeyBindingCollection(Common)
        {
            new KeyBinding(EventModifiers.None, KeyCode.Escape, (t) => t.Clear()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.Home, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.End, (t) => t.MoveToLast()),
        };

        private static int PrevWord(Terminal terminal)
        {
            var index = terminal.CursorPosition - 1;
            var command = terminal.Command;
            while (index >= 0)
            {
                var ch = command[index];
                if (char.IsLetterOrDigit(ch) == true)
                    break;
                index--;
            }
            while (index >= 0)
            {
                var ch = command[index];
                if (char.IsLetterOrDigit(ch) == false)
                    break;
                index--;
            }
            index++;
            terminal.CursorPosition = index;
            return terminal.CursorPosition;
        }

        private static int NextWord(Terminal terminal)
        {
            var index = terminal.CursorPosition;
            var command = terminal.Command;
            while (index < command.Length)
            {
                var ch = command[index];
                if (char.IsLetterOrDigit(ch) == true)
                    break;
                index++;
            }
            while (index < command.Length)
            {
                var ch = command[index];
                if (char.IsLetterOrDigit(ch) == false)
                    break;
                index++;
            }
            terminal.CursorPosition = index;
            return terminal.CursorPosition;
        }

        private static void DeleteToLast(Terminal terminal)
        {
            var index = terminal.CursorPosition;
            var command = terminal.Command;
            terminal.Command = command.Substring(0, index);
        }

        private static void DeleteToFirst(Terminal terminal)
        {
            var index = terminal.CursorPosition;
            var command = terminal.Command;
            terminal.Command = command.Remove(0, index);
            terminal.CursorPosition = 0;
        }

        private static void DeletePrevWord(Terminal terminal)
        {
            var index2 = terminal.CursorPosition;
            var command = terminal.Command;
            var index1 = PrevWord(terminal);
            var length = index2 - index1;
            terminal.Command = command.Remove(index1, length);
        }
    }
}
