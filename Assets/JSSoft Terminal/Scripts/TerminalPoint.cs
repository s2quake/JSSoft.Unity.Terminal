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

namespace JSSoft.UI
{
    [Serializable]
    public struct TerminalPoint : IEquatable<TerminalPoint>
    {
        [SerializeField]
        private int x;
        [SerializeField]
        private int y;

        public TerminalPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is TerminalPoint point)
            {
                return this.X == point.X && this.Y == point.Y;
            }
            return base.Equals(obj);
        }

        public void Shift(int x, int y, int columnCount)
        {
            
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        public override string ToString()
        {
            return $"{this.X}, {this.Y}";
        }

        public int X
        {
            get => this.x;
            set => this.x = value;
        }

        public int Y
        {
            get => this.y;
            set => this.y = value;
        }

        public static bool operator >(TerminalPoint pt1, TerminalPoint pt2)
        {
            return pt1.Y > pt2.Y || (pt1.Y == pt2.Y && pt1.X > pt2.X);
        }

        public static bool operator >=(TerminalPoint pt1, TerminalPoint pt2)
        {
            return pt1 > pt2 ? true : pt1 == pt2;
        }

        public static bool operator <(TerminalPoint pt1, TerminalPoint pt2)
        {
            return pt1.Y < pt2.Y || (pt1.Y == pt2.Y && pt1.X < pt2.X);
        }

        public static bool operator <=(TerminalPoint pt1, TerminalPoint pt2)
        {
            return pt1 < pt2 ? true : pt1 == pt2;
        }

        public static bool operator ==(TerminalPoint pt1, TerminalPoint pt2)
        {
            return pt1.Y == pt2.Y && pt1.X == pt2.X;
        }

        public static bool operator !=(TerminalPoint pt1, TerminalPoint pt2)
        {
            return pt1.Y != pt2.Y || pt1.X != pt2.X;
        }

        public static readonly TerminalPoint Zero = new TerminalPoint(0, 0);

        public static readonly TerminalPoint Invalid = new TerminalPoint(-1, -1);

        #region IEquatable

        bool IEquatable<TerminalPoint>.Equals(TerminalPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        #endregion
    }
}