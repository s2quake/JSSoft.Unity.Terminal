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

namespace JSSoft.Terminal
{
    [Serializable]
    public struct TerminalThickness : IEquatable<TerminalThickness>
    {
        [SerializeField]
        private int left;
        [SerializeField]
        private int top;
        [SerializeField]
        private int right;
        [SerializeField]
        private int bottom;

        public TerminalThickness(int length)
        {
            this.left = length;
            this.top = length;
            this.right = length;
            this.bottom = length;
        }

        public TerminalThickness(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj is TerminalThickness thickness)
            {
                return this.Left == thickness.Left && this.Top == thickness.Top && this.Right == thickness.Right && this.Bottom == thickness.Bottom;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Left ^ this.Top ^ this.Right ^ this.Bottom;
        }

        public override string ToString()
        {
            return $"{this.Left}, {this.Top}, {this.Right}, {this.Bottom}";
        }

        public int Left
        {
            get => this.left;
            set => this.left = value;
        }

        public int Top
        {
            get => this.top;
            set => this.top = value;
        }

        public int Right
        {
            get => this.right;
            set => this.right = value;
        }

        public int Bottom
        {
            get => this.bottom;
            set => this.bottom = value;
        }

        public static bool operator ==(TerminalThickness pt1, TerminalThickness pt2)
        {
            return pt1.Left == pt2.Left && pt1.Top == pt2.Top && pt1.Right == pt2.Right && pt1.Bottom == pt2.Bottom;
        }

        public static bool operator !=(TerminalThickness pt1, TerminalThickness pt2)
        {
            return pt1.Left != pt2.Left || pt1.Top != pt2.Top || pt1.Right != pt2.Right || pt1.Bottom != pt2.Bottom;
        }

        public static Rect operator +(Rect rect, TerminalThickness value)
        {
            var x1 = rect.xMin + value.left;
            var y1 = rect.yMin + value.top;
            var x2 = rect.xMax - value.right;
            var y2 = rect.yMax - value.bottom;
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        public static readonly TerminalThickness Empty = new TerminalThickness(0);

        #region implementations

        bool IEquatable<TerminalThickness>.Equals(TerminalThickness other)
        {
            return this.Left == other.Left && this.Top == other.Top && this.Right == other.Right && this.Bottom == other.Bottom;
        }

        #endregion
    }
}
