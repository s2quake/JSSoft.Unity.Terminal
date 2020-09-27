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

namespace JSSoft.Unity.Terminal
{
    class PropertyNotifier : IDisposable
    {
        private readonly Action<string> action;
        private readonly List<string> propertyList = new List<string>();

        public PropertyNotifier(Action<string> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Begin()
        {
            this.Begin(0);
        }

        public void Begin(int capacity)
        {
            this.propertyList.Clear();
            if (capacity > this.propertyList.Capacity)
            {
                this.propertyList.Capacity = capacity;
            }
        }

        public void SetField<T>(ref T field, T value, string propertyName)
        {
            if (object.Equals(field, value) == false)
            {
                field = value;
                this.propertyList.Add(propertyName);
            }
        }

        public void End()
        {
            foreach (var item in this.propertyList)
            {
                this.action(item);
            }
            this.propertyList.Clear();
        }

        public void Dispose()
        {
            this.End();
        }
    }
}