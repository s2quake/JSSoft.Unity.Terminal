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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;
using System.Collections;

namespace JSSoft.UI
{
    class TerminalEventCollection : IList<Event>, IReadOnlyList<Event>
    {
        private readonly List<Event> itemList = new List<Event>();
        private Stack<Event> pool = new Stack<Event>();
        private int count;

        public bool PopEvents()
        {
            var eventCount = Event.GetEventCount();
            if (eventCount > 0)
            {
                if (this.itemList.Capacity < eventCount)
                {
                    this.itemList.Capacity = eventCount;
                }
                for (var i = this.itemList.Count; i < eventCount; i++)
                {
                    this.itemList.Add(null);
                }
                for (var i = 0; i < eventCount; i++)
                {
                    if (this.itemList[i] == null)
                    {
                        var item = this.pool.Any() == true ? this.pool.Pop() : new Event();
                        this.itemList[i] = item;
                    }
                    Event.PopEvent(this.itemList[i]);
                }
            }
            this.count = eventCount;
            return this.count > 0;
        }

        public int IndexOf(Event item)
        {
            for (var i = 0; i < this.count; i++)
            {
                if (this.itemList[i] == item)
                    return i;
            }
            return -1;
        }

        public void Insert(int index, Event item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (this.itemList.Contains(item) == true)
                throw new ArgumentException("item already exist.");
            if (index < 0 || index > this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.itemList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            var item = this.itemList[index];
            this.pool.Push(item);
            this.count--;
        }

        public void Add(Event item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (this.itemList.Contains(item) == true)
                throw new ArgumentException("item already exist.");
            this.itemList.Insert(this.count, item);
            this.count++;
        }

        public void Clear()
        {
            this.itemList.Clear();
            this.count = 0;
        }

        public bool Contains(Event item)
        {
            for (var i = 0; i < this.count; i++)
            {
                if (this.itemList[i] == item)
                    return true;
            }
            return false;
        }

        public void CopyTo(Event[] array, int arrayIndex)
        {
            for (var i = 0; i < this.count; i++)
            {
                array[i + arrayIndex] = this.itemList[i];
            }
        }

        public bool Remove(Event item)
        {
            for (var i = 0; i < this.count; i++)
            {
                if (this.itemList[i] == item)
                {
                    this.itemList.RemoveAt(i);
                    this.count--;
                    return true;
                }
            }
            return false;
        }

        public int Count => this.count;

        public bool IsReadOnly => false;

        public Event this[int index]
        {
            get
            {
                if (index < 0 || index >= this.count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return this.itemList[index];
            }
            set
            {
                if (index < 0 || index >= this.count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                var oldItem = this.itemList[index];
                if (oldItem == value)
                    return;
                this.itemList[index] = value;
                if (oldItem != null)
                {
                    this.pool.Push(oldItem);
                }
            }
        }

        #region IEnumerable

        IEnumerator<Event> IEnumerable<Event>.GetEnumerator()
        {
            for (var i = 0; i < this.count; i++)
            {
                yield return this.itemList[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < this.count; i++)
            {
                yield return this.itemList[i];
            }
        }

        #endregion
    }
}
