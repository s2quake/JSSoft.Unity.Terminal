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
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalBackground : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;

        private readonly TerminalRect terminalRect = new TerminalRect();
        private Rect rect;

        public TerminalBackground()
        {

        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var rect = TerminalGridUtility.TransformRect(this.grid, this.rect, true);
            // Debug.Log($"rect: {this.rect}");

            var visibleCells = TerminalGridUtility.GetVisibleCells(this.grid, this.Predicate);
            var index = 0;
            var selectionColor = TerminalGridUtility.GetSelectionColor(this.grid);
            this.terminalRect.Count = visibleCells.Count();
            // Debug.Log($"visible cells: {visibleCells.Count()}");
            foreach (var item in visibleCells)
            {
                // Debug.Log($"123: {item.BackgroundRect}");
                this.terminalRect.SetVertex(index, item.BackgroundRect, rect);
                this.terminalRect.SetUV(index, item.BackgroundUV);
                if (item.BackgroundColor is Color32 color)
                    this.terminalRect.SetColor(index, color);
                else
                    this.terminalRect.SetColor(index, selectionColor);
                index++;
            }
            this.material.color = base.color;
            this.terminalRect.Fill(vh);
        }


        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            // Debug.Log("OnTransformParentChanged()");
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.rect = this.rectTransform.rect;
            this.SetVerticesDirty();
            // Debug.Log($"OnRectTransformDimensionsChange(): {this.rectTransform.rect}");
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            // Debug.Log("OnCanvasHierarchyChanged()");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.TextChanged += TerminalGrid_TextChanged;
            TerminalGridEvents.LayoutChanged += TerminalGrid_LayoutChanged;
            TerminalGridEvents.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
            TerminalGridEvents.SelectionChanged += TerminalGrid_SelectionChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalGridEvents.TextChanged -= TerminalGrid_TextChanged;
            TerminalGridEvents.LayoutChanged -= TerminalGrid_LayoutChanged;
            TerminalGridEvents.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
            TerminalGridEvents.SelectionChanged -= TerminalGrid_SelectionChanged;
        }

        private bool Predicate(ITerminalCell cell)
        {
            if (cell.BackgroundColor is Color32)
                return true;
            return TerminalGridUtility.IsSelecting(this.grid, cell);
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void TerminalGrid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void TerminalGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }
    }
}
