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
    public class TerminalComposition : MaskableGraphic
    {
        private static readonly int[] backgroundTriangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        private static readonly int[] foregroundTriangles = new int[6] { 4, 5, 6, 6, 7, 4 };

        [SerializeField]
        private string text = string.Empty;
        [SerializeField]
        private Color fontColor = Color.black;
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private int columnIndex;
        [SerializeField]
        private int rowIndex;

        private Texture texture;
        private Vector3[] vertices = new Vector3[8];
        private Vector2[] uvs = new Vector2[8];
        private Color32[] colors = new Color32[8];

        public TerminalComposition()
        {

        }

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value ?? throw new ArgumentNullException(nameof(value));
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

        public override void Rebuild(CanvasUpdate update)
        {
            base.Rebuild(update);
            if (update == CanvasUpdate.LatePreRender)
            {
                this.UpdateGeometry();
            }
        }

        public Vector2 Offset { get; set; }

        public override Texture mainTexture => this.texture;

        public TMP_FontAsset FontAsset => this.grid?.FontAsset;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.columnIndex = Math.Min(this.ColumnCount - 1, this.columnIndex);
            this.columnIndex = Math.Max(0, this.columnIndex);
            this.rowIndex = Math.Min(this.RowCount - 1, this.rowIndex);
            this.rowIndex = Math.Max(0, this.rowIndex);
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("Unlit/Color"));
            this.material.color = base.color;
            if (this.grid != null)
            {
                this.grid.CursorPositionChanged += TerminalGrid_CursorPositionChanged;
                this.grid.CompositionStringChanged += TerminalGrid_CompositionStringChanged;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.grid != null)
            {
                this.grid.CursorPositionChanged -= TerminalGrid_CursorPositionChanged;
                this.grid.CompositionStringChanged -= TerminalGrid_CompositionStringChanged;
            }
        }

        private void TerminalGrid_CursorPositionChanged(object sender, EventArgs e)
        {
            this.columnIndex = this.grid.CursorLocation.X;
            this.rowIndex = this.grid.CursorLocation.Y - this.grid.VisibleIndex;
            this.SetVerticesDirty();
        }

        private void TerminalGrid_CompositionStringChanged(object sender, EventArgs e)
        {
            this.text = this.grid.CompositionString;
            this.SetVerticesDirty();
        }

        protected override void UpdateGeometry()
        {
            base.UpdateGeometry();

            if (this.columnIndex < this.ColumnCount && this.rowIndex < this.RowCount && this.text != string.Empty)
            {
                var rect = this.rectTransform.rect;
                var character = this.text.First();
                var fontAsset = FontUtility.GetFontAsset(this.FontAsset, character);
                var characterInfo = fontAsset.characterLookupTable[character];
                var texture = fontAsset.atlasTexture;
                var itemWidth = TerminalGrid.GetItemWidth(this.grid);
                var itemHeight = TerminalGrid.GetItemHeight(this.grid);
                var bx = this.columnIndex * itemWidth;
                var by = this.rowIndex * itemHeight;
                var foregroundRect = FontUtility.GetForegroundRect(fontAsset, character, bx, by);
                var backgroundRect = foregroundRect.Expand(1);
                var uv = FontUtility.GetUV(fontAsset, character);

                this.vertices.SetVertex(0, backgroundRect);
                this.vertices.Transform(0, rect);
                this.vertices.SetVertex(4, foregroundRect);
                this.vertices.Transform(4, rect);
                this.uvs.SetUV(0, Vector2.zero, Vector2.zero);
                this.uvs.SetUV(4, uv);
                this.colors.SetColor(0, base.color);
                this.colors.SetColor(4, this.fontColor);
                this.texture = texture;

                this.Mesh.Clear();
                this.Mesh.subMeshCount = 2;
                this.Mesh.vertices = this.vertices;
                this.Mesh.uv = this.uvs;
                this.Mesh.colors32 = this.colors;
                this.Mesh.SetTriangles(backgroundTriangles, 0);
                this.Mesh.SetTriangles(foregroundTriangles, 1);

                this.canvasRenderer.materialCount = 2;
                this.canvasRenderer.SetTexture(this.texture);
                this.canvasRenderer.SetMaterial(this.material, 0);
                this.canvasRenderer.SetMaterial(fontAsset.material, 1);
                this.canvasRenderer.SetMesh(this.Mesh);
            }
            else
            {
                this.Mesh.Clear();
                this.canvasRenderer.SetMesh(this.Mesh);
                this.texture = null;
            }
        }

        private int ColumnCount => this.grid != null ? this.grid.ColumnCount : 0;

        private int RowCount => this.grid != null ? this.grid.RowCount : 0;

        private Mesh Mesh
        {
            get
            {
                if (m_CachedMesh == null)
                {
                    m_CachedMesh = new Mesh();
                }
                return m_CachedMesh;
            }
        }
    }
}
