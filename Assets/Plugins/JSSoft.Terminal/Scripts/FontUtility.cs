// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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

namespace JSSoft.Terminal
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
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (font.Contains(character) == false)
                throw new ArgumentException($"'{character}' does not exits.", nameof(character));
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

        public static Rect GetForegroundRect(TerminalFont font, char character)
        {
            return GetForegroundRect(font, character, 0, 0);
        }

        public static Rect GetForegroundRect(TerminalFont font, char character, int x, int y)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (font.Contains(character) == false)
                throw new ArgumentException($"'{character}' does not exits.", nameof(character));
            var charInfo = font[character];
            var fx = x + charInfo.XOffset;
            var fy = y + charInfo.YOffset;
            return new Rect(fx, fy, charInfo.Width, charInfo.Height);
        }
    }
}
