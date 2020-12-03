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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public struct TerminalGridData
    {
        public TerminalData TerminalData { get; set; }

        public TerminalRange[] Selections { get; set; }

        public int BufferWidth { get; set; }

        public int BufferHeight { get; set; }

        public TerminalFont Font { get; set; }

        public Color BackgroundColor { get; set; }

        public Color ForegroundColor { get; set; }

        public Color SelectionColor { get; set; }

        public Color SelectionTextColor { get; set; }

        public Color CursorColor { get; set; }

        public Color CursorTextColor { get; set; }

        public Texture2D FallbackTexture { get; set; }

        public TerminalColorPalette ColorPalette { get; set; }

        public int VisibleIndex { get; set; }

        public int MaxBufferHeight { get; set; }

        public TerminalCursorStyle CursorStyle { get; set; }

        public int CursorThickness { get; set; }

        public bool IsCursorBlinkable { get; set; }

        public float CursorBlinkDelay { get; set; }

        public bool IsScrollForwardEnabled { get; set; }

        public TerminalBehaviourBase[] Behaviours { get; set; }

        public TerminalPoint CursorPoint { get; set; }

        public bool IsCursorVisible { get; set; }

        public string CompositionString { get; set; }

        public TerminalThickness Padding { get; set; }

        public TerminalStyle Style { get; set; }
    }
}
