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
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI.InputHandlers
{
    public static class InputHandlerUtility
    {
        public static TerminalRange UpdatePoint(ITerminalGrid grid, TerminalPoint p1, TerminalPoint p2)
        {
            var (s1, s2) = p1 < p2 ? (p1, p2) : (p2, p1);
            var cell1 = grid.Rows[s1.Y].Cells[s1.X];
            var cell2 = grid.Rows[s2.Y].Cells[s2.X];
            var row1 = cell1.Row;
            var row2 = cell2.Row;
            var isEnabled1 = IsEnabled(cell1);
            var isEnabled2 = IsEnabled(cell2);
            var columnCount = grid.ColumnCount;
            var gap = 5;
            if (isEnabled1 == true && isEnabled2 == false)
            {
                var l2 = LastPoint(row2, false);
                var distance = l2.DistanceOf(s2, columnCount);
                s2.X = distance > gap ? columnCount : l2.X + 1;
            }
            else if (isEnabled2 == true && isEnabled1 == false)
            {
                var l1 = LastPoint(row1, true);
                var distance = l1.DistanceOf(s1, columnCount);
                s1.X = distance > gap ? columnCount : l1.X;
                s2.X++;
            }
            else if (isEnabled1 == false && isEnabled2 == false)
            {
                if (row1.Text != string.Empty)
                {
                    var l1 = LastPoint(row1, true);
                    var distance = l1.DistanceOf(s1, columnCount);
                    s1.X = distance > gap ? columnCount : l1.X;
                }
                else
                {
                    s1.X = columnCount;
                }

                if (row2.Text != string.Empty)
                {
                    var l2 = LastPoint(row2, true);
                    var distance = l2.DistanceOf(s2, columnCount);
                    s2.X = distance > gap ? columnCount : l2.X;
                }
                else
                {
                    s2.X = columnCount;
                }
            }
            else
            {
                s2.X++;
            }
            return new TerminalRange(s1, s2);
        }

        public static TerminalPoint LastPoint(ITerminalRow row, bool isCursor)
        {
            var columnCount = row.Grid.ColumnCount;
            var index = row.Index;
            var point = new TerminalPoint(columnCount, index);
            if (row.Text != string.Empty)
            {
                for (var i = columnCount - 1; i >= 0; i--)
                {
                    var item = row.Cells[i];
                    var character = item.Character;
                    if (character != char.MinValue && character != '\n')
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

        public static bool IsEnabled(ITerminalCell cell)
        {
            var character = cell.Character;
            return character != char.MinValue && character != '\n';
        }
    }
}
