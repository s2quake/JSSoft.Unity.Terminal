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

namespace JSSoft.UI.Fonts
{
    [Serializable]
    public struct BaseInfo
    {
        [SerializeField]
        private string face;
        [SerializeField]
        private int size;
        [SerializeField]
        private bool bold;
        [SerializeField]
        private bool italic;
        [SerializeField]
        private string charset;
        [SerializeField]
        private bool unicode;
        [SerializeField]
        private int stretchH;
        [SerializeField]
        private bool smooth;
        [SerializeField]
        private bool aa;
        [SerializeField]
        private int paddingLeft;
        [SerializeField]
        private int paddingTop;
        [SerializeField]
        private int paddingRight;
        [SerializeField]
        private int paddingBottom;
        [SerializeField]
        private int verticalSpacing;
        [SerializeField]
        private int horizontalSpacing;
        [SerializeField]
        private bool outline;

        public string Face { get => this.face; set => this.face = value; }

        public int Size { get => this.size; set => this.size = value; }

        public bool Bold { get => this.bold; set => this.bold = value; }

        public bool Italic { get => this.italic; set => this.italic = value; }

        public string Charset { get => this.charset; set => this.charset = value; }

        public bool Unicode { get => this.unicode; set => this.unicode = value; }

        public int StretchH { get => this.stretchH; set => this.stretchH = value; }

        public bool Smooth { get => this.smooth; set => this.smooth = value; }

        public bool Aa { get => this.aa; set => this.aa = value; }

        public (int Top, int Right, int Bottom, int Left) Padding
        {
            get => (this.paddingLeft, this.paddingTop, this.paddingRight, this.paddingBottom);
            set
            {
                this.paddingLeft = value.Left;
                this.paddingTop = value.Top;
                this.paddingRight = value.Right;
                this.paddingBottom = value.Bottom;
            }
        }

        public (int Vertical, int Horizontal) Spacing
        {
            get => (this.verticalSpacing, this.horizontalSpacing);
            set
            {
                this.verticalSpacing = value.Vertical;
                this.horizontalSpacing = value.Horizontal;
            }
        }

        public bool Outline { get => this.outline; set => this.outline = value; }

        public static readonly BaseInfo Empty = new BaseInfo();
    }
}
