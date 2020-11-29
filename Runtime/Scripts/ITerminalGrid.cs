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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public interface ITerminalGrid
    {
        Vector2 WorldToGrid(Vector2 position, Camera camera);

        TerminalPoint Intersect(Vector2 position);

        ITerminalCell IntersectWithCell(Vector2 position);

        TerminalPoint IndexToPoint(int index);

        int PointToIndex(TerminalPoint point);

        void Focus();

        void ScrollToTop();

        void ScrollToBottom();

        void ScrollToCursor();

        void PageUp();

        void PageDown();

        void LineUp();

        void LineDown();

        void Scroll(int value);

        string Copy();

        void Paste(string text);

        void SelectAll();

        TerminalGridData Save();

        void Load(TerminalGridData data);

        ITerminal Terminal { get; }

        bool IsFocused { get; }

        string Text { get; }

        TerminalFont Font { get; set; }

        int MaxBufferHeight { get; set; }

        int BufferWidth { get; }

        int BufferHeight { get; }

        IReadOnlyList<ITerminalRow> Rows { get; }

        IReadOnlyList<TerminalCharacterInfo> CharacterInfos { get; }

        int VisibleIndex { get; set; }

        int MinimumVisibleIndex { get; }

        int MaximumVisibleIndex { get; }

        Color BackgroundColor { get; set; }

        Color ForegroundColor { get; set; }

        Color SelectionColor { get; set; }

        Color SelectionTextColor { get; set; }

        Color CursorColor { get; set; }

        Color CursorTextColor { get; set; }

        Texture2D FallbackTexture { get; set; }

        TerminalThickness Padding { get; set; }

        TerminalPoint CursorPoint { get; set; }

        IList<TerminalRange> Selections { get; }

        Rect Rectangle { get; }

        bool IsCursorVisible { get; set; }

        TerminalRange SelectingRange { get; set; }

        string CompositionString { get; set; }

        GameObject GameObject { get; }

        TerminalStyle Style { get; set; }

        TerminalCursorStyle CursorStyle { get; set; }

        int CursorThickness { get; set; }

        bool IsCursorBlinkable { get; set; }

        float CursorBlinkDelay { get; set; }

        bool IsScrollForwardEnabled { get; set; }

        bool IsScrolling { get; set; }

        IKeyBindingCollection KeyBindings { get; set; }

        IInputHandler InputHandler { get; set; }

        IList<TerminalBehaviourBase> BehaviourList { get; }

        TerminalDispatcher Dispatcher { get; }

        event EventHandler LayoutChanged;

        event NotifyCollectionChangedEventHandler SelectionChanged;

        event EventHandler GotFocus;

        event EventHandler LostFocus;

        event EventHandler Validated;

        event PropertyChangedEventHandler PropertyChanged;

        event EventHandler Enabled;

        event EventHandler Disabled;

        event EventHandler<TerminalKeyDownEventArgs> PreviewKeyDown;

        event EventHandler<TerminalKeyDownEventArgs> KeyDown;

        event EventHandler<TerminalKeyPressEventArgs> KeyPress;
    }
}
