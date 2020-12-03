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

namespace JSSoft.Unity.Terminal
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
