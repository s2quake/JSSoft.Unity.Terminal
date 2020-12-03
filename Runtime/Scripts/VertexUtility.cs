////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.Unity.Terminal
{
    public static class VertexUtility
    {
        public static void SetVertex(this Vector3[] vertices, int index, GlyphRect rect)
        {
            var left = rect.x;
            var top = rect.y + rect.height;
            var right = rect.x + rect.width;
            var bottom = rect.y;
            vertices[index + 0] = new Vector3(left, top, 0);
            vertices[index + 1] = new Vector3(left, bottom, 0);
            vertices[index + 2] = new Vector3(right, bottom, 0);
            vertices[index + 3] = new Vector3(right, top, 0);
        }

        public static void SetVertex(this Vector3[] vertices, int index, Rect rect)
        {
            var left = rect.x;
            var top = rect.y + rect.height;
            var right = rect.x + rect.width;
            var bottom = rect.y;
            vertices[index + 0] = new Vector3(left, top, 0);
            vertices[index + 1] = new Vector3(left, bottom, 0);
            vertices[index + 2] = new Vector3(right, bottom, 0);
            vertices[index + 3] = new Vector3(right, top, 0);
        }

        public static void SetVertex(this UIVertex[] vertices, int index, GlyphRect rect)
        {
            var left = rect.x;
            var top = rect.y + rect.height;
            var right = rect.x + rect.width;
            var bottom = rect.y;
            vertices[index + 0].position = new Vector3(left, top, 0);
            vertices[index + 1].position = new Vector3(left, bottom, 0);
            vertices[index + 2].position = new Vector3(right, bottom, 0);
            vertices[index + 3].position = new Vector3(right, top, 0);
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

        public static void SetUV(this Vector2[] uvs, int index)
        {
            SetUV(uvs, index, Vector2.zero, Vector2.one);
        }

        public static void SetUV(this Vector2[] uvs, int index, (Vector2, Vector2) uv)
        {
            SetUV(uvs, index, uv.Item1, uv.Item2);
        }

        public static void SetUV(this Vector2[] uvs, int index, Vector2 uv0, Vector2 uv1)
        {
            uvs[index + 0] = new Vector2(uv0.x, uv0.y);
            uvs[index + 1] = new Vector2(uv0.x, uv1.y);
            uvs[index + 2] = new Vector2(uv1.x, uv1.y);
            uvs[index + 3] = new Vector2(uv1.x, uv0.y);
        }

        public static void SetUV(this UIVertex[] vertices, int index)
        {
            SetUV(vertices, index, Vector2.zero, Vector2.one);
        }

        public static void SetUV(this UIVertex[] vertices, int index, (Vector2, Vector2) uv)
        {
            SetUV(vertices, index, uv.Item1, uv.Item2);
        }

        public static void SetUV(this UIVertex[] vertices, int index, Vector2 uv0, Vector2 uv1)
        {
            vertices[index + 0].uv0 = new Vector2(uv0.x, uv0.y);
            vertices[index + 1].uv0 = new Vector2(uv1.x, uv0.y);
            vertices[index + 2].uv0 = new Vector2(uv1.x, uv1.y);
            vertices[index + 3].uv0 = new Vector2(uv0.x, uv1.y);
        }

        public static void SetColor(this Color32[] colors, int index, Color32 color)
        {
            colors[index + 0] = color;
            colors[index + 1] = color;
            colors[index + 2] = color;
            colors[index + 3] = color;
        }

        public static void SetColor(this UIVertex[] vertices, int index, Color32 color)
        {
            vertices[index + 0].color = color;
            vertices[index + 1].color = color;
            vertices[index + 2].color = color;
            vertices[index + 3].color = color;
        }

        public static void SetTriangle(this int[] triangles, int index, int vertexIndex)
        {
            triangles[index + 0] = vertexIndex + 0;
            triangles[index + 1] = vertexIndex + 1;
            triangles[index + 2] = vertexIndex + 2;
            triangles[index + 3] = vertexIndex + 2;
            triangles[index + 4] = vertexIndex + 3;
            triangles[index + 5] = vertexIndex + 0;
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

        public static Rect Expand(this Rect rect, int size)
        {
            rect.x -= size;
            rect.y -= size;
            rect.width += (size * 2);
            rect.height += (size * 2);
            return rect;
        }

        public static bool Intersect(this GlyphRect rect, Vector2 position)
        {
            if (position.x < rect.x || position.y < rect.y)
                return false;
            if (position.x >= rect.x + rect.width || position.y >= rect.y + rect.height)
                return false;
            return true;
        }

        public static bool Intersect(this Rect rect, Vector2 position)
        {
            if (position.x < rect.x || position.y < rect.y)
                return false;
            if (position.x >= rect.x + rect.width || position.y >= rect.y + rect.height)
                return false;
            return true;
        }
    }
}
