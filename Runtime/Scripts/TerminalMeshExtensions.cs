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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalMeshExtensions
    {
        public static void SetBackgroundVertices(this TerminalMesh terminalMesh, IEnumerable<ITerminalCell> cells, Rect rect)
        {
            var index = 0;
            terminalMesh.Count = cells.Count();
            foreach (var item in cells)
            {
                terminalMesh.SetVertex(index, item.BackgroundRect, rect);
                terminalMesh.SetUV(index, item.BackgroundUV);
                terminalMesh.SetColor(index, TerminalCell.GetBackgroundColor(item));
                index++;
            }
        }

        public static void SetForegroundVertices(this TerminalMesh terminalMesh, IEnumerable<ITerminalCell> cells, Rect rect)
        {
            var index = 0;
            terminalMesh.Count = cells.Count();
            foreach (var item in cells)
            {
                terminalMesh.SetVertex(index, item.ForegroundRect, rect);
                terminalMesh.SetUV(index, item.ForegroundUV);
                terminalMesh.SetColor(index, TerminalCell.GetForegroundColor(item));
                index++;
            }
        }
    }
}
