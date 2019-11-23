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
using UnityEngine.UI;

namespace JSSoft.UI
{
    class TerminalForegroundItem : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private int page;
        [SerializeField]
        private float x1;
        [SerializeField]
        private float x2 = 1.0f;
        [SerializeField]
        private float y1;
        [SerializeField]
        private float y2 = 1.0f;

        private TerminalRect terminalRect = new TerminalRect();

        public TerminalForegroundItem()
        {

        }

        public override Texture mainTexture
        {
            get
            {
                if (this.Font != null)
                {
                    var textures = this.Font.Textures;
                    if (this.page < textures.Length)
                    {
                        return textures[this.page];
                    }
                }
                return null;
            }
        }

        public TerminalFont Font => this.grid?.Font;

        public TerminalGrid Grid
        {
            get => this.grid;
            internal set => this.grid = value;
        }

        public int Page
        {
            get => this.page;
            internal set => this.page = value;
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
            var renderCount = 1;
            var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, true);
            var visibleCells = TerminalGridUtility.GetVisibleCells(this.grid, item => item.Character != 0 && item.Page == this.page);
            var index = 0;
            this.terminalRect.Count = visibleCells.Count() * renderCount;
            // Debug.Log(visibleCells.Count());
            // Debug.Log(this.mainTexture);
            foreach (var item in visibleCells)
            {
                // for (var i = 0; i < renderCount; i++)
                {
                    this.terminalRect.SetVertex(index, item.ForegroundRect, rect);
                    // this.terminalRect.SetUV(index, (new Vector2(x1, x2), new Vector2(y1, y2)));
                    this.terminalRect.SetUV(index, item.ForegroundUV);
                    // Debug.Log(item.ForegroundUV);
                    this.terminalRect.SetColor(index, TerminalCell.GetForegroundColor(item));
                    index++;
                }
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
