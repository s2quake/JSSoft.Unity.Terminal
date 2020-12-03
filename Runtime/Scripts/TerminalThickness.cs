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

namespace JSSoft.Unity.Terminal
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
