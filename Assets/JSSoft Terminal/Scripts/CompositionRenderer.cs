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
        [SerializeField]
        private TMP_FontAsset fontAsset;

        private char character;
        private Texture texture;
        private Material material;

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

        public override Texture mainTexture => this.texture;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.material = new Material (Shader.Find("Unlit/Color"));
            this.material.color = new Color(1, 0, 1, 1);
            this.SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            // var vertices = new UIVertex[8];
            // var rect = this.rectTransform.rect;

            // vertices[0].color = base.color;
            // vertices[1].color = base.color;
            // vertices[2].color = base.color;
            // vertices[3].color = base.color;
            // vertices[4].color = TerminalColors.Black;
            // vertices[5].color = TerminalColors.Black;
            // vertices[6].color = TerminalColors.Black;
            // vertices[7].color = TerminalColors.Black;

            // var fontAsset = FontUtility.GetFontAsset(this.fontAsset, this.character);
            // if (fontAsset != null)
            // {
            //     var characterInfo = fontAsset.characterLookupTable[this.character];
            //     var texture = fontAsset.atlasTexture;
            //     var glyph = characterInfo.glyph;
            //     var glyphRect = glyph.glyphRect;
            //     var textWidth = (float)texture.width;
            //     var textHeight = (float)texture.height;
            //     var uv0 = new Vector2(glyphRect.x / textWidth, glyphRect.y / textHeight);
            //     var uv1 = new Vector2((glyphRect.x + glyphRect.width) / textWidth, (glyphRect.y + glyphRect.height) / textHeight);
            //     var x = (rect.x + rect.width / 2) - (glyphRect.width / 2);
            //     var y = (rect.y + rect.height / 2) - (glyphRect.height / 2);

            //     vertices[0].position = new Vector3(x, y, 0);
            //     vertices[1].position = new Vector3(x, y + glyphRect.height, 0);
            //     vertices[2].position = new Vector3(x + glyphRect.width, y + glyphRect.height, 0);
            //     vertices[3].position = new Vector3(x + glyphRect.width, y, 0);

            //     vertices[0].uv0 = new Vector2(uv0.x, uv0.y);
            //     vertices[1].uv0 = new Vector2(uv0.x, uv1.y);
            //     vertices[2].uv0 = new Vector2(uv1.x, uv1.y);
            //     vertices[3].uv0 = new Vector2(uv1.x, uv0.y);
            //     this.texture = texture;
            // }
            // else
            // {
            //     vertices[0].position = new Vector3(rect.x + 100, rect.y, 0);
            //     vertices[1].position = new Vector3(rect.x + 100, rect.y + rect.height, 0);
            //     vertices[2].position = new Vector3(rect.x - 100 + rect.width, rect.y + rect.height, 0);
            //     vertices[3].position = new Vector3(rect.x - 100 + rect.width, rect.y, 0);
            //     vertices[4].position = new Vector3(rect.x + 200, rect.y + 100, 0);
            //     vertices[5].position = new Vector3(rect.x + 200, rect.y + rect.height - 100, 0);
            //     vertices[6].position = new Vector3(rect.x - 200 + rect.width, rect.y + rect.height - 100, 0);
            //     vertices[7].position = new Vector3(rect.x - 200 + rect.width, rect.y + 100, 0);

            //     vertices[0].uv0 = new Vector2(0, 0);
            //     vertices[1].uv0 = new Vector2(0, 0);
            //     vertices[2].uv0 = new Vector2(0, 0);
            //     vertices[3].uv0 = new Vector2(0, 0);
            //     vertices[4].uv0 = new Vector2(0, 0);
            //     vertices[5].uv0 = new Vector2(0, 1);
            //     vertices[6].uv0 = new Vector2(1, 1);
            //     vertices[7].uv0 = new Vector2(1, 0);
            //     this.texture = this.fontAsset?.atlasTexture;
            // }
            // vh.Clear();
            // for (var i = 0; i < vertices.Length; i++)
            // {
            //     vh.AddVert(vertices[i]);
            // }
            // vh.AddTriangle(0, 1, 2);
            // vh.AddTriangle(2, 3, 0);
            // vh.AddTriangle(4, 5, 6);
            // vh.AddTriangle(6, 7, 4);
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
            // return;
            if (executing == CanvasUpdate.LatePreRender)
            {
                if (m_CachedMesh == null)
                {
                    m_CachedMesh = new Mesh();
                }
                var rect = this.rectTransform.rect;
                var vertices = new Vector3[8];
                var uvs = new Vector2[8];
                var colors = new Color32[8]
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
                var triangles = new int[12]
                {
                    0, 1, 2, 2, 3, 0,
                    4, 5, 6, 6, 7, 4
                };

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
                    var x = (rect.x + rect.width / 2) - (glyphRect.width / 2);
                    var y = (rect.y + rect.height / 2) - (glyphRect.height / 2);

                    vertices[0] = new Vector3(x, y, 0);
                    vertices[1] = new Vector3(x, y + glyphRect.height, 0);
                    vertices[2] = new Vector3(x + glyphRect.width, y + glyphRect.height, 0);
                    vertices[3] = new Vector3(x + glyphRect.width, y, 0);
                    vertices[4] = new Vector3(x, y, 0);
                    vertices[5] = new Vector3(x, y + glyphRect.height, 0);
                    vertices[6] = new Vector3(x + glyphRect.width, y + glyphRect.height, 0);
                    vertices[7] = new Vector3(x + glyphRect.width, y, 0);

                    uvs[0] = new Vector2(0, 0);
                    uvs[1] = new Vector2(0, 1);
                    uvs[2] = new Vector2(1, 1);
                    uvs[3] = new Vector2(1, 0);
                    uvs[4] = new Vector2(uv0.x, uv0.y);
                    uvs[5] = new Vector2(uv0.x, uv1.y);
                    uvs[6] = new Vector2(uv1.x, uv1.y);
                    uvs[7] = new Vector2(uv1.x, uv0.y);
                    this.texture = texture;
                }
                else
                {
                    vertices[0] = new Vector3(rect.x + 100, rect.y, 0);
                    vertices[1] = new Vector3(rect.x + 100, rect.y + rect.height, 0);
                    vertices[2] = new Vector3(rect.x - 100 + rect.width, rect.y + rect.height, 0);
                    vertices[3] = new Vector3(rect.x - 100 + rect.width, rect.y, 0);
                    vertices[4] = new Vector3(rect.x + 200, rect.y, 0);
                    vertices[5] = new Vector3(rect.x + 200, rect.y + rect.height, 0);
                    vertices[6] = new Vector3(rect.x - 200 + rect.width, rect.y + rect.height, 0);
                    vertices[7] = new Vector3(rect.x - 200 + rect.width, rect.y, 0);

                    uvs[0] = new Vector2(0, 0);
                    uvs[1] = new Vector2(0, 1);
                    uvs[2] = new Vector2(1, 1);
                    uvs[3] = new Vector2(1, 0);
                    uvs[4] = new Vector2(0, 0);
                    uvs[5] = new Vector2(0, 1);
                    uvs[6] = new Vector2(1, 1);
                    uvs[7] = new Vector2(1, 0);
                    this.texture = this.fontAsset?.atlasTexture;
                }
                m_CachedMesh.Clear();
                m_CachedMesh.subMeshCount = 2;
                m_CachedMesh.vertices = vertices;
                m_CachedMesh.uv = uvs;
                m_CachedMesh.colors32 = colors;
                // m_CachedMesh.triangles = triangles;
                m_CachedMesh.SetTriangles(new int[] { 0, 1, 2, 2, 3, 0}, 0);
                m_CachedMesh.SetTriangles(new int[] { 4, 5, 6, 6, 7, 4}, 1);
                this.canvasRenderer.materialCount = 2;
                
                this.canvasRenderer.SetMaterial(this.material, 0);
                this.canvasRenderer.SetMaterial(this.fontAsset?.material, 1);
                this.canvasRenderer.SetMesh(m_CachedMesh);
                // if (this.fontAsset?.material.
                Debug.Log("1");
            }
        }
    }
}
