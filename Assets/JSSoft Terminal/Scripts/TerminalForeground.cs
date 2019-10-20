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
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalForeground : MaskableGraphic
    {
        [SerializeField]
        private TMP_FontAsset fontAsset = null;
        [SerializeField]
        private TerminalGrid grid = null;

        private TerminalRect terminalRect = new TerminalRect();

        public TerminalForeground()
        {

        }

        public override Texture mainTexture => this.fontAsset?.atlasTexture;

        public TerminalForeground Parent => this.GetComponentInParent<TerminalForeground>();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var renderCount = 2;
            var rect = TerminalGrid.TransformRect(this.grid, this.rectTransform.rect);
            var visibleCells = TerminalGrid.GetVisibleCells(this.grid, item => item.Character != 0 && item.FontAsset == this.fontAsset);
            var index = 0;
            this.terminalRect.Count = visibleCells.Count() * renderCount;
            foreach (var item in visibleCells)
            {
                for (var i = 0; i < renderCount; i++)
                {
                    this.terminalRect.SetVertex(index, item.ForegroundRect, rect);
                    this.terminalRect.SetUV(index, item.ForegroundUV);
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
            if (this.grid != null)
            {
                this.grid.TextChanged += TerminalGrid_TextChanged;
                this.grid.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
            }
            this.SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.grid != null)
            {
                this.grid.TextChanged -= TerminalGrid_TextChanged;
                this.grid.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
            }
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            this.SetVerticesDirty();
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            this.SetVerticesDirty();
        }

        private IEnumerable<TMP_FontAsset> FallbackFontAssets
        {
            get
            {
                if (this.fontAsset != null && this.fontAsset.fallbackFontAssetTable != null)
                {
                    foreach (var item in this.fontAsset.fallbackFontAssetTable)
                    {
                        yield return item;
                    }
                }
            }
        }

        private IEnumerable<TerminalForeground> FallbackComponents
        {
            get
            {
                for (var i = 0; i < this.rectTransform.childCount; i++)
                {
                    var childTransform = this.rectTransform.GetChild(i);
                    if (childTransform.GetComponent<TerminalForeground>() is TerminalForeground component)
                    {
                        yield return component;
                    }
                }
            }
        }
    }
}
