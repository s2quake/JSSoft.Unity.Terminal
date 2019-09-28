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
    public class CompositionRenderer : MaskableGraphic
    {
        private static readonly int[] backgroundTriangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        private static readonly int[] foregroundTriangles = new int[6] { 4, 5, 6, 6, 7, 4 };

        [SerializeField]
        private TMP_FontAsset fontAsset;

        private char character;
        private Texture texture;
        private Material material;
        private Vector3[] vertices = new Vector3[8];
        private Vector2[] uvs = new Vector2[8];
        private Color32[] colors = new Color32[8]
        {
            TerminalColors.Green,
            TerminalColors.Green,
            TerminalColors.Green,
            TerminalColors.Green,
            TerminalColors.Black,
            TerminalColors.Black,
            TerminalColors.Black,
            TerminalColors.Black
        };

        public char Character
        {
            get => this.character;
            set
            {
                this.character = value;
                this.SetVerticesDirty();
                this.SetMaterialDirty();
            }
        }

        public TMP_FontAsset FontAsset
        {
            get => this.fontAsset;
            set
            {
                this.fontAsset = value;
            }
        }

        public override Texture mainTexture => this.texture;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material(Shader.Find("Unlit/Color"));
            this.material.color = new Color(1, 0, 1, 1);
            this.SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
            // return;
            if (executing == CanvasUpdate.LatePreRender)
            {
                var rect = this.rectTransform.rect;
                var fontAsset = FontUtility.GetFontAsset(this.fontAsset, this.character);
                if (fontAsset != null)
                {
                    var characterInfo = fontAsset.characterLookupTable[this.character];
                    var texture = fontAsset.atlasTexture;
                    var glyph = characterInfo.glyph;
                    var glyphRect = glyph.glyphRect;
                    var textWidth = (float)texture.width;
                    var textHeight = (float)texture.height;
                    var uv0 = new Vector2(glyphRect.x / textWidth, glyphRect.y / textHeight);
                    var uv1 = new Vector2((glyphRect.x + glyphRect.width) / textWidth, (glyphRect.y + glyphRect.height) / textHeight);
                    var x = rect.x + (rect.width - glyphRect.width) / 2;
                    var y = rect.y + (rect.height - glyphRect.height) / 2;
                    var backgroundRect = new GlyphRect((int)x, (int)y, glyphRect.width, glyphRect.height);

                    VertexUtility.SetVertex(this.vertices, 0, backgroundRect);
                    // this.vertices[0] = new Vector3(x, y, 0);
                    // this.vertices[1] = new Vector3(x, y + glyphRect.height, 0);
                    // this.vertices[2] = new Vector3(x + glyphRect.width, y + glyphRect.height, 0);
                    // this.vertices[3] = new Vector3(x + glyphRect.width, y, 0);
                    this.vertices[4] = new Vector3(x, y, 0);
                    this.vertices[5] = new Vector3(x, y + glyphRect.height, 0);
                    this.vertices[6] = new Vector3(x + glyphRect.width, y + glyphRect.height, 0);
                    this.vertices[7] = new Vector3(x + glyphRect.width, y, 0);

                    this.uvs[0] = new Vector2(0, 0);
                    this.uvs[1] = new Vector2(0, 1);
                    this.uvs[2] = new Vector2(1, 1);
                    this.uvs[3] = new Vector2(1, 0);
                    this.uvs[4] = new Vector2(uv0.x, uv0.y);
                    this.uvs[5] = new Vector2(uv0.x, uv1.y);
                    this.uvs[6] = new Vector2(uv1.x, uv1.y);
                    this.uvs[7] = new Vector2(uv1.x, uv0.y);
                    this.texture = texture;
                }
                else
                {
                    this.vertices[0] = new Vector3(rect.x + 100, rect.y, 0);
                    this.vertices[1] = new Vector3(rect.x + 100, rect.y + rect.height, 0);
                    this.vertices[2] = new Vector3(rect.x - 100 + rect.width, rect.y + rect.height, 0);
                    this.vertices[3] = new Vector3(rect.x - 100 + rect.width, rect.y, 0);
                    this.vertices[4] = new Vector3(rect.x + 200, rect.y, 0);
                    this.vertices[5] = new Vector3(rect.x + 200, rect.y + rect.height, 0);
                    this.vertices[6] = new Vector3(rect.x - 200 + rect.width, rect.y + rect.height, 0);
                    this.vertices[7] = new Vector3(rect.x - 200 + rect.width, rect.y, 0);

                    this.uvs[0] = new Vector2(0, 0);
                    this.uvs[1] = new Vector2(0, 1);
                    this.uvs[2] = new Vector2(1, 1);
                    this.uvs[3] = new Vector2(1, 0);
                    this.uvs[4] = new Vector2(0, 0);
                    this.uvs[5] = new Vector2(0, 1);
                    this.uvs[6] = new Vector2(1, 1);
                    this.uvs[7] = new Vector2(1, 0);
                    this.texture = this.fontAsset?.atlasTexture;
                }
                this.Mesh.Clear();
                this.Mesh.subMeshCount = 2;
                this.Mesh.vertices = vertices;
                this.Mesh.uv = uvs;
                this.Mesh.colors32 = colors;
                this.Mesh.SetTriangles(backgroundTriangles, 0);
                this.Mesh.SetTriangles(foregroundTriangles, 1);
                this.canvasRenderer.materialCount = 2;

                this.canvasRenderer.SetMaterial(this.material, 0);
                this.canvasRenderer.SetMaterial(this.fontAsset?.material, 1);
                this.canvasRenderer.SetMesh(this.Mesh);
            }
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
