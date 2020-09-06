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
using UnityEngine;

namespace JSSoft.Terminal
{
    public class KeyBinding<T> : KeyBindingBase<T> where T : class
    {
        private readonly Func<T, bool> action;
        private readonly Func<T, bool> verify;

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<T, bool> action)
            : this(modifiers, key, action, (obj) => true)
        {
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<T> action)
            : this(modifiers, key, action, (obj) => true)
        {
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<T> action, Func<T, bool> verify)
            : base(modifiers, key)
        {
            this.action = (t) =>
            {
                action(t);
                return true;
            };
            this.verify = verify;
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<T, bool> action, Func<T, bool> verify)
            : base(modifiers, key)
        {
            this.action = action;
            this.verify = verify;
        }

        protected override bool OnVerify(T obj)
        {
            return this.verify(obj);
        }

        protected override bool OnAction(T obj)
        {
            return this.action(obj);
        }
    }
}
