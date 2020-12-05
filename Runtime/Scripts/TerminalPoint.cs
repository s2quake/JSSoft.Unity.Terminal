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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    [Serializable]
    public struct TerminalPoint : IEquatable<TerminalPoint>, IComparable
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

        public int DistanceOf(TerminalPoint point, int bufferWidth)
        {
            var (s1, s2, op) = this < point ? (this, point, 1) : (point, this, -1);
            var x = s1.X;
            var y = s1.Y;
            var c = 0;
            for (; y <= s2.Y; y++)
            {
                var count = y == s2.Y ? s2.X : bufferWidth;
                if (count >= bufferWidth)
                {
                    count = bufferWidth - 1;
                }
                for (; x <= count; x++)
                {
                    c++;
                }
                x = 0;
            }
            return c * op;
        }

        public IEnumerable<TerminalPoint> EnumerateTo(TerminalPoint point, int bufferWidth)
        {
            var (s1, s2) = this < point ? (this, point) : (point, this);
            var x = s1.X;
            for (var y = s1.Y; y <= s2.Y; y++)
            {
                var count = y == s2.Y ? s2.X : bufferWidth;
                for (; x < count; x++)
                {
                    yield return new TerminalPoint(x, y);
                }
                x = 0;
            }
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

        #region implementations

        bool IEquatable<TerminalPoint>.Equals(TerminalPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is TerminalPoint point)
            {
                if (this < point)
                    return -1;
                else if (this > point)
                    return 1;
                return 0;
            }
            throw new ArgumentException("invalid object", nameof(obj));
        }

        #endregion
    }
}
