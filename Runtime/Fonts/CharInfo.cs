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
using UnityEngine;

namespace JSSoft.Unity.Terminal.Fonts
{
    [Serializable]
    public struct CharInfo
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private int x;
        [SerializeField]
        private int y;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;
        [SerializeField]
        private int xOffset;
        [SerializeField]
        private int yOffset;
        [SerializeField]
        private int xAdvance;
        [SerializeField]
        private int yAdvance;
        [SerializeField]
        private int channel;
        [SerializeField]
        private Texture2D texture;

        public int ID { get => this.id; set => this.id = value; }

        public int X { get => this.x; set => this.x = value; }

        public int Y { get => this.y; set => this.y = value; }

        public int Width { get => this.width; set => this.width = value; }

        public int Height { get => this.height; set => this.height = value; }

        public int XOffset { get => this.xOffset; set => this.xOffset = value; }

        public int YOffset { get => this.yOffset; set => this.yOffset = value; }

        public int XAdvance { get => this.xAdvance; set => this.xAdvance = value; }

        public int YAdvance { get => this.yAdvance; set => this.yAdvance = value; }

        public int Channel { get => this.channel; set => this.channel = value; }

        public Texture2D Texture { get => this.texture; set => this.texture = value; }

        public static readonly CharInfo Empty = new CharInfo();
    }
}
