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

namespace JSSoft.UI
{
    class TerminalRow : ITerminalRow
    {
        private readonly List<TerminalCell> cells = new List<TerminalCell>();
        private readonly Stack<TerminalCell> pool = new Stack<TerminalCell>();
        private bool isSelected;
        private bool isEmpty;

        public TerminalRow(TerminalGrid grid, int index)
        {
            this.Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.Index = index;
            this.cells.Capacity = grid.ColumnCount;
            for (var i = 0; i < grid.ColumnCount; i++)
            {
                this.cells.Add(new TerminalCell(this, i, () => this.IsModified = true));
            }
            this.UpdateRect();
            this.IsModified = false;
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
                return new TerminalPoint(this.Grid.ColumnCount, this.Index);
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

        public TerminalPoint LastPoint(bool isCursor)
        {
            var columnCount = this.Grid.ColumnCount;
            var index = this.Index;
            var point = new TerminalPoint(columnCount, index);
            if (this.IsEmpty == false)
            {
                for (var i = columnCount - 1; i >= 0; i--)
                {
                    var item = this.Cells[i];
                    if (item.IsEnabled == true)
                    {
                        point.X = i;
                        if (isCursor)
                            point.X++;
                        break;
                    }
                }
            }
            return point;
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

        public void Resize(int columnCount)
        {
            for (var i = this.cells.Count - 1; i >= columnCount; i--)
            {
                this.pool.Push(this.cells[i]);
                this.cells.RemoveAt(i);
            }
            for (var i = this.cells.Count; i < columnCount; i++)
            {
                var item = this.pool.Any() ? this.pool.Pop() : new TerminalCell(this, i, () => this.IsModified = true);
                item.Reset();
                this.cells.Add(item);
            }
            this.UpdateRect();
        }

        public TerminalGrid Grid { get; }

        public int Index { get; }

        public IReadOnlyList<TerminalCell> Cells => this.cells;

        public bool IsSelected
        {
            get
            {
                this.UpdateFlag();
                return this.isSelected;
            }
        }

        public bool IsEmpty
        {
            get
            {
                this.UpdateFlag();
                return this.isEmpty;
            }
        }

        public GlyphRect Rect { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public bool IsModified { get; private set; }

        private void UpdateRect()
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(this.Grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(this.Grid);
            var x = 0;
            var y = this.Index * itemHeight;
            var width = this.cells.Count * itemWidth;
            var height = itemHeight;
            this.Rect = new GlyphRect(x, y, width, height);
        }

        private void UpdateFlag()
        {
            if (this.IsModified == true)
            {
                this.isSelected = this.cells.Any(item => item.IsSelected);
                this.isEmpty = this.cells.Any(item => item.Character != char.MinValue) == false;
            }
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
