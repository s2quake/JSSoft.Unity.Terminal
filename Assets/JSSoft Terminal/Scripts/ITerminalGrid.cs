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
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.UI
{
    public interface ITerminalGrid
    {
        Vector2 WorldToGrid(Vector2 position);

        TerminalPoint Intersect(Vector2 position);

        ITerminalCell IntersectWithCell(Vector2 position);

        void Focus();

        void ScrollToTop();

        void ScrollToBottom();

        void PageUp();

        void PageDown();

        void LineUp();

        void LineDown();

        void Scroll(int value);

        void Copy();

        void SelectAll();

        ITerminal Terminal { get; }

        bool IsFocused { get; }

        string Text { get; set; }

        TerminalFont Font { get; set; }

        int BufferWidth { get; set; }

        int BufferHeight { get; set; }

        IReadOnlyList<ITerminalRow> Rows { get; }

        IReadOnlyList<TerminalCharacterInfo> CharacterInfos { get; }

        int VisibleIndex { get; set; }

        Color BackgroundColor { get; set; }

        Color ForegroundColor { get; set; }

        Color SelectionColor { get; set; }

        Color CursorColor { get; set; }

        Color CompositionColor { get; set; }

        TerminalThickness Padding { get; set; }

        TerminalPoint CursorPoint { get; set; }

        IList<TerminalRange> Selections { get; }

        Rect Rectangle { get; }

        bool IsCursorVisible { get; set; }

        bool IsScrolling { get; set; }

        TerminalRange SelectingRange { get; set; }

        string CompositionString { get; set; }

        GameObject GameObject { get; }

        TerminalStyle Style { get; set; }

        TerminalCursorStyle CursorStyle { get; set; }

        int CursorThickness { get; set; }

        bool IsCursorBlinkable { get; set; }

        float CursorBlinkDelay { get; set; }

        IKeyBindingCollection KeyBindings { get; set; }

        IInputHandler InputHandler { get; set; }

        event EventHandler LayoutChanged;

        event EventHandler SelectionChanged;

        event EventHandler GotFocus;

        event EventHandler LostFocus;

        event EventHandler Validated;

        event PropertyChangedEventHandler PropertyChanged;

        event EventHandler Enabled;

        event EventHandler Disabled;
    }
}
