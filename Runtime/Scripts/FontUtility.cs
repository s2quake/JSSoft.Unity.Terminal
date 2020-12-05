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
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public static class FontUtility
    {
        private static readonly int defaultItemWidth = 14;
        private static readonly int defaultItemHeight = 27;

        public static int DefaultItemWidth => defaultItemWidth;

        public static int DefaultItemHeight => defaultItemHeight;

        public static Fonts.CharInfo GetCharacter(TerminalFont font, char character)
        {
            if (font?.Contains(character) == true)
                return font[character];
            return new Fonts.CharInfo() { ID = character };
        }

        public static int GetCharacterVolume(TerminalFont font, char character)
        {
            if (font != null && GetCharacter(font, character) is Fonts.CharInfo characterInfo)
            {
                var defaultWidth = font.Width;
                var horizontalAdvance = characterInfo.XAdvance;
                var volume = (int)Math.Ceiling((float)horizontalAdvance / defaultWidth);
                return Math.Max(volume, 1);
            }
            return 1;
        }

        public static (Vector2, Vector2) GetUV(TerminalFont font, char character)
        {
            if (font?.Contains(character) == true)
            {
                var charInfo = font[character];
                var texture = charInfo.Texture;
                var w = (float)texture.width;
                var h = (float)texture.height;
                var l = (float)charInfo.X;
                var t = (float)charInfo.Y;
                var r = (float)charInfo.X + charInfo.Width;
                var b = (float)charInfo.Y + charInfo.Height;
                var uv0 = new Vector2(l / w, 1.0f - b / h);
                var uv1 = new Vector2(r / w, 1.0f - t / h);
                return (uv0, uv1);
            }
            return (Vector2.zero, Vector2.zero);
        }

        public static Rect GetForegroundRect(TerminalFont font, char character)
        {
            return GetForegroundRect(font, character, 0, 0);
        }

        public static Rect GetForegroundRect(TerminalFont font, char character, int x, int y)
        {
            if (font?.Contains(character) == true)
            {
                var charInfo = font[character];
                var fx = x + charInfo.XOffset;
                var fy = y + charInfo.YOffset;
                return new Rect(fx, fy, charInfo.Width, charInfo.Height);
            }
            else if (font != null)
            {
                return new Rect(x + 1, y + 1, font.Width - 2, font.Height - 2);
            }
            return new Rect(x + 1, y + 1, defaultItemWidth - 2, defaultItemHeight - 2);
        }
    }
}
