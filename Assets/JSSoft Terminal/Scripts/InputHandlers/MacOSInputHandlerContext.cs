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
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

namespace JSSoft.UI.InputHandlers
{
    class MacOSInputHandlerContext : InputHandlerContext
    {
        private readonly float clickThreshold = 0.5f;
        private TerminalPoint downPoint;
        private TerminalRange dragRange;
        private TerminalRange downRange;
        private float time;
        private int downCount;

        public MacOSInputHandlerContext(ITerminalGrid grid)
            : base(grid)
        {

        }

        public bool BeginDrag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                var position = grid.WorldToGrid(eventData.position);
                var point = grid.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    grid.SelectingRange = InputHandlerUtility.UpdatePoint(grid, downPoint, point);
                }
                return true;
            }
            return false;
        }

        public bool Drag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            var downRange = this.downRange;
            var dragRange = this.dragRange;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                var position = grid.WorldToGrid(eventData.position);
                var point = grid.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    // var range = InputHandlerUtility.UpdatePoint(grid, downPoint, point);
                    // var p1 = downRange.BeginPoint < range.BeginPoint ? downRange.BeginPoint : range.BeginPoint;
                    // var p2 = downRange.EndPoint > range.EndPoint ? downRange.EndPoint : range.EndPoint;
                    // grid.SelectingRange = InputHandlerUtility.UpdatePoint(grid, p1, p2);
                    
                    this.dragRange = InputHandlerUtility.UpdatePoint(grid, downPoint, point);;
                    // Debug.Log(this.dragRange);
                    // Debug.Log(this.downRange);
                    this.UpdateSelecting();
                }
                return true;
            }
            return false;
        }

        public bool EndDrag(PointerEventData eventData)
        {
            var grid = this.Grid;
            // var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                // var position = grid.WorldToGrid(eventData.position);
                // var point = grid.Intersect(position);
                // if (point != TerminalPoint.Invalid)
                // {
                //     var range = InputHandlerUtility.UpdatePoint(grid, downPoint, point);
                //     var p1 = downRange.BeginPoint < range.BeginPoint ? downRange.BeginPoint : range.BeginPoint;
                //     var p2 = downRange.EndPoint > range.EndPoint ? downRange.EndPoint : range.EndPoint;
                //     grid.SelectingRange = InputHandlerUtility.UpdatePoint(grid, p1, p2);
                //     this.dragRange = range;
                //     this.UpdateSelecting();
                // }
                grid.Selections.Clear();
                grid.Selections.Add(grid.SelectingRange);
                grid.SelectingRange = TerminalRange.Empty;
                this.downPoint = TerminalPoint.Invalid;
                return true;
            }
            return false;
        }

        public bool PointerClick(PointerEventData eventData)
        {
            return false;
        }

        public bool PointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                return this.OnLeftPointerDown(eventData);
            }
            return false;
        }

        public bool PointerEnter(PointerEventData eventData)
        {
            return false;
        }

        public bool PointerExit(PointerEventData eventData)
        {
            return false;
        }

        public bool PointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                return this.OnLeftPointerUp(eventData);
            }
            return false;
        }

        private void SelectWord(TerminalPoint point)
        {
            var grid = this.Grid;
            var row = grid.Rows[point.Y];
            var cell = row.Cells[point.X];
            if (row.Text == string.Empty)
            {
                this.SelectWordOfEmptyRow(row);
            }
            else if (cell.Character == char.MinValue)
            {
                this.SelectWordOfEmptyCell(cell);
            }
            else
            {
                this.SelectWordOfCell(cell);
            }
        }

        private void SelectLine(TerminalPoint point)
        {
            var grid = this.Grid;
            var row = grid.Rows[point.Y];
            if (row.Text != string.Empty)
            {
                var cell = row.Cells.First();
                var index = cell.TextIndex;
                var text = grid.Text + char.MinValue;
                var predicate = new Func<char, bool>((item => item != '\n'));
                var s1 = CommandStringUtility.SkipBackward(text, index, predicate) + 1;
                var s2 = CommandStringUtility.SkipForward(text, index, predicate) - 1;
                var p1 = grid.CharacterInfos[s1].Point;
                var p2 = grid.CharacterInfos[s2].Point;
                var p3 = new TerminalPoint(0, p1.Y);
                var p4 = new TerminalPoint(grid.ColumnCount, p2.Y);
                this.downRange = new TerminalRange(p3, p4);
                this.UpdateSelecting();
            }
            else
            {
                var p1 = new TerminalPoint(0, point.Y);
                var p2 = new TerminalPoint(grid.ColumnCount, point.Y);
                this.downRange = new TerminalRange(p1, p2);
                this.UpdateSelecting();
            }
        }

        private void SelectWordOfEmptyRow(ITerminalRow row)
        {
            var grid = this.Grid;
            var p1 = new TerminalPoint(0, row.Index);
            var p2 = new TerminalPoint(grid.ColumnCount, row.Index);
            this.downRange = new TerminalRange(p1, p2);
            this.UpdateSelecting();
        }

        private void SelectWordOfEmptyCell(ITerminalCell cell)
        {
            var grid = this.Grid;
            var row = cell.Row;
            var cells = row.Cells;
            var p1 = InputHandlerUtility.LastPoint(row, true);
            var p2 = new TerminalPoint(grid.ColumnCount, row.Index);
            this.downRange = new TerminalRange(p1, p2);
            this.UpdateSelecting();
        }

        private void SelectWordOfCell(ITerminalCell cell)
        {
            var grid = this.Grid;
            var text = grid.Text;
            var row = cell.Row;
            var cells = row.Cells;
            var index = cell.TextIndex;
            var character = cell.Character;
            if (char.IsLetterOrDigit(character) == true)
            {
                var predicate = new Func<char, bool>((c) => char.IsLetterOrDigit(c) || c == '.');
                var i1 = CommandStringUtility.SkipBackward(text, index, predicate);
                i1++;
                var input = text.Substring(i1);
                var pattern = @"(?:\w+\.(?=\w))*\w+";
                var match = Regex.Match(input, pattern, RegexOptions.ECMAScript);
                var i2 = i1 + match.Length;
                var c1 = grid.CharacterInfos[i1];
                var c2 = grid.CharacterInfos[i2];
                var p1 = c1.Point;
                var p2 = c2.Point;
                this.downRange = new TerminalRange(p1, p2);
                this.UpdateSelecting();
            }
            else if (char.IsWhiteSpace(character) == true)
            {
                var predicate = new Func<char, bool>((c) => char.IsWhiteSpace(c));
                var i1 = CommandStringUtility.SkipBackward(text, index, predicate);
                i1++;
                var input = text.Substring(i1);
                var pattern = @"\s+";
                var match = Regex.Match(input, pattern, RegexOptions.ECMAScript);
                var i2 = i1 + match.Length;
                var c1 = grid.CharacterInfos[i1];
                var c2 = grid.CharacterInfos[i2];
                var p1 = c1.Point;
                var p2 = c2.Point;
                this.downRange = new TerminalRange(p1, p2);
                this.UpdateSelecting();
            }
            else
            {
                var c1 = grid.CharacterInfos[index];
                var c2 = grid.CharacterInfos[index + 1];
                var p1 = c1.Point;
                var p2 = c2.Point;
                this.downRange = new TerminalRange(p1, p2);
                this.UpdateSelecting();
            }
        }

        private bool OnLeftPointerDown(PointerEventData eventData)
        {
            var grid = this.Grid;
            var position = grid.WorldToGrid(eventData.position);
            var newPoint = grid.Intersect(position);
            var newTime = Time.time;
            var oldPoint = this.downPoint;
            var oldTime = this.time;
            var downCount = this.downCount;
            var clickThreshold = this.clickThreshold;
            var diffTime = newTime - oldTime;
            if (diffTime > clickThreshold || newPoint != oldPoint)
            {
                downCount = 1;
            }
            else
            {
                downCount++;
            }
            if (newPoint != TerminalPoint.Invalid)
            {
                var row = grid.Rows[newPoint.Y];
                if (downCount == 1)
                {
                    grid.SelectingRange = TerminalRange.Empty;
                    grid.Selections.Clear();
                    this.downRange = InputHandlerUtility.UpdatePoint(grid, newPoint, newPoint);
                }
                else if (downCount == 2)
                {
                    this.SelectWord(newPoint);
                }
                else if (downCount == 3)
                {
                    this.SelectLine(newPoint);
                }
            }

            eventData.useDragThreshold = false;
            grid.Focus();

            this.downPoint = newPoint;
            this.downCount = downCount;
            this.dragRange = new TerminalRange(newPoint, newPoint);
            this.time = newTime;
            return true;
        }

        private bool OnLeftPointerUp(PointerEventData eventData)
        {
            var grid = this.Grid;
            var position = grid.WorldToGrid(eventData.position);
            var newPoint = grid.Intersect(position);
            var oldPoint = this.downPoint;
            if (oldPoint == newPoint)
            {
                grid.Selections.Clear();
                grid.Selections.Add(grid.SelectingRange);
                grid.SelectingRange = TerminalRange.Empty;
                return true;
            }
            return false;
        }

        private void UpdateSelecting()
        {
            var grid = this.Grid;
            var p1 = this.downRange.BeginPoint < this.dragRange.BeginPoint ? this.downRange.BeginPoint : this.dragRange.BeginPoint;
            var p2 = this.downRange.EndPoint > this.dragRange.EndPoint ? this.downRange.EndPoint : this.dragRange.EndPoint;
            grid.Selections.Clear();
            grid.SelectingRange = new TerminalRange(p1, p2);
        }
    }
}
