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
    public static class VertexUtility
    {
        public static int SetVertex(this Vector3[] vertices, int index, GlyphRect rect)
        {
            var left = rect.x;
            var top = rect.y + rect.height;
            var right = rect.x + rect.width;
            var bottom = rect.y;
            vertices[index + 0] = new Vector3(left, top, 0);
            vertices[index + 1] = new Vector3(left, bottom, 0);
            vertices[index + 2] = new Vector3(right, bottom, 0);
            vertices[index + 3] = new Vector3(right, top, 0);
            return 4;
        }

        public static int SetVertex(this UIVertex[] vertices, int index, GlyphRect rect)
        {
            var left = rect.x;
            var top = rect.y + rect.height;
            var right = rect.x + rect.width;
            var bottom = rect.y;
            vertices[index + 0].position = new Vector3(left, top, 0);
            vertices[index + 1].position = new Vector3(left, bottom, 0);
            vertices[index + 2].position = new Vector3(right, bottom, 0);
            vertices[index + 3].position = new Vector3(right, top, 0);
            return 4;
        }

        public static void Transform(this Vector3[] vertices, int index, Rect rect)
        {
            for (var i = 0; i < 4; i++)
            {
                vertices[index + i].x += rect.x;
                vertices[index + i].y = rect.y + rect.height - vertices[index + i].y;
            }
        }

        public static void Transform(this UIVertex[] vertices, int index, Rect rect)
        {
            for (var i = 0; i < 4; i++)
            {
                var position = vertices[index + i].position;
                position.x += rect.x;
                position.y = rect.y + rect.height - position.y;
                vertices[index + i].position = position;
            }
        }

        public static int SetUV(this Vector2[] uvs, int index)
        {
            return SetUV(uvs, index, Vector2.zero, Vector2.one);
        }

        public static int SetUV(this Vector2[] uvs, int index, (Vector2, Vector2) uv)
        {
            return SetUV(uvs, index, uv.Item1, uv.Item2);
        }

        public static int SetUV(this Vector2[] uvs, int index, Vector2 uv0, Vector2 uv1)
        {
            uvs[index + 0] = new Vector2(uv0.x, uv0.y);
            uvs[index + 1] = new Vector2(uv0.x, uv1.y);
            uvs[index + 2] = new Vector2(uv1.x, uv1.y);
            uvs[index + 3] = new Vector2(uv1.x, uv0.y);
            return 4;
        }

        public static int SetUV(this UIVertex[] vertices, int index)
        {
            return SetUV(vertices, index, Vector2.zero, Vector2.one);
        }

        public static int SetUV(this UIVertex[] vertices, int index, (Vector2, Vector2) uv)
        {
            return SetUV(vertices, index, uv.Item1, uv.Item2);
        }

        public static int SetUV(this UIVertex[] vertices, int index, Vector2 uv0, Vector2 uv1)
        {
            vertices[index + 0].uv0 = new Vector2(uv0.x, uv0.y);
            vertices[index + 1].uv0 = new Vector2(uv0.x, uv1.y);
            vertices[index + 2].uv0 = new Vector2(uv1.x, uv1.y);
            vertices[index + 3].uv0 = new Vector2(uv1.x, uv0.y);
            return 4;
        }

        public static int SetColor(this Color32[] colors, int index, Color32 color)
        {
            colors[index + 0] = color;
            colors[index + 1] = color;
            colors[index + 2] = color;
            colors[index + 3] = color;
            return 4;
        }

        public static int SetColor(this UIVertex[] vertices, int index, Color32 color)
        {
            vertices[index + 0].color = color;
            vertices[index + 1].color = color;
            vertices[index + 2].color = color;
            vertices[index + 3].color = color;
            return 4;
        }

        public static int SetTriangle(this int[] triangles, int index, int vertexIndex)
        {
            triangles[index + 0] = vertexIndex + 0;
            triangles[index + 1] = vertexIndex + 1;
            triangles[index + 2] = vertexIndex + 2;
            triangles[index + 3] = vertexIndex + 2;
            triangles[index + 4] = vertexIndex + 3;
            triangles[index + 5] = vertexIndex + 0;
            return 6;
        }

        public static void SetTriangles(this int[] triangles, int index, int vertexIndex, int count)
        {
            for (var i = 0; i < count; i++)
            {
                SetTriangle(triangles, index, vertexIndex);
                index += 6;
                vertexIndex += 4;
            }
        }

        public static GlyphRect Expand(this GlyphRect rect, int size)
        {
            rect.x -= size;
            rect.y -= size;
            rect.width += (size * 2);
            rect.height += (size * 2);
            return rect;
        }
    }
}
