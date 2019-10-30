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
        public static Rect TransformRect(ITerminalGrid grid, Rect rect)
        {
            return TransformRect(grid, rect, false);
        }

        public static Rect TransformRect(ITerminalGrid grid, Rect rect, bool scroll)
        {
            if (grid != null && grid.FontAsset != null && grid.Rows.Count > 0)
            {
                var itemHeight = FontUtility.GetItemHeight(grid.FontAsset);
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
                var bottomIndex = topIndex + grid.RowCount;
                var query = from row in grid.Rows
                            where row.Index >= topIndex && row.Index < bottomIndex
                            from cell in row.Cells
                            where predicate(cell)
                            select cell;
                return query;
            }
            return Enumerable.Empty<ITerminalCell>();
        }

        public static ITerminalCell GetCell(ITerminalGrid grid, int cursorLeft, int cursorTop)
        {
            if (grid != null)
            {
                if (cursorTop >= grid.Rows.Count)
                    throw new ArgumentOutOfRangeException(nameof(cursorTop));
                if (cursorLeft >= grid.ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(cursorLeft));
                return grid.Rows[cursorTop].Cells[cursorLeft];
            }
            return null;
        }

        public static int GetItemWidth(ITerminalGrid grid)
        {
            if (grid != null && grid.FontAsset != null)
            {
                return FontUtility.GetItemWidth(grid.FontAsset);
            }
            return 0;
        }

        public static int GetItemHeight(ITerminalGrid grid)
        {
            if (grid != null && grid.FontAsset != null)
            {
                return FontUtility.GetItemHeight(grid.FontAsset);
            }
            return 0;
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

        public static int GetItemWidth(ITerminalGrid grid, char character)
        {
            if (grid != null && grid.FontAsset != null)
            {
                return FontUtility.GetItemWidth(grid.FontAsset, character);
            }
            return 0;
        }

        public static Color32 GetSelectionColor(ITerminalGrid grid)
        {
            return grid != null ? grid.SelectionColor : TerminalGrid.DefaultSelectionColor;
        }

        public static Color32 GetCursorColor(ITerminalGrid grid)
        {
            return grid != null ? grid.CursorColor : TerminalGrid.DefaultCursorColor;
        }

        public static Color32 GetBackgroundColor(ITerminalGrid grid)
        {
            if (grid == null)
                return TerminalGrid.DefaultBackgroundColor;
            return grid.BackgroundColor ?? TerminalGrid.DefaultBackgroundColor;
        }

        public static Color32 GetForegroundColor(ITerminalGrid grid)
        {
            if (grid == null)
                return TerminalGrid.DefaultForegroundColor;
            return grid.ForegroundColor ?? TerminalGrid.DefaultForegroundColor;
        }
    }
}
