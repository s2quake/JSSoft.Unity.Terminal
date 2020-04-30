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

namespace JSSoft.UI.InputHandlers
{
    class IOSInputHandlerContext : InputHandlerContext
    {
        private static Texture2D cursorTexture;
        private readonly float clickThreshold = 0.5f;
        private readonly Swiper swiper = new Swiper();
        private Vector2 downPosition;
        private TerminalPoint downPoint;
        private TerminalRange dragRange;
        private TerminalRange downRange;
        private float time;
        private int downCount;
        private TouchScreenKeyboard keyboard;
        private float scrollPos;
        private bool isDragging;
        private bool isScrolling;
        private bool isDown;
        private bool isSelecting;
        private float downTime;
        private float scrollDelta;
        private float scrollSpeed = 1.0f;
        private Vector2 gridPosition;

        public IOSInputHandlerContext()
        {

        }

        public override void BeginDrag(PointerEventData eventData)
        {
            var grid = this.Grid;
            var downPoint = this.downPoint;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.downTime < 0.5f)
                {
                    this.isScrolling = true;
                    this.scrollPos = (int)grid.VisibleIndex;
                }
                else if (this.isSelecting == true && downPoint != TerminalPoint.Invalid)
                {
                    var position = InputHandlerUtility.WorldToGrid(grid, eventData.position);
                    var point = InputHandlerUtility.Intersect(grid, position);
                    if (point != TerminalPoint.Invalid)
                    {
                        this.SelectingRange = InputHandlerUtility.UpdatePoint(grid, downPoint, point);
                    }
                }
            }
            this.isDragging = true;
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
                    if (grid.MaximumVisibleIndex > grid.MinimumVisibleIndex)
                    {
                        var scrollData = eventData.delta.y * this.scrollSpeed;
                        this.scrollPos += Time.deltaTime * scrollData;
                        this.scrollPos = Math.Max(this.scrollPos, grid.MinimumVisibleIndex);
                        this.scrollPos = Math.Min(this.scrollPos, grid.MaximumVisibleIndex);
                        this.scrollDelta = scrollData;
                        grid.VisibleIndex = (int)this.scrollPos;
                    }
                }
                else if (this.isSelecting == true && downPoint != TerminalPoint.Invalid)
                {
                    var position = InputHandlerUtility.WorldToGrid(grid, eventData.position);
                    var point = InputHandlerUtility.Intersect(grid, position);
                    if (point != TerminalPoint.Invalid)
                    {
                        this.dragRange = InputHandlerUtility.UpdatePoint(grid, downPoint, point);
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
                    this.Selections.Clear();
                    this.Selections.Add(this.SelectingRange);
                    this.SelectingRange = TerminalRange.Empty;
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
                this.downTime = 0.0f;
                this.isDown = false;
                this.isSelecting = false;
                this.OnLeftPointerUp(eventData);
            }
        }

        public override void Select(BaseEventData eventData)
        {
            this.scrollPos = this.Grid.VisibleIndex;
        }

        public override void Deselect(BaseEventData eventData)
        {
        }

        // public float maxTime = 1.0f;
        // public float minSwipeDist = 2.0f;

        // float startTime;
        // float endTime;

        // Vector3 startPos;
        // Vector3 endPos;
        // float swipeDistance;
        // float swipeTime;

        public override void Update(BaseEventData eventData)
        {
            var grid = this.Grid;
            if (this.isDown == true && this.isScrolling == false && this.isSelecting == false)
            {
                this.downTime += Time.deltaTime;
            }

            if (this.isDown == true && this.downTime > 0.5f)
            {
                this.downRange = InputHandlerUtility.SelectWord(grid, this.downPoint);
                this.isDown = false;
                this.isSelecting = true;
                this.isScrolling = false;
                this.UpdateSelecting();
            }

            if (this.isScrolling == true && this.isDown == false)
            {
                if (grid.MaximumVisibleIndex > grid.MinimumVisibleIndex)
                {
                    this.scrollPos += this.scrollDelta * Time.deltaTime;
                    this.scrollPos = Math.Max(this.scrollPos, grid.MinimumVisibleIndex);
                    this.scrollPos = Math.Min(this.scrollPos, grid.MaximumVisibleIndex);
                    this.scrollDelta = Mathf.Lerp(this.scrollDelta, 0, this.scrollSpeed * Time.deltaTime);
                    grid.VisibleIndex = (int)this.scrollPos;
                    if (Mathf.Abs(this.scrollDelta) < 0.1f)
                    {
                        this.isScrolling = false;
                        this.scrollDelta = 0.0f;
                    }
                }
            }

            // if (Input.touchCount > 0)
            // {
            //     // Debug.Log(Input.touchCount);
            //     Touch touch = Input.GetTouch(0);
            //     if (touch.phase == TouchPhase.Began)
            //     {
            //         startTime = Time.time;
            //         startPos = touch.position;
            //     }
            //     else if (touch.phase == TouchPhase.Ended)
            //     {
            //         endTime = Time.time;
            //         endPos = touch.position;

            //         swipeDistance = (endPos - startPos).magnitude;
            //         swipeTime = endTime - startTime;

            //         if (swipeTime < maxTime && swipeDistance > minSwipeDist)
            //         {
            //             SwipeFunc();
            //         }
            //     }
            // }

            this.swiper.Update();

            if (this.keyboard != null)
            {
                // Debug.Log(i);
                if (this.keyboard.status == TouchScreenKeyboard.Status.Done)
                {
                    this.Terminal.Command = this.keyboard.text;
                    this.Terminal.Execute();
                    this.keyboard = null;
                    this.scrollPos = (int)this.Grid.VisibleIndex;
                    this.Grid.SetPosition(this.gridPosition);
                    // var gameObject = this.Grid.GameObject;
                    // var rectTransform = gameObject.GetComponent<RectTransform>();
                    // rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
                }
                else if (this.keyboard.status == TouchScreenKeyboard.Status.Canceled)
                {
                    Debug.Log(TouchScreenKeyboard.area);
                    // this.Terminal.Command = this.keyboard.text;
                    this.keyboard = null;
                    this.scrollPos = (int)this.Grid.VisibleIndex;
                    var gameObject = this.Grid.GameObject;
                    var rectTransform = gameObject.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
                }
                else if (this.keyboard.status == TouchScreenKeyboard.Status.Visible)
                {
                    // Debug.Log(TouchScreenKeyboard.area);
                    this.Terminal.Command = this.keyboard.text;
                    if (this.keyboard.selection.length == 0)
                    {
                        this.Terminal.CursorPosition = this.keyboard.selection.start;
                    }
                    else
                    {
                        this.Terminal.CursorPosition = this.keyboard.selection.start;
                        var index1 = this.Terminal.CursorPosition + this.Terminal.OutputText.Length + this.Terminal.Prompt.Length;
                        var index2 = index1 + this.keyboard.selection.length;
                        var point1 = this.Grid.IndexToPoint(index1);
                        var point2 = this.Grid.IndexToPoint(index2);
                        this.Grid.Selections.Clear();
                        this.Grid.Selections.Add(new TerminalRange(point1, point2));
                        // this.Grid.sele
                    }
                    var point = this.Grid.CursorPoint;
                    var height = this.Grid.Font.Height;
                    var index = point.Y - this.Grid.VisibleIndex;
                    var i = index * height;
                    var y = Math.Floor(TouchScreenKeyboard.area.y / this.Grid.Font.Height) * this.Grid.Font.Height;
                    Debug.Log($"{i}, {y}");
                    if (i >= y)
                    {
                        var gameObject = this.Grid.GameObject;
                        var rectTransform = gameObject.GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, i - (float)y + this.Grid.Font.Height);
                    }
                    // Debug.Log($"{this.keyboard.selection.start}: {this.keyboard.selection.length}");
                }
                // else
                {

                }
            }
        }

        public override void Attach(ITerminalGrid grid)
        {
            base.Attach(grid);
            this.Terminal.Executed += Terminal_Executed;
            this.Terminal.PromptTextChanged += Terminal_PromptTextChanged;
            this.swiper.Swiped += Swiper_Swiped;
        }

        public override void Detach(ITerminalGrid grid)
        {
            this.Terminal.Executed -= Terminal_Executed;
            this.Terminal.PromptTextChanged -= Terminal_PromptTextChanged;
            this.swiper.Swiped -= Swiper_Swiped;
            base.Detach(grid);
        }

        private void OnLeftPointerDown(PointerEventData eventData)
        {
            var grid = this.Grid;
            var terminal = this.Terminal;
            var newPosition = InputHandlerUtility.WorldToGrid(grid, eventData.position);
            var newPoint = InputHandlerUtility.Intersect(grid, newPosition);
            var newTime = Time.time;
            var downCount = GetDownCount(this.downCount, this.clickThreshold, this.time, newTime, this.downPosition, newPosition);
            eventData.useDragThreshold = false;
            this.Focus();
            this.downPosition = newPosition;
            this.downPoint = newPoint;
            this.downCount = downCount;
            this.dragRange = new TerminalRange(newPoint, newPoint);
            this.time = newTime;
            this.isDown = true;
            this.downTime = 0.0f;

            if (newPoint != TerminalPoint.Invalid)
            {
                if (this.isScrolling == true)
                {
                    this.isScrolling = false;
                    this.scrollDelta = 0.0f;
                    this.downPoint = TerminalPoint.Invalid;
                    this.downCount = 0;
                }
                else if (downCount == 1)
                {
                    this.SelectingRange = TerminalRange.Empty;
                    this.Selections.Clear();
                    this.downRange = InputHandlerUtility.UpdatePoint(grid, newPoint, newPoint);
                }
                else if (downCount == 2)
                {
                    this.downRange = InputHandlerUtility.SelectWord(grid, newPoint);
                    this.UpdateSelecting();
                }
                else if (downCount == 3)
                {
                    this.downRange = InputHandlerUtility.SelectLine(grid, newPoint);
                    this.UpdateSelecting();
                }
            }
        }

        private void OnLeftPointerUp(PointerEventData eventData)
        {
            var grid = this.Grid;
            var position = InputHandlerUtility.WorldToGrid(grid, eventData.position);
            var newPoint1 = InputHandlerUtility.Intersect(grid, position);
            var newPoint2 = new TerminalPoint(newPoint1.X + 1, newPoint1.Y);
            var oldPoint = this.downPoint;
            if (oldPoint == newPoint1)
            {
                this.Selections.Clear();
                this.Selections.Add(this.SelectingRange);
                this.SelectingRange = TerminalRange.Empty;
                if (this.downCount == 1 && this.isSelecting == false)
                {
                    var row = this.Grid.Rows[newPoint1.Y];
                    var cell = row.Cells[newPoint1.X];
                    if (cell.TextIndex < 0)
                    {
                        var cursorPosition = this.Terminal.CursorPosition;
                        if (cursorPosition != this.Terminal.Command.Length)
                        {
                            this.Terminal.MoveToLast();
                            this.Grid.ScrollToCursor();
                        }
                        else
                        {
                            var gridPosition = TerminalGridUtility.GetPosition(this.Grid);
                            this.Grid.ScrollToCursor();
                            this.keyboard = TouchScreenKeyboard.Open(this.Terminal.Command, TouchScreenKeyboardType.Default, false, false, false, false, "abc");
                            this.gridPosition = gridPosition;
                        }
                    }
                    else
                    {
                        var min = this.Terminal.OutputText.Length;
                        var max = min + this.Terminal.Command.Length;
                        var index = cell.TextIndex - min;
                        if (cell.TextIndex < this.Terminal.OutputText.Length)
                        {
                            this.SelectingRange = TerminalRange.Empty;
                            this.Selections.Clear();
                            this.downRange = new TerminalRange(newPoint1, newPoint1);
                            this.SelectingRange = new TerminalRange(newPoint1, newPoint2);
                        }
                        else
                        {
                            this.Terminal.CursorPosition = index - this.Terminal.Prompt.Length;
                        }
                    }
                }
                else if (this.downCount == 2)
                {
                    var range = InputHandlerUtility.SelectGroup(grid, newPoint1);
                    InputHandlerUtility.Select(grid, range);
                }
            }
        }

        private void UpdateSelecting()
        {
            var p1 = this.downRange.BeginPoint < this.dragRange.BeginPoint ? this.downRange.BeginPoint : this.dragRange.BeginPoint;
            var p2 = this.downRange.EndPoint > this.dragRange.EndPoint ? this.downRange.EndPoint : this.dragRange.EndPoint;
            this.Selections.Clear();
            this.SelectingRange = new TerminalRange(p1, p2);
        }

        private void Focus() => this.Grid.Focus();

        private void Terminal_Executed(object sender, TerminalExecuteEventArgs e)
        {
            this.scrollPos = (int)this.Grid.VisibleIndex;
        }

        private void Terminal_PromptTextChanged(object sender, EventArgs e)
        {
            if (this.keyboard != null && this.keyboard.text != this.Terminal.Command)
            {
                this.keyboard.text = this.Terminal.Command;
            }
        }

        private void Swiper_Swiped(object sender, SwipedEventArgs e)
        {
            if (this.keyboard != null)
            {
                if (e.Direction == SwipeDirection.Right)
                {
                    Debug.Log("Horizontal swipe right");
                    this.Terminal.NextCompletion();
                    // var gameObject = this.Grid.GameObject;
                    // var rectTransform = gameObject.GetComponent<RectTransform>();
                    // Debug.Log(rectTransform.position.y);
                    // rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 100);
                    // this.keyboard.text = this.Terminal.Command;
                }
                else if (e.Direction == SwipeDirection.Left)
                {
                    Debug.Log("Horizontal swipe left");
                    this.Terminal.PrevCompletion();
                    // var gameObject = this.Grid.GameObject;
                    // var rectTransform = gameObject.GetComponent<RectTransform>();
                    // rectTransform.anchoredPosition = Vector2.zero;
                    // this.keyboard.text = this.Terminal.Command;
                }
            }
        }

        static int GetDownCount(int count, float clickThreshold, float oldTime, float newTime, Vector2 oldPosition, Vector2 newPosition)
        {
            var diffTime = newTime - oldTime;
            if (diffTime > clickThreshold || oldPosition != newPosition)
                return 1;
            return (count % 3) + 1;
        }

        // void SwipeFunc()
        // {
        //     // var gameObject = this.Grid.GameObject;
        //     //         var rectTransform = gameObject.GetComponent<RectTransform>();
        //     //         rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
        //     Vector2 distance = endPos - startPos;
        //     if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        //     {
        //         if (this.keyboard != null)
        //         {
        //             if (startPos.x < endPos.x)
        //             {
        //                 Debug.Log("Horizontal swipe right");
        //                 this.Terminal.NextCompletion();
        //                 // var gameObject = this.Grid.GameObject;
        //                 // var rectTransform = gameObject.GetComponent<RectTransform>();
        //                 // Debug.Log(rectTransform.position.y);
        //                 // rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 100);
        //             }
        //             else
        //             {
        //                 Debug.Log("Horizontal swipe left");
        //                 this.Terminal.PrevCompletion();
        //                 // var gameObject = this.Grid.GameObject;
        //                 // var rectTransform = gameObject.GetComponent<RectTransform>();
        //                 // rectTransform.anchoredPosition = Vector2.zero;
        //             }
        //             this.keyboard.text = this.Terminal.Command;
        //         }
        //     }
        //     else if (Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
        //     {
        //         if (startPos.y < endPos.y)
        //             Debug.Log(" vertical  swipe up");
        //         else
        //             Debug.Log(" vertical  swipe down");
        //     }
        // }

        private TerminalRange SelectingRange
        {
            get => this.Grid.SelectingRange;
            set => this.Grid.SelectingRange = value;
        }

        private IList<TerminalRange> Selections => this.Grid.Selections;
    }
}
