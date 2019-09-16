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
        public static readonly Color32 Black = new Color32(0, 0, 0, 255);
        public static readonly Color32 DarkBlue = new Color32(0, 0, 128, 255);
        public static readonly Color32 DarkGreen = new Color32(0, 128, 0, 255);
        public static readonly Color32 DarkCyan = new Color32(0, 128, 128, 255);
        public static readonly Color32 DarkRed = new Color32(128, 0, 0, 255);
        public static readonly Color32 DarkMagenta = new Color32(128, 0, 128, 255);
        public static readonly Color32 DarkYellow = new Color32(128, 128, 0, 255);
        public static readonly Color32 Gray = new Color32(128, 128, 128, 255);
        public static readonly Color32 DarkGray = new Color32(128, 128, 128, 255);
        public static readonly Color32 Blue = new Color32(0, 0, 255, 255);
        public static readonly Color32 Green = new Color32(0, 255, 0, 255);
        public static readonly Color32 Cyan = new Color32(0, 255, 255, 255);
        public static readonly Color32 Red = new Color32(255, 0, 0, 255);
        public static readonly Color32 Magenta = new Color32(255, 0, 255, 255);
        public static readonly Color32 Yellow = new Color32(255, 255, 0, 255);
        public static readonly Color32 White = new Color32(255, 255, 255, 255);

        internal static readonly Color32 Transparent = new Color32(0, 0, 0, 0);
    }
}