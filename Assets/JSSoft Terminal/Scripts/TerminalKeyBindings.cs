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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KeyBinding = JSSoft.UI.KeyBinding<JSSoft.UI.Terminal>;

namespace JSSoft.UI
{
    public static class TerminalKeyBindings
    {
        public static readonly KeyBindingCollection Common = new KeyBindingCollection()
        {
            new KeyBinding(EventModifiers.None, KeyCode.UpArrow, (t) => t.PrevHistory()),
            new KeyBinding(EventModifiers.None, KeyCode.DownArrow, (t) => t.NextHistory()),
            new KeyBinding(EventModifiers.None, KeyCode.LeftArrow, (t) => t.CursorPosition--),
            new KeyBinding(EventModifiers.None, KeyCode.RightArrow, (t) => t.CursorPosition++),
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
            new KeyBinding(EventModifiers.None, KeyCode.Backspace, (t) => t.Backspace()),
            // ime 입력중에 Backspace 키를 누르면 두번이 호출됨 그중 처음에는 EventModifiers.None + KeyCode.Backspace 가 호출됨.
            new KeyBinding(EventModifiers.None, KeyCode.Backspace, (t) => true),
            new KeyBinding(EventModifiers.None, KeyCode.Delete, (t) => t.Delete()),
            new KeyBinding(EventModifiers.None, KeyCode.Tab, (t) => t.NextCompletion()),
            new KeyBinding(EventModifiers.Shift, KeyCode.Tab, (t) => t.PrevCompletion())
        };

        public static readonly KeyBindingCollection Mac = new KeyBindingCollection(Common)
        {
            new KeyBinding(EventModifiers.Control, KeyCode.U, (t) => t.Clear()),
            new KeyBinding(EventModifiers.Command | EventModifiers.None, KeyCode.LeftArrow, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.Command | EventModifiers.None, KeyCode.RightArrow, (t) => t.MoveToLast()),
        };

        public static readonly KeyBindingCollection Windows = new KeyBindingCollection(Common)
        {
            new KeyBinding(EventModifiers.None, KeyCode.Escape, (t) => t.Clear()),
            new KeyBinding(EventModifiers.None, KeyCode.Home, (t) => t.MoveToFirst()),
            new KeyBinding(EventModifiers.None, KeyCode.End, (t) => t.MoveToLast()),
        };
    }
}
