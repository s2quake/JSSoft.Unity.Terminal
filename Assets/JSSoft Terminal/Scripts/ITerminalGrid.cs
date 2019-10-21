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
using TMPro;
using UnityEngine;

namespace JSSoft.UI
{
    public interface ITerminalGrid
    {
        TerminalPoint Intersect(Vector2 position);

        ITerminalCell IntersectWithCell(Vector2 position);

        void ClearSelection();

        // TerminalPoint IndexToPoint(int index) => this.characterInfos[index].Point;

        // int PointToIndex(TerminalPoint point) => this.characterInfos.PointToIndex(point);

        // Color32? IndexToBackgroundColor(int index) => this.Terminal.GetBackgroundColor(index);

        // Color32? IndexToForegroundColor(int index) => this.Terminal.GetForegroundColor(index);

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

        TMP_FontAsset FontAsset { get; }

        int ColumnCount { get; }

        int RowCount { get; }

        IReadOnlyList<ITerminalRow> Rows { get; }

        int VisibleIndex { get; set; }

        Color32? BackgroundColor { get; set; }

        Color32? ForegroundColor { get; set; }

        Color32 SelectionColor { get; set; }

        Color32 CursorColor { get; set; }

        TerminalPoint CursorPosition { get; set; }

        Rect Rectangle { get; }

        bool IsCursorVisible { get; set; }

        bool IsScrolling { get; }

        string CompositionString { get; set; }

        event EventHandler TextChanged;

        event EventHandler VisibleIndexChanged;

        event EventHandler LayoutChanged;

        event EventHandler SelectionChanged;

        event EventHandler CursorPositionChanged;

        event EventHandler CompositionStringChanged;

        event EventHandler GotFocus;

        event EventHandler LostFocus;
    }
}