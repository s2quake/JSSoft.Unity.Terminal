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
    public class TerminalRow
    {
        private readonly List<TerminalCell> cells = new List<TerminalCell>();
        private readonly Stack<TerminalCell> pool = new Stack<TerminalCell>();
        private Color32? backgroundColor;
        private Color32? foregroundColor;

        public TerminalRow(TerminalGrid grid, int index)
        {
            this.Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.Index = index;
            this.cells.Capacity = grid.ColumnCount;
            for (var i = 0; i < grid.ColumnCount; i++)
            {
                this.cells.Add(new TerminalCell(this, i));
            }
            this.UpdateRect();
        }

        public static Color32 GetBackgroundColor(TerminalRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return row.BackgroundColor ?? TerminalGrid.GetBackgroundColor(row.Grid);
        }

        public static Color32 GetForegroundColor(TerminalRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return row.ForegroundColor ?? TerminalGrid.GetForegroundColor(row.Grid);
        }

        public TerminalCell Intersect(Vector2 position)
        {
            if (this.Rect.Intersect(position) == false)
                return null;
            foreach (var item in this.cells)
            {
                if (item.Intersect(position) == true)
                    return item;
            }
            throw new NotImplementedException();
        }

        public TerminalGrid Grid { get; }

        public int Index { get; }

        public IReadOnlyList<TerminalCell> Cells => this.cells;

        public bool IsSelected { get; set; }

        public GlyphRect Rect { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        // public void Cut(int x)
        // {
        //     for (var i = x; i < this.cells.Count; i++)
        //     {
        //         var item = this.cells[i];
        //         item.Character = char.MinValue;
        //         item.BackgroundColor = null;
        //         item.ForegroundColor = null;
        //     }
        // }

        // public void Reset()
        // {
        //     this.Cut(0);
        // }

        public void Resize()
        {
            for (var i = this.cells.Count - 1; i >= this.Grid.ColumnCount; i--)
            {
                this.pool.Push(this.cells[i]);
                this.cells.RemoveAt(i);
            }
            for (var i = this.cells.Count; i < this.Grid.ColumnCount; i++)
            {
                var item = this.pool.Any() ? this.pool.Pop() : new TerminalCell(this, i);
                this.cells.Add(item);
            }
            for (var i = 0; i < this.cells.Count; i++)
            {
                var item = this.cells[i];
                item.Character = char.MinValue;
                item.BackgroundColor = null;
                item.ForegroundColor = null;
            }
        }

        private void UpdateRect()
        {
            var itemWidth = TerminalGrid.GetItemWidth(this.Grid);
            var itemHeight = TerminalGrid.GetItemHeight(this.Grid);
            var x = 0;
            var y = this.Index * itemHeight;
            var width = this.cells.Count * itemWidth;
            var height = itemHeight;
            this.Rect = new GlyphRect(x, y, width, height);
        }

        private IEnumerable<TerminalCell> ValidCells => this.Cells.Where(item => item.Character != 0);
    }
}
