// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(CanvasRenderer))]
    [DefaultExecutionOrder(-196)]
    class TerminalForegroundItem : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private TerminalForeground foreground = null;
        [SerializeField]
        private Texture2D texture;

        private readonly TerminalMesh terminalMesh = new TerminalMesh();

        public TerminalForegroundItem()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override Texture mainTexture => this.texture ?? Texture2D.whiteTexture;

        public TerminalFont Font => this.grid?.Font;

        public TerminalGrid Grid
        {
            get => this.grid;
            internal set => this.grid = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Texture2D Texture
        {
            get => this.texture;
            internal set => this.texture = value;
        }

        public TerminalForeground Foreground
        {
            get => this.foreground;
            internal set => this.foreground = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            if (this.foreground != null)
            {
                var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, true);
                var visibleCells = this.foreground.GetCells(this.texture);
                this.terminalMesh.SetForegroundVertices(visibleCells, rect);
                this.terminalMesh.Fill(vh);
            }
        }
    }
}
