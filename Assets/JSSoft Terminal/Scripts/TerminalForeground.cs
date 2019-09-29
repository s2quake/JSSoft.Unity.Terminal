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
        private TMP_FontAsset fontAsset;
        [SerializeField]
        private TerminalGrid grid;

        private Vector3[] vertices = new Vector3[] { };
        private Vector2[] uvs = new Vector2[] { };
        private Color32[] colors = new Color32[] { };
        private int[] triangles = new int[] { };

        public TerminalForeground()
        {

        }

        public override Texture mainTexture => this.fontAsset?.atlasTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            // this.material.color = base.color;
            // CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
            if (this.fontAsset != null && this.Rows != null && executing == CanvasUpdate.LatePreRender)
            {
                var rect = this.rectTransform.rect;
                var itemWidth = FontUtility.GetItemWidth(this.fontAsset);
                var itemHeight = (int)this.fontAsset.faceInfo.lineHeight;
                var columnCount = (int)(rect.width / itemWidth);
                var rowCount = (int)(rect.height / itemHeight);
                var vertexCount = rowCount * columnCount * 4 * 2;
                if (vertexCount > this.vertices.Length)
                {
                    this.vertices = new Vector3[vertexCount];
                    this.uvs = new Vector2[vertexCount];
                    this.colors = new Color32[vertexCount];
                    this.triangles = new int[vertexCount / 4 * 6];
                }

                var index = 0;
                var query = from row in this.Rows
                            from cell in row.Cells
                            where cell.Character != 0
                            select cell;

                foreach (var item in query)
                {
                    this.vertices.SetVertex(index, item.ForegroundRect);
                    this.vertices.Transform(index, rect);
                    this.uvs.SetUV(index, item.ForegroundUV);
                    if (item.ForegroundColor is Color32 color)
                        this.colors.SetColor(index, color);
                    index += 4;
                }

                Array.Clear(this.triangles, 0, this.triangles.Length);
                this.triangles.SetTriangles(0, 0, index / 4);

                this.Mesh.Clear();
                this.Mesh.vertices = this.vertices;
                this.Mesh.uv = this.uvs;
                this.Mesh.colors32 = this.colors;
                this.Mesh.triangles = this.triangles;
                this.canvasRenderer.SetMesh(this.Mesh);
            }
        }

        public TerminalRow[] Rows => this.grid?.Rows;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // this.material = new Material(Shader.Find("TextMeshPro/Distance Field"));
            // this.material.color = base.color;
            this.SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // this.gameObject.GetComponentsInChildren
        }

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
