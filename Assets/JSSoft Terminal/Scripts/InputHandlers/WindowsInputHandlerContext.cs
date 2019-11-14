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
    class WindowsInputHandlerContext : InputHandlerContext
    {
        private static Texture2D cursorTexture;
        private readonly float clickThreshold = 0.5f;
        private Vector2 downPosition;
        private TerminalPoint downPoint;
        private TerminalRange dragRange;
        private TerminalRange downRange;
        private float time;
        private int downCount;

        static WindowsInputHandlerContext()
        {

        }

        public WindowsInputHandlerContext(ITerminalGrid grid)
            : base(grid)
        {

        }

        public bool BeginDrag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    this.SelectingRange = InputHandlerUtility.UpdatePoint(grid, downPoint, point);
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
                var position = this.WorldToGrid(eventData.position);
                var point = this.Intersect(position);
                if (point != TerminalPoint.Invalid)
                {
                    this.dragRange = InputHandlerUtility.UpdatePoint(grid, downPoint, point); ;
                    this.UpdateSelecting();
                }
                return true;
            }
            return false;
        }

        public bool EndDrag(PointerEventData eventData)
        {
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                this.Selections.Clear();
                this.Selections.Add(this.SelectingRange);
                this.SelectingRange = TerminalRange.Empty;
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

        public static Texture2D CursorTexture
        {
            get
            {
                if (cursorTexture == null)
                {
                    cursorTexture = Resources.Load("ibeam-Windows") as Texture2D;
                }
                return cursorTexture;
            }
        }

        private void SelectWord(TerminalPoint point)
        {
            var grid = this.Grid;
            var row = grid.Rows[point.Y];
            var cell = row.Cells[point.X];
            if (row.Text == string.Empty)
                this.SelectWordOfEmptyRow(row);
            else if (cell.Character == char.MinValue)
                this.SelectWordOfEmptyCell(cell);
            else
                this.SelectWordOfCell(cell);
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
                var matches = Regex.Matches(grid.Text, @"^|$", RegexOptions.Multiline).Cast<Match>();
                var match1 = matches.Where(item => item.Index <= index).Last();
                var match2 = matches.Where(item => item.Index > index).First();
                var p1 = grid.CharacterInfos[match1.Index].Point;
                var p2 = grid.CharacterInfos[match2.Index].Point;
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

        private void SelectGroup(TerminalPoint point)
        {
            var grid = this.Grid;
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
                var range = new TerminalRange(p1, p2);
                this.Selections.Clear();
                this.Selections.Add(range);
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
            this.downRange = new TerminalRange(p1, p2);
            this.UpdateSelecting();

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

        private bool OnLeftPointerDown(PointerEventData eventData)
        {
            var grid = this.Grid;
            var newPosition = this.WorldToGrid(eventData.position);
            var newPoint = this.Intersect(newPosition);
            var newTime = Time.time;
            var downCount = GetDownCount(this.downCount, this.clickThreshold, this.time, newTime, this.downPosition, newPosition);
            eventData.useDragThreshold = false;
            this.Focus();
            this.downPosition = newPosition;
            this.downPoint = newPoint;
            this.downCount = downCount;
            this.dragRange = new TerminalRange(newPoint, newPoint);
            this.time = newTime;

            if (newPoint != TerminalPoint.Invalid)
            {
                var row = grid.Rows[newPoint.Y];
                if (downCount == 1)
                {
                    this.SelectingRange = TerminalRange.Empty;
                    this.Selections.Clear();
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
            return true;
        }

        private bool OnLeftPointerUp(PointerEventData eventData)
        {
            var position = this.WorldToGrid(eventData.position);
            var newPoint = this.Intersect(position);
            var oldPoint = this.downPoint;
            if (oldPoint == newPoint)
            {
                this.Selections.Clear();
                this.Selections.Add(this.SelectingRange);
                this.SelectingRange = TerminalRange.Empty;
                if (this.downCount == 2)
                {
                    this.SelectGroup(newPoint);
                }
                return true;
            }
            return false;
        }

        private void UpdateSelecting()
        {
            var p1 = this.downRange.BeginPoint < this.dragRange.BeginPoint ? this.downRange.BeginPoint : this.dragRange.BeginPoint;
            var p2 = this.downRange.EndPoint > this.dragRange.EndPoint ? this.downRange.EndPoint : this.dragRange.EndPoint;
            this.Selections.Clear();
            this.SelectingRange = new TerminalRange(p1, p2);
        }

        private Vector2 WorldToGrid(Vector2 position) => this.Grid.WorldToGrid(position);

        private TerminalPoint Intersect(Vector2 position) => this.Grid.Intersect(position);

        private void Focus() => this.Grid.Focus();

        static int GetDownCount(int count, float clickThreshold, float oldTime, float newTime, Vector2 oldPosition, Vector2 newPosition)
        {
            var diffTime = newTime - oldTime;
            if (diffTime > clickThreshold || oldPosition != newPosition)
                return 1;
            return ++count;
        }

        private TerminalRange SelectingRange
        {
            get => this.Grid.SelectingRange;
            set => this.Grid.SelectingRange = value;
        }

        private IList<TerminalRange> Selections => this.Grid.Selections;
    }
}
