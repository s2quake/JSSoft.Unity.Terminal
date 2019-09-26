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
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class BufferedText : MaskableGraphic
    {
        public BufferedText()
        {


        }

        // public override Texture mainTexture => s_WhiteTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
            var vertices = new UIVertex[4];
            vertices[0].position = new Vector3(0, 0, 0);
            vertices[1].position = new Vector3(0, 1000, 0);
            vertices[2].position = new Vector3(1000, 0, 0);
            vertices[3].position = new Vector3(1000, 1000, 0);

            vertices[0].color = new Color(1, 0, 0);
            vertices[1].color = new Color(1, 0, 0);
            vertices[2].color = new Color(1, 0, 0);
            vertices[3].color = new Color(1, 0, 0);

            vertices[0].uv0 = new Vector2(0, 0);
            vertices[1].uv0 = new Vector2(0, 1);
            vertices[2].uv0 = new Vector2(1, 0);
            vertices[3].uv0 = new Vector2(1, 1);

            vh.AddVert(vertices[0]);
            vh.AddVert(vertices[1]);
            vh.AddVert(vertices[2]);
            vh.AddVert(vertices[3]);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 1, 3);
            // vas, this);
            // CanvasUpdateRegistry.register

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
            // return;
            if (executing == CanvasUpdate.LatePreRender)
            {
                var itemWidth = 14;
                var itemHeight = 21;
                var columnCount = (int)(this.rectTransform.rect.width / itemWidth);
                var rowCount = (int)(this.rectTransform.rect.height / itemHeight);
                var mesh = new Mesh();
                var vertices = new Vector3[columnCount * rowCount * 4];
                var colors = new Color[columnCount * rowCount * 4];
                var uv = new Vector2[columnCount * rowCount * 4];
                var triangles = new int[columnCount * rowCount * 6];

                var bottom = this.rectTransform.rect.y + this.rectTransform.rect.height;
                var i = 0;
                var t = 0;
                for (var y = 0; y < rowCount; y++)
                {
                    var left = this.rectTransform.rect.x;
                    for (var x = 0; x < columnCount; x++)
                    {
                        var right = left + itemWidth;
                        var top = bottom - itemHeight;
                        vertices[i + 0] = new Vector3(left, top, 0);
                        vertices[i + 1] = new Vector3(left, bottom, 0);
                        vertices[i + 2] = new Vector3(right, bottom, 0);
                        vertices[i + 3] = new Vector3(right, top, 0);

                        colors[i + 0] = new Color(0, 1, 1, 1);
                        colors[i + 1] = new Color(0, 1, 1, 1);
                        colors[i + 2] = new Color(0, 1, 1, 1);
                        colors[i + 3] = new Color(0, 1, 1, 1);

                        uv[i + 0] = new Vector2(0, 0);
                        uv[i + 1] = new Vector2(0, 1);
                        uv[i + 2] = new Vector2(1, 0);
                        uv[i + 3] = new Vector2(1, 1);

                        triangles[t + 0] = i + 0;
                        triangles[t + 1] = i + 1;
                        triangles[t + 2] = i + 2;
                        triangles[t + 3] = i + 2;
                        triangles[t + 4] = i + 3;
                        triangles[t + 5] = i + 0;

                        i += 4;
                        t += 6;
                        left += itemWidth;
                    }
                    bottom -= itemHeight;
                }
                mesh.vertices = vertices;
                mesh.colors = colors;
                mesh.uv = uv;
                mesh.triangles = triangles;

                this.canvasRenderer.Clear();
                this.canvasRenderer.SetMaterial(Graphic.defaultGraphicMaterial, null);
                this.canvasRenderer.SetMesh(mesh);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            SetMaterialDirty();
            SetLayoutDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // GraphicRegistry.RegisterGraphicForCanvas(this.can
        }
    }
}
