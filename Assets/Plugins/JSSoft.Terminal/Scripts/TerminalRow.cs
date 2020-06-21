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
using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.Terminal
{
    class TerminalRow : ITerminalRow
    {
        private readonly List<TerminalCell> cells = new List<TerminalCell>();
        private readonly Stack<TerminalCell> pool = new Stack<TerminalCell>();
        private TerminalRowAttributes attributes;

        public TerminalRow(TerminalGrid grid, int index)
        {
            this.Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.Index = index;
            this.cells.Capacity = grid.BufferWidth;
            for (var i = 0; i < grid.BufferWidth; i++)
            {
                this.cells.Add(new TerminalCell(this, i));
            }
            this.UpdateRect();
        }

        public static Color32 GetBackgroundColor(ITerminalRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return row.BackgroundColor ?? TerminalGridUtility.GetBackgroundColor(row.Grid);
        }

        public static Color32 GetForegroundColor(ITerminalRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return row.ForegroundColor ?? TerminalGridUtility.GetForegroundColor(row.Grid);
        }

        public TerminalPoint Intersect(Vector2 position)
        {
            if (this.Rect.Intersect(position) == true)
            {
                foreach (var item in this.cells)
                {
                    if (item.Intersect(position) == true)
                        return item.Point;
                }
                return new TerminalPoint(this.Grid.BufferWidth, this.Index);
            }
            return TerminalPoint.Invalid;
        }

        public TerminalCell IntersectWithCell(Vector2 position)
        {
            if (this.Rect.Intersect(position) == false)
                return null;
            foreach (var item in this.cells)
            {
                if (item.Intersect(position) == true)
                    return item;
            }
            return null;
        }

        public void Reset()
        {
            this.ResetAfter(0);
        }

        public void ResetAfter(int index)
        {
            for (var i = index; i < this.cells.Count; i++)
            {
                var item = this.cells[i];
                item.Reset();
            }
        }

        public void Resize(int bufferWidth)
        {
            for (var i = this.cells.Count - 1; i >= bufferWidth; i--)
            {
                this.pool.Push(this.cells[i]);
                this.cells.RemoveAt(i);
            }
            for (var i = this.cells.Count; i < bufferWidth; i++)
            {
                var item = this.pool.Any() ? this.pool.Pop() : new TerminalCell(this, i);
                item.Reset();
                this.cells.Add(item);
            }
            this.UpdateRect();
        }

        public TerminalGrid Grid { get; }

        public int Index { get; }

        public IReadOnlyList<TerminalCell> Cells => this.cells;

        public TerminalRowAttributes Attributes => this.attributes;

        public bool IsSelected
        {
            get => this.attributes.HasFlag(TerminalRowAttributes.IsSelected);
            private set
            {
                if (value == true)
                    this.attributes |= TerminalRowAttributes.IsSelected;
                else
                    this.attributes &= ~TerminalRowAttributes.IsSelected;
            }
        }

        public bool IsMultiline
        {
            get => this.attributes.HasFlag(TerminalRowAttributes.Multiline);
            set
            {
                if (value == true)
                    this.attributes |= TerminalRowAttributes.Multiline;
                else
                    this.attributes &= ~TerminalRowAttributes.Multiline;
            }
        }

        public GlyphRect Rect { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        private void UpdateRect()
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(this.Grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(this.Grid);
            var padding = TerminalGridUtility.GetPadding(this.Grid);
            var x = 0 + padding.Left;
            var y = this.Index * itemHeight + padding.Top;
            var width = this.cells.Count * itemWidth;
            var height = itemHeight;
            this.Rect = new GlyphRect(x, y, width, height);
        }

        #region ITerminalRow

        ITerminalCell ITerminalRow.Intersect(Vector2 position)
        {
            return this.IntersectWithCell(position);
        }

        IReadOnlyList<ITerminalCell> ITerminalRow.Cells => this.cells;

        ITerminalGrid ITerminalRow.Grid => this.Grid;

        #endregion
    }
}
