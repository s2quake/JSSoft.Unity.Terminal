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

namespace JSSoft.Unity.Terminal
{
    static class TerminalGridValidator
    {
        public static TerminalPoint ValidateCursorPoint(TerminalGrid grid, TerminalPoint cursorPoint)
        {
            var x = grid.CursorPoint.X;
            var y = grid.CursorPoint.Y;
            var bufferWidth = grid.BufferWidth;
            var bufferHeight = grid.BufferHeight;
            var rowCount = grid.Rows.Count;
            var maxBufferHeight = Math.Max(bufferHeight, rowCount);
            x = Math.Min(x, bufferWidth - 1);
            x = Math.Max(x, 0);
            y = Math.Min(y, maxBufferHeight - 1);
            y = Math.Max(y, 0);
            return new TerminalPoint(x, y);
        }

        public static int GetVisibleIndex(TerminalGrid grid)
        {
            return GetVisibleIndex(grid, grid.VisibleIndex);
        }

        public static int GetVisibleIndex(TerminalGrid grid, int visibleIndex)
        {
            var minimumVisibleIndex = grid.MinimumVisibleIndex;
            var maximumVisibleIndex = grid.MaximumVisibleIndex;
            visibleIndex = Math.Max(visibleIndex, minimumVisibleIndex);
            visibleIndex = Math.Min(visibleIndex, maximumVisibleIndex);
            return visibleIndex;
        }
    }
}
