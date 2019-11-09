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

namespace JSSoft.UI.InputHandlers
{
    class MacOSInputHandlerContext : InputHandlerContext
    {
        private TerminalPoint downPoint;
        private TerminalPoint beginPoint;
        private TerminalPoint endPoint;
        private float clickThreshold = 0.5f;
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
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                var position = grid.WorldToGrid(eventData.position);
                var point = grid.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    grid.SelectingRange = InputHandlerUtility.UpdatePoint(grid, this.downPoint, point);
                }
                return true;
            }
            return false;
        }

        public bool EndDrag(PointerEventData eventData)
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
            return false;
        }

        private void SelectWord(TerminalPoint point)
        {
            var grid = this.Grid;
            var terminal = grid.Terminal;
            var row = grid.Rows[point.Y];
            if (row.Text == string.Empty)
            {
                var p1 = new TerminalPoint(0, point.Y);
                var p2 = new TerminalPoint(grid.ColumnCount, point.Y);
                var range = new TerminalRange(p1, p2);
                grid.Selections.Clear();
                grid.Selections.Add(range);
                Debug.Log(1);
            }
            else
            {
                var text = row.Text;
                var cell = row.Cells[point.X];
                var character = cell.Character;
                if (character == char.MinValue)
                {
                    
                }
                else
                {
                    // var p1 = point;
                    // var p2 = point;
                    // // p1 = SkipBackward(grid, p1, true);
                    // p1 = SkipBackward(grid, p1);
                    // // p1.X++;
                    // // p2 = SkipForward(grid, p2, true);
                    // p2 = SkipForward(grid, p2);
                    // // p2.X--;
                    // var range = new TerminalRange(p1, p2);
                    // grid.Selections.Clear();
                    // grid.Selections.Add(range);
                }
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
                var range = new TerminalRange(p1, new TerminalPoint(grid.ColumnCount, p2.Y));
                grid.Selections.Clear();
                grid.Selections.Add(range);
            }
            else
            {
                var p1 = new TerminalPoint(0, point.Y);
                var p2 = new TerminalPoint(grid.ColumnCount, point.Y);
                var range = new TerminalRange(p1, p2);
                grid.Selections.Clear();
                grid.Selections.Add(range);
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
                    grid.Selections.Clear();
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
            this.time = newTime;
            return true;
        }
    }
}
