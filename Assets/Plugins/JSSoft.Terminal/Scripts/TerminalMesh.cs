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
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore;
using System.Collections.Generic;
using System.Linq;

namespace JSSoft.Terminal
{
    public class TerminalMesh
    {
        private int count;
        private Vector3[] vertices = new Vector3[] { };
        private Vector2[] uvs = new Vector2[] { };
        private Color32[] colors = new Color32[] { };

        public TerminalMesh()
        {

        }

        public void Fill(VertexHelper vertexHelper)
        {
            var vertexCount = this.count * 4;
            vertexHelper.Clear();
            for (var i = 0; i < vertexCount; i++)
            {
                vertexHelper.AddVert(this.vertices[i], this.colors[i], this.uvs[i]);
            }
            for (var i = 0; i < this.count; i++)
            {
                vertexHelper.AddTriangle(i * 4 + 0, i * 4 + 1, i * 4 + 2);
                vertexHelper.AddTriangle(i * 4 + 2, i * 4 + 3, i * 4 + 0);
            }
        }

        public void SetVertex(int index, GlyphRect value, Rect transform)
        {
            if (index < 0 || index >= this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.vertices.SetVertex(index * 4, value);
            this.vertices.Transform(index * 4, transform);
        }

        public void SetVertex(int index, Rect value, Rect transform)
        {
            if (index < 0 || index >= this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.vertices.SetVertex(index * 4, value);
            this.vertices.Transform(index * 4, transform);
        }

        public void SetVertex(int index, Vector2 lt, Vector2 rt, Vector2 lb, Vector2 rb, Rect transform)
        {
            if (index < 0 || index >= this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.vertices[index * 4 + 0] = lt;
            this.vertices[index * 4 + 1] = lb;
            this.vertices[index * 4 + 2] = rb;
            this.vertices[index * 4 + 3] = rt;
            this.vertices.Transform(index * 4, transform);
        }

        public void SetUV(int index, (Vector2, Vector2) value)
        {
            if (index < 0 || index >= this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.uvs.SetUV(index * 4, value);
        }

        public void SetColor(int index, Color32 value)
        {
            if (index < 0 || index >= this.count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.colors.SetColor(index * 4, value);
        }

        public int Count
        {
            get => this.count;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                var length = value * 4;
                ArrayUtility.Resize(ref this.vertices, length);
                ArrayUtility.Resize(ref this.uvs, length);
                ArrayUtility.Resize(ref this.colors, length);
                this.count = value;
            }
        }
    }
}
