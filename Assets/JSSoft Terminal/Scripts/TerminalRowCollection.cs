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

namespace JSSoft.UI
{
    class TerminalRowCollection : List<TerminalRow>
    {
        private readonly TerminalGrid grid;
        private readonly Stack<TerminalRow> pool = new Stack<TerminalRow>();
        private TMP_FontAsset fontAsset;
        private string text = string.Empty;
        private int columnCount;

        public TerminalRowCollection(TerminalGrid grid)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public void Udpate(TerminalPointCollection points)
        {
            var fontAsset = this.grid.FontAsset;
            var text = this.grid.Text + char.MinValue;
            var columnCount = this.grid.ColumnCount;

            if (fontAsset != null && this.text != text)
            {
                var volume = points.Volume;
                var index = 0;//this.FindUpdateIndex(fontAsset, text, columnCount);
                this.Resize(this.grid.ColumnCount, volume.Bottom);
                for (var i = index; i < text.Length; i++)
                {
                    var point = points[i];
                    var character = text[i];
                    if (point.X < columnCount)
                    {
                        var cell = this.Prepare(point);
                        cell.Character = character;
                    }
                }
            }

            this.fontAsset = fontAsset;
            this.text = text;
            this.columnCount = columnCount;
        }

        public TerminalCell Prepare(TerminalPoint point)
        {
            if (point.Y >= this.Count)
            {
                var row = this.pool.Any() == true ? this.pool.Pop() : new TerminalRow(this.grid, this.Count);
                this.Add(row);
            }

            return this[point.Y].Cells[point.X];
        }

        private void Resize(int columnCount, int rowCount)
        {
            for (var i = this.Count - 1; i >= rowCount; i--)
            {
                this.pool.Push(this[i]);
                this.RemoveAt(i);
            }
            for (var i = this.Count; i < rowCount; i++)
            {
                var item = this.pool.Any() ? this.pool.Pop() : new TerminalRow(this.grid, i);
                this.Add(item);
            }
            for (var i = this.Count - 1; i >= 0; i--)
            {
                var row = this[i];
                row.Resize(columnCount);
            }
        }

        private int FindUpdateIndex(TMP_FontAsset fontAsset, string text, int columnCount)
        {
            if (this.fontAsset != fontAsset || this.columnCount != columnCount)
                return 0;
            return GetIndex(this.text, text);
        }

        public static int GetIndex(string text1, string text2)
        {
            var oldValue = text1 ?? throw new ArgumentNullException(nameof(text1));
            var newValue = text2 ?? throw new ArgumentNullException(nameof(text2));
            var minLength = Math.Min(newValue.Length, oldValue.Length);
            var index = minLength;

            for (var i = 0; i < minLength; i++)
            {
                if (newValue[i] != oldValue[i])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
