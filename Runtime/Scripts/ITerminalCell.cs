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

using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public interface ITerminalCell
    {
        bool Intersect(Vector2 position);

        int Index { get; }

        int TextIndex { get; }

        Texture2D Texture { get; }

        ITerminalRow Row { get; }

        ITerminalGrid Grid { get; }

        TerminalPoint Point { get; }

        char Character { get; }

        int Volume { get; }

        bool IsSelected { get; }

        bool IsCursor { get; }

        bool IsSelecting { get; }

        Rect BackgroundRect { get; }

        Rect ForegroundRect { get; }

        (Vector2, Vector2) BackgroundUV { get; }

        (Vector2, Vector2) ForegroundUV { get; }

        Color32? BackgroundColor { get; set; }

        Color32? ForegroundColor { get; set; }
    }
}
