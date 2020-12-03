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
    public class TerminalColors
    {
        public static readonly Color Black = new Color32(0, 0, 0, 255);
        public static readonly Color Red = new Color32(128, 0, 0, 255);
        public static readonly Color Green = new Color32(0, 128, 0, 255);
        public static readonly Color Yellow = new Color32(128, 128, 0, 255);
        public static readonly Color Blue = new Color32(0, 0, 128, 255);
        public static readonly Color Magenta = new Color32(128, 0, 128, 255);
        public static readonly Color Cyan = new Color32(0, 128, 128, 255);
        public static readonly Color White = new Color32(128, 128, 128, 255);
        public static readonly Color BrightBlack = new Color32(128, 128, 128, 255);
        public static readonly Color BrightRed = new Color32(255, 0, 0, 255);
        public static readonly Color BrightGreen = new Color32(0, 255, 0, 255);
        public static readonly Color BrightYellow = new Color32(255, 255, 0, 255);
        public static readonly Color BrightBlue = new Color32(0, 0, 255, 255);
        public static readonly Color BrightMagenta = new Color32(255, 0, 255, 255);
        public static readonly Color BrightCyan = new Color32(0, 255, 255, 255);
        public static readonly Color BrightWhite = new Color32(255, 255, 255, 255);

        internal static readonly Color Transparent = new Color32(0, 0, 0, 0);

        public static Color GetColor(TerminalColor color)
        {
            switch (color)
            {
                case TerminalColor.Black:
                    return TerminalColors.Black;
                case TerminalColor.Blue:
                    return TerminalColors.Blue;
                case TerminalColor.Green:
                    return TerminalColors.Green;
                case TerminalColor.Cyan:
                    return TerminalColors.Cyan;
                case TerminalColor.Red:
                    return TerminalColors.Red;
                case TerminalColor.Magenta:
                    return TerminalColors.Magenta;
                case TerminalColor.Yellow:
                    return TerminalColors.Yellow;
                case TerminalColor.BrightBlack:
                    return TerminalColors.BrightBlack;
                case TerminalColor.White:
                    return TerminalColors.White;
                case TerminalColor.BrightBlue:
                    return TerminalColors.BrightBlue;
                case TerminalColor.BrightGreen:
                    return TerminalColors.BrightGreen;
                case TerminalColor.BrightCyan:
                    return TerminalColors.BrightCyan;
                case TerminalColor.BrightRed:
                    return TerminalColors.BrightRed;
                case TerminalColor.BrightMagenta:
                    return TerminalColors.BrightMagenta;
                case TerminalColor.BrightYellow:
                    return TerminalColors.BrightYellow;
                case TerminalColor.BrightWhite:
                    return TerminalColors.BrightWhite;
            }
            throw new NotImplementedException();
        }
    }
}
