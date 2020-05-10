﻿// MIT License
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
using System.Collections.Specialized;
using System.ComponentModel;

namespace JSSoft.UI.InputHandlers
{
    class MacOSInputHandlerContext : InputHandlerContext
    {
        private static Texture2D cursorTexture;
        private readonly List<TerminalRange> selections = new List<TerminalRange>();
        private readonly List<RangeInt> rangeList = new List<RangeInt>();
        private readonly float clickThreshold = 0.5f;
        private Vector2 downPosition;
        private TerminalPoint downPoint;
        private TerminalRange dragRange;
        private TerminalRange downRange;
        private float time;
        private int downCount;

        public override void BeginDrag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                var position = SelectionUtility.WorldToGrid(grid, eventData.position);
                var point = SelectionUtility.Intersect(grid, position);
                if (point != TerminalPoint.Invalid)
                {
                    grid.SelectingRange = SelectionUtility.UpdatePoint(grid, downPoint, point);
                }
            }
        }

        public override void Drag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            var downRange = this.downRange;
            var dragRange = this.dragRange;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                var position = SelectionUtility.WorldToGrid(grid, eventData.position);
                var point = SelectionUtility.Intersect(grid, position);
                if (point != TerminalPoint.Invalid)
                {
                    this.dragRange = SelectionUtility.UpdatePoint(grid, downPoint, point);
                    this.UpdateSelecting();
                }
            }
        }

        public override void EndDrag(PointerEventData eventData)
        {
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left && downPoint != TerminalPoint.Invalid)
            {
                // Debug.Log(nameof(EndDrag));
                this.Grid.Selections.Clear();
                // Debug.Log(this.Grid.SelectingRange);
                if (this.Grid.SelectingRange != TerminalRange.Empty)
                {
                    this.Grid.Selections.Add(this.Grid.SelectingRange);
                    this.Grid.SelectingRange = TerminalRange.Empty;
                }
                this.downPoint = TerminalPoint.Invalid;
            }
        }

        public override void PointerClick(PointerEventData eventData)
        {

        }

        public override void PointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var grid = this.Grid;
                var newPosition = SelectionUtility.WorldToGrid(grid, eventData.position);
                var newPoint = SelectionUtility.Intersect(grid, newPosition);
                var newTime = Time.time;
                var downCount = GetDownCount(this.downCount, this.clickThreshold, this.time, newTime, this.downPosition, newPosition);
                eventData.useDragThreshold = false;
                this.Grid.Focus();
                this.downPosition = newPosition;
                this.downPoint = newPoint;
                this.downCount = downCount;
                this.dragRange = new TerminalRange(newPoint, newPoint);
                this.time = newTime;

                if (newPoint != TerminalPoint.Invalid)
                {
                    if (downCount == 1)
                    {
                        this.Grid.SelectingRange = TerminalRange.Empty;
                        // this.Grid.Selections.Clear();
                        this.downRange = SelectionUtility.UpdatePoint(grid, newPoint, newPoint);
                    }
                    else if (downCount == 2)
                    {
                        this.downRange = SelectionUtility.SelectWord(grid, newPoint);
                        this.UpdateSelecting();
                    }
                    else if (downCount == 3)
                    {
                        this.downRange = SelectionUtility.SelectLine(grid, newPoint);
                        this.UpdateSelecting();
                    }
                }
            }
        }

        public override void PointerEnter(PointerEventData eventData)
        {

        }

        public override void PointerExit(PointerEventData eventData)
        {

        }

        public override void PointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var grid = this.Grid;
                var downCount = this.downCount;
                var position = SelectionUtility.WorldToGrid(grid, eventData.position);
                var newPoint = SelectionUtility.Intersect(grid, position);
                var oldPoint = this.downPoint;
                if (oldPoint == newPoint && eventData.dragging == false)
                {
                    if (downCount == 2 || downCount == 3)
                    {
                        SelectionUtility.Select(grid, grid.SelectingRange);
                    }
                }
            }
        }

        public override void Attach(ITerminalGrid grid)
        {
            base.Attach(grid);
            this.Grid.SelectionChanged += Grid_SelectionChanged;
            this.Grid.PropertyChanged += Grid_PropertyChanged;
        }

        public override void Detach(ITerminalGrid grid)
        {
            this.Grid.SelectionChanged -= Grid_SelectionChanged;
            this.Grid.PropertyChanged -= Grid_PropertyChanged;
            base.Detach(grid);
        }

        public override void Update(BaseEventData eventData)
        {

        }

        private void UpdateSelecting()
        {
            var p1 = this.downRange.BeginPoint < this.dragRange.BeginPoint ? this.downRange.BeginPoint : this.dragRange.BeginPoint;
            var p2 = this.downRange.EndPoint > this.dragRange.EndPoint ? this.downRange.EndPoint : this.dragRange.EndPoint;
            this.Grid.Selections.Clear();
            this.Grid.SelectingRange = new TerminalRange(p1, p2);
        }

        private static int GetDownCount(int count, float clickThreshold, float oldTime, float newTime, Vector2 oldPosition, Vector2 newPosition)
        {
            var diffTime = newTime - oldTime;
            if (diffTime > clickThreshold || oldPosition != newPosition)
                return 1;
            return (count) % 3 + 1;
        }

        private void Grid_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Debug.Log(e.Action);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            // Debug.Log((TerminalRange)item);
                            this.selections.Add((TerminalRange)item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            this.selections.Remove((TerminalRange)item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var oldItem = (TerminalRange)e.OldItems[i];
                            var newItem = (TerminalRange)e.NewItems[i];
                            var index = this.selections.IndexOf(oldItem);
                            this.selections[index] = newItem;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var item = this.selections[e.OldStartingIndex];
                        this.selections.RemoveAt(e.OldStartingIndex);
                        this.selections.Insert(e.NewStartingIndex, item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.selections.Clear();
                    }
                    break;
            }
            this.SelectionToRange();
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITerminalGrid.BufferWidth):
                case nameof(ITerminalGrid.BufferHeight):
                    {
                        this.RangeToSelection();
                    }
                    break;
            }
        }

        private void SelectionToRange()
        {
            // Debug.Log($"{nameof(MacOSInputHandlerContext)}.{nameof(SelectionToRange)}");
            this.rangeList.Clear();
            foreach (var item in this.selections)
            {
                var range = SelectionUtility.RangeToInt(this.Grid, item);
                Debug.Log($"{range.start}:{range.end}");
                this.rangeList.Add(range);
            }
        }

        private void RangeToSelection()
        {
            Debug.Log($"{nameof(MacOSInputHandlerContext)}.{nameof(RangeToSelection)}");
            var text = this.Terminal.Text;
            this.Grid.SelectionChanged -= Grid_SelectionChanged;
            this.Grid.Selections.Clear();
            foreach (var item in this.rangeList)
            {
                if (item.start >= 0)
                {
                    var p1 = this.Grid.IndexToPoint(item.start);
                    if (item.length >= 0)
                    {
                        if (text[item.end - 1] == '\n')
                        {
                            Debug.Log("n");
                            var p2 = this.Grid.IndexToPoint(item.end);
                            p2.X = this.Grid.BufferWidth;
                            this.Grid.Selections.Add(new TerminalRange(p1, p2));
                        }
                        else
                        {
                            var p2 = this.Grid.IndexToPoint(item.end);
                            this.Grid.Selections.Add(new TerminalRange(p1, p2));
                        }
                    }
                    else
                    {
                        var range = SelectionUtility.SelectLine(this.Grid, p1);
                        var p2 = range.EndPoint;
                        this.Grid.Selections.Add(new TerminalRange(p1, p2));
                    }
                }
                else
                {
                    var p1 = new TerminalPoint(0, this.Grid.CursorPoint.Y - item.start);
                    var p2 = new TerminalPoint(this.Grid.BufferWidth, this.Grid.CursorPoint.Y - item.end);
                    this.Grid.Selections.Add(new TerminalRange(p1, p2));
                }
            }
            this.SelectionToRange();
            this.Grid.SelectionChanged += Grid_SelectionChanged;
        }
    }
}
