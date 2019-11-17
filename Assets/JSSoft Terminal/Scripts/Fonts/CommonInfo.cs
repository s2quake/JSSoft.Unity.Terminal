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
using UnityEngine.TextCore;

namespace JSSoft.UI.Fonts
{
    [Serializable]
    public struct CommonInfo
    {
        [SerializeField]
        private int lineHeight;
        [SerializeField]
        private int baseLine;
        [SerializeField]
        private int scaleW;
        [SerializeField]
        private int scaleH;
        [SerializeField]
        private int pages;
        [SerializeField]
        private bool packed;
        [SerializeField]
        private bool alphaChannel;
        [SerializeField]
        private bool redChannel;
        [SerializeField]
        private bool greenChannel;
        [SerializeField]
        private bool blueChannel;

        public int LineHeight { get => this.lineHeight; set => this.lineHeight = value; }

        public int BaseLine { get => this.baseLine; set => this.baseLine = value; }

        public int ScaleW { get => this.scaleW; set => this.scaleW = value; }

        public int ScaleH { get => this.scaleH; set => this.scaleH = value; }

        public int Pages { get => this.pages; set => this.pages = value; }

        public bool Packed { get => this.packed; set => this.packed = value; }

        public bool AlphaChannel { get => this.alphaChannel; set => this.alphaChannel = value; }

        public bool RedChannel { get => this.redChannel; set => this.redChannel = value; }

        public bool GreenChannel { get => this.greenChannel; set => this.greenChannel = value; }

        public bool BlueChannel { get => this.blueChannel; set => this.blueChannel = value; }
    }
}
