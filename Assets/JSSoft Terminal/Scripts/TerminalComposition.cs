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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalComposition : MaskableGraphic
    {
        [SerializeField]
        private char character = '최';
        [SerializeField]
        private TerminalGrid grid;
        [SerializeField]
        private int columnIndex;
        [SerializeField]
        private int rowIndex;
        private TerminalRect terminalRect = new TerminalRect();
        private Texture texture;

        public TerminalComposition()
        {

        }

        public char Character
        {
            get => this.character;
            set
            {
                this.character = value;
                this.SetVerticesDirty();
            }
        }

        public int ColumnIndex
        {
            get => this.columnIndex;
            set
            {
                if (value < 0 || value >= this.ColumnCount)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.columnIndex = value;
                this.SetVerticesDirty();
            }
        }

        public int RowIndex
        {
            get => this.rowIndex;
            set
            {
                if (value < 0 || value >= this.RowCount)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.rowIndex = value;
                this.SetVerticesDirty();
            }
        }

        public Vector2 Offset { get; set; }

        public override Texture mainTexture => this.texture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            if (this.columnIndex < this.ColumnCount && this.rowIndex < this.RowCount)
            {
                var rect = TerminalGrid.TransformRect(this.grid, this.rectTransform.rect);
                var itemWidth = TerminalGrid.GetItemWidth(this.grid);
                var itemHeight = TerminalGrid.GetItemHeight(this.grid);
                var x = this.columnIndex * itemWidth;
                var y = this.rowIndex * itemHeight;
                var fontAsset = FontUtility.GetFontAsset(this.grid.FontAsset, this.character);
                var itemRect = FontUtility.GetForegroundRect(fontAsset, this.character, x, y);
                var uv = FontUtility.GetUV(fontAsset, this.character);
                this.terminalRect.Count = 1;
                this.terminalRect.SetVertex(0, itemRect, rect);
                this.terminalRect.SetUV(0, uv);
                this.terminalRect.SetColor(0, TerminalColors.White);
                this.texture = fontAsset.atlasTexture;
            }
            else
            {
                this.terminalRect.Count = 0;
                this.texture = null;
            }
            this.material.color = base.color;
            this.terminalRect.Fill(vh);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.columnIndex = Math.Min(this.ColumnCount - 1, this.columnIndex);
            this.columnIndex = Math.Max(0, this.columnIndex);
            this.rowIndex = Math.Min(this.RowCount - 1, this.rowIndex);
            this.rowIndex = Math.Max(0, this.rowIndex);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("TextMeshPro/Bitmap"));
            this.material.color = base.color;
        }

        private int ColumnCount => this.grid != null ? this.grid.ColumnCount : 0;

        private int RowCount => this.grid != null ? this.grid.RowCount : 0;
    }
}
