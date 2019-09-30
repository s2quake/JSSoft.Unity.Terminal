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
using UnityEngine.EventSystems;

namespace JSSoft.UI
{
    public class TerminalGrid : MaskableGraphic
    {
        [SerializeField]
        private TMP_FontAsset fontAsset;
        [SerializeField]
        private string text = "123";

        public int ColumnCount { get; private set; }

        public int RowCount { get; private set; }

        public TerminalRow[] Rows { get; private set; } = new TerminalRow[] { };

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
            Debug.Log($"{nameof(Rebuild)}: {executing}");
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Debug.Log(nameof(OnValidate));

            for (var i = 0; i < this.rectTransform.childCount; i++)
            {
                var childTransform = this.rectTransform.GetChild(i);
                var childGraphic = childTransform.GetComponent<Graphic>();
                if (childGraphic != null)
                {
                    CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(childGraphic);
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Debug.Log(nameof(OnRectTransformDimensionsChange));
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            Debug.Log(nameof(OnPopulateMesh));

            if (this.fontAsset != null)
            {
                var rect = this.rectTransform.rect;
                var itemWidth = FontUtility.GetItemWidth(this.fontAsset);
                var itemHeight = (int)this.fontAsset.faceInfo.lineHeight;
                this.ColumnCount = (int)(rect.width / itemWidth);
                this.RowCount = (int)(rect.height / itemHeight);
                this.Rows = FontUtility.GenerateTerminalRows(this.fontAsset, this.text, this.ColumnCount);
            }
            else
            {
                this.Rows = new TerminalRow[] { };
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            this.material.color = base.color;



            this.SetVerticesDirty();
        }
    }
}
