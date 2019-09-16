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
using UnityEngine;

namespace JSSoft.UI
{
    public class KeyBinding : KeyBindingBase<Terminal>
    {
        private readonly Func<Terminal, bool> action;
        private readonly Func<Terminal, bool> verify;

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<Terminal, bool> action)
            : this(modifiers, key, action, (obj) => true)
        {

        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<Terminal> action)
            : this(modifiers, key, action, (obj) => true)
        {

        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<Terminal> action, Func<Terminal, bool> verify)
            : base(modifiers, key)
        {
            this.action = (t) =>
            {
                action(t);
                return true;
            };
            this.verify = verify;
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<Terminal, bool> action, Func<Terminal, bool> verify)
            : base(modifiers, key)
        {
            this.action = action;
            this.verify = verify;
        }

        protected override bool OnVerify(Terminal obj)
        {
            return this.verify(obj);
        }

        protected override bool OnAction(Terminal obj)
        {
            return this.action(obj);
        }
    }
}
