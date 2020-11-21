﻿// MIT License
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
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal.InputHandlers
{
    class MobileInputHandlerContext : InputHandlerContext
    {
        private const float scrollSpeed = 1.0f;
        private const float scrollStopSpeed = 100.0f;
        private readonly float clickThreshold = 0.5f;
        private readonly Swiper swiper = new Swiper();
        private readonly TerminalKeyboardBase keyboard;
        private InputSelections selections;
        private Vector2 downPosition;
        private TerminalPoint downPoint;
        private TerminalRange dragRange;
        private TerminalRange downRange;
        private float time;
        private int downCount;
        private float scrollPos;
        private bool isScrolling;
        private bool isDown;
        private bool isSelecting;
        private bool isExecuting;
        private float downTime;
        private float scrollDelta;

        public MobileInputHandlerContext()
        {
            this.keyboard = KeyboardCreator != null ? KeyboardCreator() : new MobileKeyboard();
        }

        public override void BeginDrag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.keyboard.IsOpened == false && this.downCount == 1 && this.downTime < 0.5f)
                {
                    this.isScrolling = true;
                    this.scrollPos = (int)grid.VisibleIndex;
                }
                else if (this.isSelecting == true && downPoint != TerminalPoint.Invalid)
                {
                    var position = SelectionUtility.WorldToGrid(grid, eventData.position, eventData.pressEventCamera);
                    var point = SelectionUtility.Intersect(grid, position);
                    if (point != TerminalPoint.Invalid)
                    {
                        this.Grid.SelectingRange = SelectionUtility.UpdatePoint(grid, downPoint, point);
                    }
                }
            }
        }

        public override void Drag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var grid = this.Grid;
                var downPoint = this.downPoint;
                var downRange = this.downRange;
                var dragRange = this.dragRange;
                if (this.isScrolling == true)
                {
                    this.scrollDelta = eventData.delta.y * scrollSpeed;
                    this.DoScroll();
                }
                else if (this.isSelecting == true && downPoint != TerminalPoint.Invalid)
                {
                    var position = SelectionUtility.WorldToGrid(grid, eventData.position, eventData.pressEventCamera);
                    var point = SelectionUtility.Intersect(grid, position);
                    if (point != TerminalPoint.Invalid)
                    {
                        this.dragRange = SelectionUtility.UpdatePoint(grid, downPoint, point);
                        this.UpdateSelecting();
                    }
                }
            }
        }

        public override void EndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.isScrolling == true)
                {
                }
                else if (this.isSelecting == true && this.downPoint != TerminalPoint.Invalid)
                {
                    this.Grid.Selections.Clear();
                    this.Grid.Selections.Add(this.Grid.SelectingRange);
                    this.Grid.SelectingRange = TerminalRange.Empty;
                    this.downPoint = TerminalPoint.Invalid;
                }
            }
        }

        public override void PointerClick(PointerEventData eventData)
        {
        }

        public override void PointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.OnLeftPointerDown(eventData);
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
                this.OnLeftPointerUp(eventData);
                this.downTime = 0.0f;
                this.isDown = false;
                this.isSelecting = false;
            }
        }

        public override void Select(BaseEventData eventData)
        {
            this.scrollPos = this.Grid.VisibleIndex;
        }

        public override void Deselect(BaseEventData eventData)
        {
            if (this.keyboard.IsOpened == true)
            {
                this.keyboard.Close();
            }
        }

        public override void Update(BaseEventData eventData)
        {
            if (this.isDown == true)
            {
                if (this.isScrolling == false && this.isSelecting == false)
                {
                    this.downTime += Time.deltaTime;
                }
                if (this.downCount == 1 && this.downTime > 0.5f && this.isSelecting == false)
                {
                    this.downRange = SelectionUtility.SelectWord(this.Grid, this.downPoint);
                    this.isSelecting = true;
                    this.isScrolling = false;
                    this.UpdateSelecting();
                }
            }
            else
            {
                if (this.isScrolling == true)
                {
                    this.DoScroll();
                }
            }

            this.swiper.Update();
            this.keyboard.Update();
        }

        public override void Attach(ITerminalGrid grid)
        {
            base.Attach(grid);
            TerminalEvents.Executed += Terminal_Executed;
            TerminalEvents.PropertyChanged += Terminal_PropertyChanged;
            this.swiper.Swiped += Swiper_Swiped;
            this.keyboard.Opened += Keyboard_Opened;
            this.keyboard.Done += Keyboard_Done;
            this.keyboard.Canceled += Keyboard_Canceled;
            this.keyboard.Changed += Keyboard_Changed;
            this.selections = new InputSelections(grid);
            TerminalKeyboardEvents.Register(this.keyboard);
        }

        public override void Detach(ITerminalGrid grid)
        {
            TerminalKeyboardEvents.Unregister(this.keyboard);
            this.selections.Dispose();
            this.selections = null;
            TerminalEvents.Executed -= Terminal_Executed;
            TerminalEvents.PropertyChanged -= Terminal_PropertyChanged;
            this.swiper.Swiped -= Swiper_Swiped;
            this.keyboard.Opened -= Keyboard_Opened;
            this.keyboard.Done -= Keyboard_Done;
            this.keyboard.Canceled -= Keyboard_Canceled;
            this.keyboard.Changed -= Keyboard_Changed;
            base.Detach(grid);
        }

        private void OnLeftPointerDown(PointerEventData eventData)
        {
            var grid = this.Grid;
            var terminal = this.Terminal;
            var newPosition = SelectionUtility.WorldToGrid(grid, eventData.position, eventData.pressEventCamera);
            var newPoint = SelectionUtility.Intersect(grid, newPosition);
            var newTime = Time.time;
            var downCount = GetDownCount(this.downCount, this.clickThreshold, this.time, newTime, this.downPosition, newPosition);
            var row = grid.Rows[newPoint.Y];
            var cell = row.Cells[newPoint.X];
            eventData.useDragThreshold = false;
            this.Focus();
            this.downPosition = newPosition;
            this.downPoint = newPoint;
            this.downCount = downCount;
            this.dragRange = new TerminalRange(newPoint, newPoint);
            this.time = newTime;
            this.isDown = true;
            this.downTime = 0.0f;

            if (this.downPoint != TerminalPoint.Invalid && this.keyboard.IsOpened == false)
            {
                if (this.isScrolling == true)
                {
                    this.scrollDelta = 0.0f;
                    this.downPoint = TerminalPoint.Invalid;
                    this.downCount = 0;
                }
            }
        }

        private void OnLeftPointerUp(PointerEventData eventData)
        {
            var grid = this.Grid;
            var position = SelectionUtility.WorldToGrid(grid, eventData.position, eventData.pressEventCamera);
            var newPoint1 = SelectionUtility.Intersect(grid, position);
            var newPoint2 = new TerminalPoint(newPoint1.X + 1, newPoint1.Y);
            var oldPoint = this.downPoint;
            if (oldPoint == newPoint1 && this.keyboard.IsOpened == false)
            {
                if (this.downCount == 1)
                {
                    if (this.downTime >= 0.5f)
                    {
                        this.Grid.Selections.Clear();
                        this.Grid.Selections.Add(this.Grid.SelectingRange);
                        this.Grid.SelectingRange = TerminalRange.Empty;
                    }
                    else if (this.Grid.Selections.Any() == true)
                    {
                        this.Grid.Selections.Clear();
                        this.Grid.SelectingRange = TerminalRange.Empty;
                    }
                    else if (this.isExecuting == false)
                    {
                        this.Grid.ScrollToCursor();
                        this.keyboard.Open(this.Grid, this.Terminal.Command);
                    }
                }
            }
            else if (this.isSelecting == true)
            {
                this.Grid.Selections.Clear();
                this.Grid.Selections.Add(this.Grid.SelectingRange);
                this.Grid.SelectingRange = TerminalRange.Empty;
            }
        }

        private void UpdateSelecting()
        {
            var p1 = this.downRange.BeginPoint < this.dragRange.BeginPoint ? this.downRange.BeginPoint : this.dragRange.BeginPoint;
            var p2 = this.downRange.EndPoint > this.dragRange.EndPoint ? this.downRange.EndPoint : this.dragRange.EndPoint;
            this.Grid.Selections.Clear();
            this.Grid.SelectingRange = new TerminalRange(p1, p2);
        }

        private void Focus() => this.Grid.Focus();

        private void DoScroll()
        {
            var maxIndex = this.Grid.MaximumVisibleIndex;
            var minIndex = this.Grid.MinimumVisibleIndex;
            var index = this.Grid.VisibleIndex;
            var scrollPos = this.scrollPos;
            var scrollDelta = this.scrollDelta;
            if (maxIndex > minIndex)
            {
                scrollPos += Time.deltaTime * scrollDelta;
                if (scrollPos <= minIndex)
                {
                    index = minIndex;
                    scrollDelta = 0.0f;
                }
                else if (scrollPos >= maxIndex)
                {
                    index = maxIndex;
                    scrollDelta = 0.0f;
                }
                else if (scrollDelta > 0)
                {
                    index = (int)scrollPos;
                    scrollDelta -= scrollStopSpeed * Time.deltaTime;
                    if (scrollDelta < 0)
                        scrollDelta = 0.0f;
                }
                else if (scrollDelta < 0)
                {
                    index = (int)scrollPos;
                    scrollDelta += scrollStopSpeed * Time.deltaTime;
                    if (scrollDelta > 0)
                        scrollDelta = 0.0f;
                }
            }

            this.Grid.IsScrolling = true;
            this.scrollDelta = scrollDelta;
            this.scrollPos = scrollPos;
            this.Grid.VisibleIndex = index;

            if (this.scrollDelta == 0.0f && this.isDown == false)
            {
                this.isScrolling = false;
                this.scrollDelta = 0.0f;
                this.Grid.IsScrolling = false;
            }
        }

        private void Terminal_Executed(object sender, TerminalExecutedEventArgs e)
        {
            this.scrollPos = (int)this.Grid.VisibleIndex;
            this.isExecuting = false;
        }

        private void Terminal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ITerminal.PromptText) && this.keyboard.IsOpened == true)
            {
                this.keyboard.Text = this.Terminal.Command;
            }
        }

        private void Swiper_Swiped(object sender, SwipedEventArgs e)
        {
            if (this.keyboard.IsOpened == true)
            {
                if (e.Direction == SwipeDirection.Left)
                {
                    this.Terminal.PrevCompletion();
                }
                else if (e.Direction == SwipeDirection.Right)
                {
                    this.Terminal.NextCompletion();
                }
                else if (e.Direction == SwipeDirection.Up)
                {
                    this.Terminal.PrevHistory();
                }
                else if (e.Direction == SwipeDirection.Down)
                {
                    this.Terminal.NextHistory();
                }
            }
        }

        private void Keyboard_Opened(object sender, TerminalKeyboardEventArgs e)
        {
            this.Grid.SetCommand(e.Text);
            this.Grid.SelectCommand(e.Selection);
        }

        private void Keyboard_Done(object sender, TerminalKeyboardEventArgs e)
        {
            this.isExecuting = true;
            this.Terminal.Command = e.Text;
            this.Terminal.Execute();
            this.scrollPos = (int)this.Grid.VisibleIndex;
        }

        private void Keyboard_Canceled(object sender, EventArgs e)
        {
            this.scrollPos = (int)this.Grid.VisibleIndex;
        }

        private void Keyboard_Changed(object sender, TerminalKeyboardEventArgs e)
        {
            this.Grid.SetCommand(e.Text);
            this.Grid.SelectCommand(e.Selection);
        }

        private static int GetDownCount(int count, float clickThreshold, float oldTime, float newTime, Vector2 oldPosition, Vector2 newPosition)
        {
            var diffTime = newTime - oldTime;
            var distance = (oldPosition - newPosition).magnitude;
            if (diffTime > clickThreshold || oldPosition == Vector2.zero || distance > 2)
                return 1;
            return (count % 3) + 1;
        }

        internal static Func<TerminalKeyboardBase> KeyboardCreator { get; set; }
    }
}