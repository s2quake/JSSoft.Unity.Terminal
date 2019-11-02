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
    class MacOSInputHandlerContext : InputHandlerContext
    {
        private TerminalPoint downPoint;
        private TerminalPoint beginPoint;
        private TerminalPoint endPoint;

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
            var grid = this.Grid;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var position = grid.WorldToGrid(eventData.position);
                this.downPoint = grid.Intersect(position);
                if (this.downPoint != TerminalPoint.Invalid)
                {
                    grid.Selections.Clear();
                }
                eventData.useDragThreshold = false;
                grid.Focus();
                return true;
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
    }
}
