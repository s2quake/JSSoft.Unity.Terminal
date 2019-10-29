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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSSoft.UI
{
    public class KeyBindingCollection : IEnumerable<IKeyBinding>
    {
        private readonly Dictionary<string, IKeyBinding> itemByKey = new Dictionary<string, IKeyBinding>();

        public KeyBindingCollection(KeyBindingCollection bindings)
        {
            this.BaseBindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
        }

        public KeyBindingCollection()
        {

        }

        public void Add(IKeyBinding item)
        {
            var key = $"{item.Modifiers}+{item.KeyCode}";
            this.itemByKey.Add(key, item);
        }

        public bool Process(object obj, EventModifiers modifiers, KeyCode keyCode)
        {
            var key = $"{modifiers}+{keyCode}";
            if (this.itemByKey.ContainsKey(key) == true)
            {
                var binding = this.itemByKey[key];
                if (binding.Verify(obj) == true && binding.Action(obj) == true)
                    return true;
            }
            if (this.BaseBindings != null && this.BaseBindings.Process(obj, modifiers, keyCode) == true)
            {
                return true;
            }
            return false;
        }

        public int Count => this.itemByKey.Count;

        public KeyBindingCollection BaseBindings { get; }

        #region IEnumerable

        IEnumerator<IKeyBinding> IEnumerable<IKeyBinding>.GetEnumerator()
        {
            foreach (var item in this.itemByKey)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.itemByKey)
            {
                yield return item.Value;
            }
        }

        #endregion
    }
}
