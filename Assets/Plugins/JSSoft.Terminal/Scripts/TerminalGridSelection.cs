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
using System.Collections.ObjectModel;

namespace JSSoft.UI
{
    class TerminalGridSelection : ObservableCollection<TerminalRange>
    {
        private readonly TerminalGrid grid;

        public TerminalGridSelection(TerminalGrid grid)
        {
            this.grid = grid;
        }

        protected override void ClearItems()
        {
            if (this.Count > 0)
                base.ClearItems();
        }

        protected override void InsertItem(int index, TerminalRange item)
        {
            if (item == TerminalRange.Empty)
                throw new ArgumentException("invalid selection", nameof(item));
            if (this.Contains(item) == true)
                throw new ArgumentException("already exists.");
            base.InsertItem(index, item);
        }
    }
}
