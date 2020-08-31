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
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.Terminal
{
    static class TerminalGridValidator
    {
        public static TerminalPoint ValidateCursorPoint(TerminalGrid grid, TerminalPoint cursorPoint)
        {
            var x = grid.CursorPoint.X;
            var y = grid.CursorPoint.Y;
            var bufferWidth = grid.BufferWidth;
            var bufferHeight = grid.BufferHeight;
            var rowCount = grid.Rows.Count;
            var maxBufferHeight = Math.Max(bufferHeight, rowCount);
            x = Math.Min(x, bufferWidth - 1);
            x = Math.Max(x, 0);
            y = Math.Min(y, maxBufferHeight - 1);
            y = Math.Max(y, 0);
            return new TerminalPoint(x, y);
        }

        public static int GetVisibleIndex(TerminalGrid grid)
        {
            return GetVisibleIndex(grid, grid.VisibleIndex);
        }

        public static int GetVisibleIndex(TerminalGrid grid, int visibleIndex)
        {
            var minimumVisibleIndex = grid.MinimumVisibleIndex;
            var maximumVisibleIndex = grid.MaximumVisibleIndex;
            visibleIndex = Math.Max(visibleIndex, minimumVisibleIndex);
            visibleIndex = Math.Min(visibleIndex, maximumVisibleIndex);
            return visibleIndex;
        }
    }
}
