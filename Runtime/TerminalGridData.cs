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
