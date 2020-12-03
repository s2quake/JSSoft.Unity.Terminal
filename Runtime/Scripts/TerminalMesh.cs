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

using System;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
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
