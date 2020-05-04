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
using UnityEngine;

namespace JSSoft.UI
{
    class TerminalGridSelection : IList<TerminalRange>
    {
        private readonly List<TerminalRange> itemList = new List<TerminalRange>();
        private readonly Action action;

        public TerminalGridSelection(Action action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public TerminalRange this[int index]
        {
            get => this.itemList[index];
            set => this.itemList[index] = value;
        }

        public int Count => this.itemList.Count;

        public void Add(TerminalRange item)
        {
            this.itemList.Add(item);
            this.InvokeSelectionChangedEvent();
        }

        public void Clear()
        {
            if (this.itemList.Any() == true)
            {
                this.itemList.Clear();
                this.InvokeSelectionChangedEvent();
            }
        }

        public bool Contains(TerminalRange item)
        {
            return this.itemList.Contains(item);
        }

        public void CopyTo(TerminalRange[] array, int arrayIndex)
        {
            this.itemList.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TerminalRange item)
        {
            return this.IndexOf(item);
        }

        public void Insert(int index, TerminalRange item)
        {
            this.itemList.Insert(index, item);
            this.InvokeSelectionChangedEvent();
        }

        public bool Remove(TerminalRange item)
        {
            var result = this.itemList.Remove(item);
            this.InvokeSelectionChangedEvent();
            return result;
        }

        public void RemoveAt(int index)
        {
            this.itemList.RemoveAt(index);
            this.InvokeSelectionChangedEvent();
        }

        private void InvokeSelectionChangedEvent() => this.action();

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item;
            }
        }

        IEnumerator<TerminalRange> IEnumerable<TerminalRange>.GetEnumerator()
        {
            foreach (var item in this.itemList)
            {
                yield return item;
            }
        }

        bool ICollection<TerminalRange>.IsReadOnly
        {
            get
            {
                if (this.itemList is ICollection<TerminalRange> collections)
                {
                    return collections.IsReadOnly;
                }
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
