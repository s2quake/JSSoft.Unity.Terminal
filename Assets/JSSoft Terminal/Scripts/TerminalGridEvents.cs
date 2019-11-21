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

namespace JSSoft.UI
{
    public static class TerminalGridEvents
    {
        private static readonly HashSet<ITerminalGrid> grids = new HashSet<ITerminalGrid>();

        public static void Register(ITerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grids.Contains(grid) == true)
                throw new ArgumentException("grid is already exists.");
            grids.Add(grid);
            grid.TextChanged += Grid_TextChanged;
            grid.VisibleIndexChanged += Grid_VisibleIndexChanged;
            grid.LayoutChanged += Grid_LayoutChanged;
            grid.SelectionChanged += Grid_SelectionChanged;
            grid.CursorPointChanged += Grid_CursorPointChanged;
            grid.CompositionStringChanged += Grid_CompositionStringChanged;
            grid.GotFocus += Grid_GotFocus;
            grid.LostFocus += Grid_LostFocus;
            grid.Validated += Grid_Validated;
        }

        public static void Unregister(ITerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grids.Contains(grid) == false)
                throw new ArgumentException("grid does not exists.");
            grid.Validated -= Grid_Validated;
            grid.TextChanged -= Grid_TextChanged;
            grid.VisibleIndexChanged -= Grid_VisibleIndexChanged;
            grid.LayoutChanged -= Grid_LayoutChanged;
            grid.SelectionChanged -= Grid_SelectionChanged;
            grid.CursorPointChanged -= Grid_CursorPointChanged;
            grid.CompositionStringChanged -= Grid_CompositionStringChanged;
            grid.GotFocus -= Grid_GotFocus;
            grid.LostFocus -= Grid_LostFocus;
            grids.Remove(grid);
        }

        public static event EventHandler TextChanged;

        public static event EventHandler VisibleIndexChanged;

        public static event EventHandler LayoutChanged;

        public static event EventHandler SelectionChanged;

        public static event EventHandler CursorPointChanged;

        public static event EventHandler CompositionStringChanged;

        public static event EventHandler GotFocus;

        public static event EventHandler LostFocus;

        public static event EventHandler Validated;

        private static void Grid_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }

        private static void Grid_VisibleIndexChanged(object sender, EventArgs e)
        {
            VisibleIndexChanged?.Invoke(sender, e);
        }

        private static void Grid_LayoutChanged(object sender, EventArgs e)
        {
            LayoutChanged?.Invoke(sender, e);
        }

        private static void Grid_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        private static void Grid_CursorPointChanged(object sender, EventArgs e)
        {
            CursorPointChanged?.Invoke(sender, e);
        }

        private static void Grid_CompositionStringChanged(object sender, EventArgs e)
        {
            CompositionStringChanged?.Invoke(sender, e);
        }

        private static void Grid_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(sender, e);
        }

        private static void Grid_LostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(sender, e);
        }

        private static void Grid_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }
    }
}
