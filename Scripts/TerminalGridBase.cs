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
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    public abstract class TerminalGridBase : MaskableGraphic, ITerminalGrid, INotifyPropertyChanged
    {
        public static readonly Color DefaultBackgroundColor = new Color32(23, 23, 23, 255);
        public static readonly Color DefaultForegroundColor = new Color32(255, 255, 255, 255);
        public static readonly Color DefaultSelectionColor = new Color32(49, 79, 129, 255);
        public static readonly Color DefaultCursorColor = new Color32(139, 139, 139, 255);
        public static readonly Color DefaultCompositionColor = new Color32(255, 255, 255, 255);
        public static readonly Color DefaultScrollbarColor = new Color32(139, 139, 139, 0);

        public abstract TerminalBase Terminal { get; }

        public abstract bool IsFocused { get; }

        public abstract string Text { get; }

        public abstract TerminalFont Font { get; set; }

        public abstract int MaxBufferHeight { get; set; }

        public abstract int BufferWidth { get; }

        public abstract int BufferHeight { get; }

        public abstract IReadOnlyList<ITerminalRow> Rows { get; }

        public abstract IReadOnlyList<TerminalCharacterInfo> CharacterInfos { get; }

        public abstract int VisibleIndex { get; set; }

        public abstract int MinimumVisibleIndex { get; }

        public abstract int MaximumVisibleIndex { get; }

        public abstract Color BackgroundColor { get; set; }

        public abstract Color ForegroundColor { get; set; }

        public abstract Color SelectionColor { get; set; }

        public abstract Color CursorColor { get; set; }

        public abstract TerminalThickness Padding { get; set; }

        public abstract TerminalPoint CursorPoint { get; set; }

        public abstract IList<TerminalRange> Selections { get; }

        public abstract Rect Rectangle { get; }

        public abstract bool IsCursorVisible { get; set; }

        public abstract TerminalRange SelectingRange { get; set; }

        public abstract string CompositionString { get; set; }

        public abstract TerminalStyle Style { get; set; }

        public abstract TerminalCursorStyle CursorStyle { get; set; }

        public abstract int CursorThickness { get; set; }

        public abstract bool IsCursorBlinkable { get; set; }

        public abstract float CursorBlinkDelay { get; set; }

        public abstract bool IsScrollForwardEnabled { get; set; }

        public abstract bool IsScrolling { get; set; }

        public abstract IKeyBindingCollection KeyBindings { get; set; }

        public abstract IInputHandler InputHandler { get; set; }

        public abstract IList<TerminalBehaviourBase> BehaviourList { get; }

        public abstract TerminalDispatcher Dispatcher { get; }

        public abstract event EventHandler LayoutChanged;

        public abstract event NotifyCollectionChangedEventHandler SelectionChanged;

        public abstract event EventHandler GotFocus;

        public abstract event EventHandler LostFocus;

        public abstract event EventHandler Validated;

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract event EventHandler Enabled;

        public abstract event EventHandler Disabled;

        public abstract event EventHandler<TerminalKeyPreviewEventArgs> KeyPreview;

        public abstract event EventHandler<TerminalKeyPressEventArgs> KeyPressed;

        public abstract string Copy();

        public abstract void Paste(string text);

        public abstract void Focus();

        public abstract TerminalPoint IndexToPoint(int index);

        public abstract TerminalPoint Intersect(Vector2 position);

        public abstract ITerminalCell IntersectWithCell(Vector2 position);

        public abstract void LineDown();

        public abstract void LineUp();

        public abstract void PageDown();

        public abstract void PageUp();

        public abstract int PointToIndex(TerminalPoint point);

        public abstract void Scroll(int value);

        public abstract void ScrollToBottom();

        public abstract void ScrollToCursor();

        public abstract void ScrollToTop();

        public abstract void SelectAll();

        public abstract Vector2 WorldToGrid(Vector2 position, Camera camera);

        #region ITerminalGrid

        ITerminal ITerminalGrid.Terminal => this.Terminal;

        GameObject ITerminalGrid.GameObject => this.gameObject;

        #endregion
    }
}
