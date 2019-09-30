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

        public override Texture mainTexture => this.fontAsset?.atlasTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var rect = this.rectTransform.rect;
            var query = from row in this.Rows
                        from cell in row.Cells
                        where cell.BackgroundColor is Color32
                        select cell;
            var index = 0;
            this.terminalRect.Count = query.Count();
            foreach (var item in query)
            {
                this.terminalRect.SetVertex(index, item.BackgroundRect, rect);
                this.terminalRect.SetUV(index, item.BackgroundUV);
                if (item.BackgroundColor is Color32 color)
                    this.terminalRect.SetColor(index, color);
                index++;
            }
            this.material.color = base.color;
            this.terminalRect.Fill(vh);
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
        }

        public TerminalRow[] Rows => this.grid != null ? this.grid.Rows : new TerminalRow[] { };

        public int ColumnCount => this.grid != null ? this.grid.ColumnCount : 0;

        public int RowCount => this.grid != null ? this.grid.RowCount : 0;

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
            this.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            this.material.color = base.color;
            this.SetVerticesDirty();
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // this.gameObject.GetComponentsInChildren
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
        }
    }
}
