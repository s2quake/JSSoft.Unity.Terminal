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
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.UI
{
    class TerminalComposition : MaskableGraphic
    {
        private static readonly int[] backgroundTriangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        private static readonly int[] foregroundTriangles = new int[6] { 4, 5, 6, 6, 7, 4 };

        [SerializeField]
        private string text = string.Empty;
        [SerializeField]
        private Color foregroundColor = Color.white;
        [SerializeField]
        private Color backgroundColor = new Color(0, 0, 0, 0);
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private int columnIndex;
        [SerializeField]
        private int rowIndex;

        private new Material material;
        private Texture texture;
        private Vector3[] vertices = new Vector3[8];
        private Vector2[] uvs = new Vector2[8];
        private Color32[] colors = new Color32[8];
        private Mesh mesh;

        public TerminalComposition()
        {

        }

        public override void Rebuild(CanvasUpdate update)
        {
            base.Rebuild(update);
            if (update == CanvasUpdate.LatePreRender)
            {
                this.UpdateGeometry();
            }
        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
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
                if (value < 0 || value >= this.BufferWidth)
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
                if (value < 0 || value >= this.BufferHeight)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.rowIndex = value;
                this.SetVerticesDirty();
            }
        }

        public Color ForegroundColor
        {
            get => this.foregroundColor;
            set
            {
                this.foregroundColor = value;
            }
        }

        public Color BackgroundColor
        {
            get => this.backgroundColor;
            set
            {
                this.backgroundColor = value;
                base.color = value;
            }
        }

        public Vector2 Offset { get; set; } = new Vector2(2, 0);

        public override Texture mainTexture => this.texture;

        public TerminalFont Font => this.grid?.Font;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.columnIndex = Math.Min(this.BufferWidth - 1, this.columnIndex);
            this.columnIndex = Math.Max(0, this.columnIndex);
            this.rowIndex = Math.Min(this.BufferHeight - 1, this.rowIndex);
            this.rowIndex = Math.Max(0, this.rowIndex);
            base.color = this.backgroundColor;
            base.material.color = base.color;
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            this.mesh = new Mesh();
            base.material = new Material(Shader.Find("Unlit/Color"));
            base.material.color = base.color;
            this.material = new Material(Shader.Find("UI/Default"));
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalStyleEvents.Validated += Style_Validated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.mesh = null;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalStyleEvents.Validated -= Style_Validated;
        }

        protected override void UpdateGeometry()
        {
            base.UpdateGeometry();

            if (this.columnIndex < this.BufferWidth && this.rowIndex < this.BufferHeight && this.text != string.Empty)
            {
                var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, false);
                var character = this.text.First();
                var characterInfo = this.Font[character];
                var texture = characterInfo.Texture;
                var itemWidth = TerminalGridUtility.GetItemWidth(this.grid);
                var itemHeight = TerminalGridUtility.GetItemHeight(this.grid);
                var padding = TerminalGridUtility.GetPadding(this.grid);
                var bx = this.columnIndex * itemWidth + padding.Left + (int)this.Offset.x;
                var by = this.rowIndex * itemHeight + padding.Top + (int)this.Offset.y;
                var foregroundRect = FontUtility.GetForegroundRect(this.Font, character, bx, by);
                var backgroundRect = new Rect(bx, by, itemWidth, itemHeight);
                var uv = FontUtility.GetUV(this.Font, character);

                this.vertices.SetVertex(0, backgroundRect);
                this.vertices.Transform(0, rect);
                this.vertices.SetVertex(4, foregroundRect);
                this.vertices.Transform(4, rect);
                this.uvs.SetUV(0, Vector2.zero, Vector2.zero);
                this.uvs.SetUV(4, uv);
                this.colors.SetColor(0, this.backgroundColor);
                this.colors.SetColor(4, this.foregroundColor);
                this.texture = texture;

                this.mesh.Clear();
                this.mesh.subMeshCount = 2;
                this.mesh.vertices = this.vertices;
                this.mesh.uv = this.uvs;
                this.mesh.colors32 = this.colors;
                this.mesh.SetTriangles(backgroundTriangles, 0);
                this.mesh.SetTriangles(foregroundTriangles, 1);

                this.canvasRenderer.materialCount = 2;
                this.canvasRenderer.SetTexture(this.texture);
                this.canvasRenderer.SetMaterial(base.material, 0);
                this.canvasRenderer.SetMaterial(this.material, 1);
                this.canvasRenderer.SetMesh(this.mesh);
            }
            else
            {
                this.mesh.Clear();
                this.canvasRenderer.SetMesh(this.mesh);
                this.texture = null;
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (object.Equals(sender, this.grid) == false)
                return;

            switch (e.PropertyName)
            {
                case nameof(ITerminalGrid.CursorPoint):
                    {
                        var cursorPoint = this.grid.CursorPoint;
                        var visibleIndex = this.grid.VisibleIndex;
                        this.columnIndex = cursorPoint.X;
                        this.rowIndex = cursorPoint.Y - visibleIndex;
                        this.SetVerticesDirty();
                    }
                    break;
                case nameof(ITerminalGrid.CompositionString):
                    {
                        this.text = this.grid.CompositionString;
                        this.SetVerticesDirty();
                    }
                    break;
                case nameof(ITerminalGrid.CompositionColor):
                    {
                        this.UpdateColor();
                    }
                    break;
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && this.grid == grid)
            {
                this.UpdateColor();
            }
        }

        private async void Style_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalStyle style == this.grid?.Style)
            {
                await Task.Delay(1);
                this.UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (this.IsDestroyed() == true)
                return;
            this.foregroundColor = this.grid.CompositionColor;
            this.SetVerticesDirty();
        }

        private int BufferWidth => this.grid != null ? this.grid.BufferWidth : 0;

        private int BufferHeight => this.grid != null ? this.grid.BufferHeight : 0;
    }
}
