// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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

namespace JSSoft.UI
{
    public class TerminalColors
    {
        public static readonly Color Black = new Color32(0, 0, 0, 255);
        public static readonly Color DarkBlue = new Color32(0, 0, 128, 255);
        public static readonly Color DarkGreen = new Color32(0, 128, 0, 255);
        public static readonly Color DarkCyan = new Color32(0, 128, 128, 255);
        public static readonly Color DarkRed = new Color32(128, 0, 0, 255);
        public static readonly Color DarkMagenta = new Color32(128, 0, 128, 255);
        public static readonly Color DarkYellow = new Color32(128, 128, 0, 255);
        public static readonly Color Gray = new Color32(128, 128, 128, 255);
        public static readonly Color DarkGray = new Color32(128, 128, 128, 255);
        public static readonly Color Blue = new Color32(0, 0, 255, 255);
        public static readonly Color Green = new Color32(0, 255, 0, 255);
        public static readonly Color Cyan = new Color32(0, 255, 255, 255);
        public static readonly Color Red = new Color32(255, 0, 0, 255);
        public static readonly Color Magenta = new Color32(255, 0, 255, 255);
        public static readonly Color Yellow = new Color32(255, 255, 0, 255);
        public static readonly Color White = new Color32(255, 255, 255, 255);

        internal static readonly Color Transparent = new Color32(0, 0, 0, 0);

        public static Color GetColor(TerminalColor color)
        {
            switch (color)
            {
                case TerminalColor.Black:
                    return TerminalColors.Black;
                case TerminalColor.DarkBlue:
                    return TerminalColors.DarkBlue;
                case TerminalColor.DarkGreen:
                    return TerminalColors.DarkGreen;
                case TerminalColor.DarkCyan:
                    return TerminalColors.DarkCyan;
                case TerminalColor.DarkRed:
                    return TerminalColors.DarkRed;
                case TerminalColor.DarkMagenta:
                    return TerminalColors.DarkMagenta;
                case TerminalColor.DarkYellow:
                    return TerminalColors.DarkYellow;
                case TerminalColor.Gray:
                    return TerminalColors.Gray;
                case TerminalColor.DarkGray:
                    return TerminalColors.DarkGray;
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
                case TerminalColor.White:
                    return TerminalColors.White;
            }
            throw new NotImplementedException();
        }
    }
}