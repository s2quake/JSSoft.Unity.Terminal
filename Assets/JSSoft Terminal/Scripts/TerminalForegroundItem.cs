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
    class TerminalForegroundItem : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private Texture2D texture;

        private TerminalRect terminalRect = new TerminalRect();

        public TerminalForegroundItem()
        {

        }

        public override Texture mainTexture => this.texture;

        public TerminalFont Font => this.grid?.Font;

        public TerminalGrid Grid
        {
            get => this.grid;
            internal set => this.grid = value;
        }

        public Texture2D Texture
        {
            get => this.texture;
            internal set => this.texture = value;
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, true);
            var visibleCells = TerminalGridUtility.GetVisibleCells(this.grid, item => item.Character != 0 && item.Texture == this.texture);
            var index = 0;
            this.terminalRect.Count = visibleCells.Count();
            foreach (var item in visibleCells)
            {
                this.terminalRect.SetVertex(index, item.ForegroundRect, rect);
                this.terminalRect.SetUV(index, item.ForegroundUV);
                this.terminalRect.SetColor(index, TerminalCell.GetForegroundColor(item));
                index++;
            }
            this.material.color = base.color;
            this.terminalRect.Fill(vh);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            base.material = new Material(Shader.Find("UI/Default"));
            TerminalGridEvents.TextChanged += TerminalGrid_TextChanged;
            TerminalGridEvents.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.TextChanged -= TerminalGrid_TextChanged;
            TerminalGridEvents.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
            base.OnDisable();
        }

        protected override void Start()
        {
            this.SetVerticesDirty();
        }

        protected override void OnDestroy()
        {
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }
    }
}