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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalBackground : MaskableGraphic
    {
        [SerializeField]
        private TMP_FontAsset fontAsset;
        [SerializeField]
        private TerminalGrid grid;

        private TerminalRect terminalRect = new TerminalRect();

        public TerminalBackground()
        {

        }

        // public override Texture mainTexture => this.fontAsset?.atlasTexture;

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
        }

        private bool Predicate(TerminalCell cell)
        {
            if (cell.BackgroundColor is Color32)
                return true;
            return TerminalGrid.IsSelecting(this.grid, cell);

        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var rect = TerminalGrid.TransformRect(this.grid, this.rectTransform.rect);
            var visibleCells = TerminalGrid.GetVisibleCells(this.grid, this.Predicate);
            var index = 0;
            this.terminalRect.Count = visibleCells.Count();
            foreach (var item in visibleCells)
            {
                this.terminalRect.SetVertex(index, item.BackgroundRect, rect);
                this.terminalRect.SetUV(index, item.BackgroundUV);
                if (item.BackgroundColor is Color32 color)
                    this.terminalRect.SetColor(index, color);
                else
                    this.terminalRect.SetColor(index, TerminalColors.Green);
                index++;
            }
            this.material.color = base.color;
            this.terminalRect.Fill(vh);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Debug.Log($"{nameof(TerminalBackground)}.{nameof(OnRectTransformDimensionsChange)}");
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            Debug.Log(nameof(OnCanvasHierarchyChanged));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("TextMeshPro/Bitmap"));
            this.material.color = base.color;
            this.SetVerticesDirty();
            if (this.grid != null)
            {
                this.grid.TextChanged += TerminalGrid_TextChanged;
                this.grid.LayoutChanged += TerminalGrid_LayoutChanged;
                this.grid.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
                this.grid.SelectionChanged += TerminalGrid_SelectionChanged;
            }
            Debug.Log($"{nameof(TerminalBackground)}.{nameof(OnEnable)}");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.grid != null)
            {
                this.grid.TextChanged -= TerminalGrid_TextChanged;
                this.grid.LayoutChanged -= TerminalGrid_LayoutChanged;
                this.grid.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
                this.grid.SelectionChanged -= TerminalGrid_SelectionChanged;
            }
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            this.SetVerticesDirty();
        }

        private void TerminalGrid_LayoutChanged(object sender, EventArgs e)
        {
            this.SetVerticesDirty();
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            this.SetVerticesDirty();
        }

        private void TerminalGrid_SelectionChanged(object sender, EventArgs e)
        {
            this.SetVerticesDirty();
        }
    }
}
