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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Terminal
{
    class TerminalBackground : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;

        private readonly TerminalMesh terminalMesh = new TerminalMesh();

        public TerminalBackground()
        {

        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, true);
            var visibleCells = TerminalGridUtility.GetVisibleCells(this.grid, this.Predicate);
            var index = 0;
            var selectionColor = TerminalGridUtility.GetSelectionColor(this.grid);
            this.terminalMesh.Count = visibleCells.Count();
            foreach (var item in visibleCells)
            {
                this.terminalMesh.SetVertex(index, item.BackgroundRect, rect);
                this.terminalMesh.SetUV(index, item.BackgroundUV);
                if (item.BackgroundColor is Color32 color)
                    this.terminalMesh.SetColor(index, color);
                else
                    this.terminalMesh.SetColor(index, selectionColor);
                index++;
            }
            this.material.color = base.color;
            this.terminalMesh.Fill(vh);
        }


        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.SetVerticesDirty();
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalEvents.Validated += Terminal_Validated;
            TerminalGridEvents.Validated += Grid_Validated;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.SelectionChanged += Grid_SelectionChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalEvents.Validated -= Terminal_Validated;
            TerminalGridEvents.Validated -= Grid_Validated;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.SelectionChanged -= Grid_SelectionChanged;
        }

        private bool Predicate(ITerminalCell cell)
        {
            if (cell.BackgroundColor is Color32)
                return true;
            return TerminalGridUtility.IsSelecting(this.grid, cell);
        }

        private void Terminal_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void Grid_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetAllDirty();
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (object.Equals(sender, this.grid) == false)
                return;

            var propertyName = e.PropertyName;
            if (propertyName == nameof(ITerminalGrid.VisibleIndex))
            {
                this.SetVerticesDirty();
            }
            else if (propertyName == nameof(ITerminalGrid.Text))
            {
                this.SetVerticesDirty();
            }
            else if (propertyName == nameof(ITerminalGrid.SelectingRange))
            {
                this.SetVerticesDirty();
            }
        }

        private void Grid_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }
    }
}
