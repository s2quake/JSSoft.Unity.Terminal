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
    public static class TerminalGridUtility
    {
        public static readonly TerminalThickness DefaultPadding = new TerminalThickness(2);

        public static Rect TransformRect(ITerminalGrid grid, Rect rect)
        {
            return TransformRect(grid, rect, false);
        }

        public static Rect TransformRect(ITerminalGrid grid, Rect rect, bool scroll)
        {
            if (grid != null && grid.Font != null && grid.Rows.Count > 0)
            {
                var font = grid.Font;
                var itemHeight = font.Height;
                if (scroll == true)
                    rect.y += itemHeight * grid.VisibleIndex;
                rect.x += grid.Rectangle.x;
                rect.y -= grid.Rectangle.y;
            }
            return rect;
        }

        public static IEnumerable<ITerminalCell> GetVisibleCells(ITerminalGrid grid, Func<ITerminalCell, bool> predicate)
        {
            if (grid != null)
            {
                var topIndex = grid.VisibleIndex;
                var bottomIndex = topIndex + grid.BufferHeight;
                for (var i = topIndex; i < bottomIndex; i++)
                {
                    var row = grid.Rows[i];
                    foreach (var item in row.Cells)
                    {
                        if (predicate(item) == true)
                            yield return item;
                    }
                }
            }
        }

        public static ITerminalCell GetCell(ITerminalGrid grid, int cursorLeft, int cursorTop)
        {
            if (grid != null)
            {
                if (cursorTop >= grid.Rows.Count)
                    throw new ArgumentOutOfRangeException(nameof(cursorTop));
                if (cursorLeft >= grid.BufferWidth)
                    throw new ArgumentOutOfRangeException(nameof(cursorLeft));
                return grid.Rows[cursorTop].Cells[cursorLeft];
            }
            return null;
        }

        public static bool IsSelecting(ITerminalGrid grid, ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var point = new TerminalPoint(cell.Index, cell.Row.Index);
            var isSelecting = IsSelecting(grid, point);
            return IsSelected(grid, point) != isSelecting;
        }

        public static bool IsSelecting(ITerminalGrid grid, TerminalPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grid.SelectingRange != TerminalRange.Empty)
            {
                return grid.SelectingRange.Intersect(point);
            }
            return false;
        }

        public static bool IsSelected(ITerminalGrid grid, TerminalPoint point)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grid.Selections.Any())
            {
                foreach (var item in grid.Selections)
                {
                    var p1 = item.BeginPoint;
                    var p2 = item.EndPoint;
                    if (point >= p1 && point < p2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int GetItemWidth(ITerminalGrid grid)
        {
            if (grid != null && grid.Font is TerminalFont font)
            {
                return font.Width;
            }
            return FontUtility.DefaultItemWidth;
        }

        public static int GetItemHeight(ITerminalGrid grid)
        {
            if (grid != null && grid.Font is TerminalFont font)
            {
                return font.Height;
            }
            return FontUtility.DefaultItemHeight;
        }

        public static TerminalThickness GetPadding(ITerminalGrid grid)
        {
            if (grid != null)
            {
                return grid.Padding;
            }
            return DefaultPadding;
        }

        public static GlyphRect GetCellRect(ITerminalGrid grid, ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            var itemWidth = TerminalGridUtility.GetItemWidth(grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(grid);
            var x = cell.Index * itemWidth;
            var y = cell.Row.Index * itemHeight;
            return new GlyphRect(x, y, itemWidth, itemHeight);
        }

        public static GlyphRect GetGlyphRect(ITerminalGrid grid, TerminalPoint point)
        {
            return GetGlyphRect(grid, point.X, point.Y);
        }

        public static GlyphRect GetGlyphRect(ITerminalGrid grid, int x, int y)
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(grid);
            var padding = TerminalGridUtility.GetPadding(grid);
            var x1 = x * itemWidth + padding.Left;
            var y1 = y * itemHeight + padding.Top;
            return new GlyphRect(x1, y1, itemWidth, itemHeight);
        }

        public static Color GetSelectionColor(ITerminalGrid grid)
        {
            return grid != null ? grid.SelectionColor : TerminalGrid.DefaultSelectionColor;
        }

        public static Color GetCursorColor(ITerminalGrid grid)
        {
            return grid != null ? grid.CursorColor : TerminalGrid.DefaultCursorColor;
        }

        public static Color32 GetBackgroundColor(ITerminalGrid grid)
        {
            if (grid == null)
                return TerminalGrid.DefaultBackgroundColor;
            return grid.BackgroundColor;
        }

        public static Color32 GetForegroundColor(ITerminalGrid grid)
        {
            if (grid == null)
                return TerminalGrid.DefaultForegroundColor;
            return grid.ForegroundColor;
        }
    }
}
