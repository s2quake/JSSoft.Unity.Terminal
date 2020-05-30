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
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;

namespace JSSoft.Terminal
{
    public static class TerminalGridEvents
    {
        private static readonly HashSet<ITerminalGrid> grids = new HashSet<ITerminalGrid>();

        public static void Register(ITerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grids.Contains(grid) == true)
                throw new ArgumentException($"{nameof(grid)} is already exists.");
            grids.Add(grid);
            grid.LayoutChanged += Grid_LayoutChanged;
            grid.SelectionChanged += Grid_SelectionChanged;
            grid.GotFocus += Grid_GotFocus;
            grid.LostFocus += Grid_LostFocus;
            grid.Validated += Grid_Validated;
            grid.PropertyChanged += Grid_PropertyChanged;
            grid.Enabled += Grid_Enabled;
            grid.Disabled += Grid_Disabled;
        }

        public static void Unregister(ITerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grids.Contains(grid) == false)
                throw new ArgumentException($"{nameof(grid)} does not exists.");
            grid.LayoutChanged -= Grid_LayoutChanged;
            grid.SelectionChanged -= Grid_SelectionChanged;
            grid.GotFocus -= Grid_GotFocus;
            grid.LostFocus -= Grid_LostFocus;
            grid.Validated -= Grid_Validated;
            grid.PropertyChanged -= Grid_PropertyChanged;
            grid.Enabled -= Grid_Enabled;
            grid.Disabled -= Grid_Disabled;
            grids.Remove(grid);
        }

        public static event EventHandler LayoutChanged;

        public static event NotifyCollectionChangedEventHandler SelectionChanged;

        public static event EventHandler GotFocus;

        public static event EventHandler LostFocus;

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        public static event EventHandler Enabled;

        public static event EventHandler Disabled;

        private static void Grid_LayoutChanged(object sender, EventArgs e)
        {
            LayoutChanged?.Invoke(sender, e);
        }

        private static void Grid_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
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

        private static void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private static void Grid_Enabled(object sender, EventArgs e)
        {
            Enabled?.Invoke(sender, e);
        }

        private static void Grid_Disabled(object sender, EventArgs e)
        {
            Disabled?.Invoke(sender, e);
        }
    }
}
