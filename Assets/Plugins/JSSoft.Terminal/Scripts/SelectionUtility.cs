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
using System.Text.RegularExpressions;
using UnityEngine;

namespace JSSoft.Terminal
{
    public static class SelectionUtility
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
            var bufferWidth = grid.BufferWidth;
            var gap = 5;
            if (isEnabled1 == true && isEnabled2 == false)
            {
                var l2 = LastPoint(row2, false);
                var distance = l2.DistanceOf(s2, bufferWidth);
                gap = Math.Min(gap, bufferWidth - (l2.X + 1));
                s2.X = distance > gap ? bufferWidth : l2.X + 1;
            }
            else if (isEnabled2 == true && isEnabled1 == false)
            {
                var l1 = LastPoint(row1, true);
                var distance = l1.DistanceOf(s1, bufferWidth);
                gap = Math.Min(gap, bufferWidth - (l1.X + 1));
                s1.X = distance > gap ? bufferWidth : l1.X;
                s2.X++;
            }
            else if (isEnabled1 == false && isEnabled2 == false)
            {
                if (IsEmpty(row1) == false)
                {
                    var l1 = LastPoint(row1, true);
                    var distance = l1.DistanceOf(s1, bufferWidth);
                    gap = Math.Min(gap, bufferWidth - (l1.X + 1));
                    s1.X = distance > gap ? bufferWidth : l1.X;
                }
                else
                {
                    s1.X = bufferWidth;
                }

                if (IsEmpty(row2) == false)
                {
                    var l2 = LastPoint(row2, true);
                    var distance = l2.DistanceOf(s2, bufferWidth);
                    gap = Math.Min(gap, bufferWidth - (l2.X + 1));
                    s2.X = distance > gap ? bufferWidth : l2.X;
                }
                else
                {
                    s2.X = bufferWidth;
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
            var bufferWidth = row.Grid.BufferWidth;
            var index = row.Index;
            var point = new TerminalPoint(bufferWidth, index);
            if (IsEmpty(row) == false)
            {
                for (var i = bufferWidth - 1; i >= 0; i--)
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

        public static bool IsEmpty(ITerminalRow row)
        {
            return row.Cells.Any(item => item.Character != char.MinValue) == false;
        }

        public static void Select(ITerminalGrid grid, TerminalRange range)
        {
            if (range != TerminalRange.Empty)
            {
                grid.Selections.Clear();
                grid.Selections.Add(range);
                grid.SelectingRange = TerminalRange.Empty;
            }
        }

        public static TerminalRange SelectGroup(ITerminalGrid grid, TerminalPoint point)
        {
            var row = grid.Rows[point.Y];
            var cell = row.Cells[point.X];
            var index = cell.TextIndex;
            var character = cell.Character;
            var patterns = new string[] { @"\[[^\]]*\]", @"\{[^\}]*\}", @"\([^\)]*\)", @"\<[^\>]*\>" };
            var pattern = string.Join("|", patterns);
            var matches = Regex.Matches(grid.Text, pattern).Cast<Match>();
            var match = matches.FirstOrDefault(item => item.Index == index);
            if (match != null)
            {
                var p1 = grid.CharacterInfos[match.Index].Point;
                var p2 = grid.CharacterInfos[match.Index + match.Length].Point;
                return new TerminalRange(p1, p2);
            }
            return TerminalRange.Empty;
        }

        public static TerminalRange SelectLine(ITerminalGrid grid, TerminalPoint point)
        {
            var row = grid.Rows[point.Y];
            if (IsEmpty(row) == false)
            {
                var cell = row.Cells.First();
                var index = cell.TextIndex;
                var text = grid.Text + char.MinValue;
                var matches = Regex.Matches(grid.Text, @"^|$", RegexOptions.Multiline).Cast<Match>();
                var match1 = matches.Where(item => item.Index <= index).Last();
                var match2 = matches.Where(item => item.Index > index).First();
                var p1 = grid.CharacterInfos[match1.Index].Point;
                var p2 = grid.CharacterInfos[match2.Index].Point;
                var p3 = new TerminalPoint(0, p1.Y);
                var p4 = new TerminalPoint(grid.BufferWidth, p2.Y);
                return new TerminalRange(p3, p4);
            }
            else
            {
                var p1 = new TerminalPoint(0, point.Y);
                var p2 = new TerminalPoint(grid.BufferWidth, point.Y);
                return new TerminalRange(p1, p2);
            }
        }

        public static TerminalRange SelectWord(ITerminalGrid grid, TerminalPoint point)
        {
            var row = grid.Rows[point.Y];
            var cell = row.Cells[point.X];
            if (IsEmpty(row) == true)
                return SelectWordOfEmptyRow(grid, row);
            else if (cell.Character == char.MinValue)
                return SelectWordOfEmptyCell(grid, cell);
            else
                return SelectWordOfCell(grid, cell);
        }

        public static Vector2 WorldToGrid(ITerminalGrid grid, Vector2 position, Camera camera) => grid.WorldToGrid(position, camera);

        public static TerminalPoint Intersect(ITerminalGrid grid, Vector2 position) => grid.Intersect(position);

        public static bool IsEnabled(ITerminalCell cell)
        {
            var character = cell.Character;
            return character != char.MinValue && character != '\n';
        }

        private static TerminalRange SelectWordOfEmptyRow(ITerminalGrid grid, ITerminalRow row)
        {
            var p1 = new TerminalPoint(0, row.Index);
            var p2 = new TerminalPoint(grid.BufferWidth, row.Index);
            return new TerminalRange(p1, p2);
        }

        private static TerminalRange SelectWordOfEmptyCell(ITerminalGrid grid, ITerminalCell cell)
        {
            var row = cell.Row;
            var cells = row.Cells;
            var p1 = LastPoint(row, true);
            var p2 = new TerminalPoint(grid.BufferWidth, row.Index);
            return new TerminalRange(p1, p2);
        }

        private static TerminalRange SelectWordOfCell(ITerminalGrid grid, ITerminalCell cell)
        {
            var text = grid.Text;
            var index = cell.TextIndex;
            var character = cell.Character;
            var pattern = GetPattern();
            var matches = Regex.Matches(text, pattern).Cast<Match>();
            var match = matches.First(item => index >= item.Index && index < item.Index + item.Length);
            var i1 = match.Index;
            var i2 = i1 + match.Length;
            var c1 = grid.CharacterInfos[i1];
            var c2 = grid.CharacterInfos[i2];
            var p1 = c1.Point;
            var p2 = c2.Point;
            return new TerminalRange(p1, p2);

            string GetPattern()
            {
                if (char.IsLetterOrDigit(character) == true)
                    return @"(?:\w+\.(?=\w))*\w+";
                else if (char.IsWhiteSpace(character) == true)
                    return @"\s+";
                else
                    return @"[^\w\s]";
            }
        }
    }
}
