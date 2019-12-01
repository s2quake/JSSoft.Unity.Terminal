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
    public struct TerminalRange
    {
        private TerminalPoint beginPoint;
        private TerminalPoint endPoint;

        public TerminalRange(TerminalPoint p1, TerminalPoint p2)
        {
            this.beginPoint = p1 < p2 ? p1 : p2;
            this.endPoint = p1 > p2 ? p1 : p2;
        }

        public override bool Equals(object obj)
        {
            if (obj is TerminalRange r)
            {
                return this.BeginPoint == r.BeginPoint && this.EndPoint == r.EndPoint;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.BeginPoint.GetHashCode() ^ this.EndPoint.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{this.BeginPoint}}} - {{{this.EndPoint}}}";
        }

        public bool Intersect(TerminalPoint point)
        {
            return point >= this.beginPoint && point < this.endPoint;
        }

        public TerminalPoint BeginPoint
        {
            get => this.beginPoint;
            set => this.beginPoint = value;
        }

        public TerminalPoint EndPoint
        {
            get => this.endPoint;
            set => this.endPoint = value;
        }

        public static bool operator ==(TerminalRange r1, TerminalRange r2)
        {
            return r1.BeginPoint == r2.BeginPoint && r1.EndPoint == r2.EndPoint;
        }

        public static bool operator !=(TerminalRange r1, TerminalRange r2)
        {
            return r1.BeginPoint != r2.BeginPoint || r1.EndPoint != r2.EndPoint;
        }

        public static readonly TerminalRange Empty = new TerminalRange(TerminalPoint.Invalid, TerminalPoint.Invalid);
    }
}
