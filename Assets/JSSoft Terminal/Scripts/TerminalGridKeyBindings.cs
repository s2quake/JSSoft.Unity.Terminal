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

using UnityEngine;
using KeyBinding = JSSoft.UI.KeyBinding<JSSoft.UI.TerminalGrid>;

namespace JSSoft.UI
{
    public static class TerminalGridKeyBindings
    {
        public static readonly KeyBindingCollection Common = new KeyBindingCollection()
        {
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageUp, (g) => g.PageUp()),
            new KeyBinding(EventModifiers.FunctionKey, KeyCode.PageDown, (g) => g.PageDown())
        };

        public static readonly KeyBindingCollection Mac = new KeyBindingCollection(Common)
        {
            new KeyBinding(EventModifiers.Alt | EventModifiers.Command, KeyCode.PageUp, (g) => g.LineUp()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Alt | EventModifiers.Command, KeyCode.PageDown, (g) => g.LineDown()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Command, KeyCode.Home, (g) => g.ScrollToTop()),
            new KeyBinding(EventModifiers.FunctionKey | EventModifiers.Command, KeyCode.End, (g) => g.ScrollToBottom()),
            new KeyBinding(EventModifiers.Command, KeyCode.C, (g) => g.Copy()),
            new KeyBinding(EventModifiers.Command, KeyCode.A, (g) => g.SelectAll()),
        };

        public static readonly KeyBindingCollection Windows = new KeyBindingCollection(Common)
        {

        };
    }
}
