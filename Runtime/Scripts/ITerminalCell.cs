﻿// MIT License
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