﻿////////////////////////////////////////////////////////////////////////////////
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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

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
