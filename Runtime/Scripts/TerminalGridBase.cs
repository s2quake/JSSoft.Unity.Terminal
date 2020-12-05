////////////////////////////////////////////////////////////////////////////////
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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

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
        public static readonly Color DefaultBackgroundColor = new Color32(23, 23, 23, 240);
        public static readonly Color DefaultForegroundColor = new Color32(255, 255, 255, 255);
        public static readonly Color DefaultSelectionColor = new Color32(49, 79, 129, 255);
        public static readonly Color DefaultSelectionTextColor = new Color32(255, 255, 255, 255);
        public static readonly Color DefaultCursorColor = new Color32(139, 139, 139, 255);
        public static readonly Color DefaultCursorTextColor = new Color32(255, 255, 255, 255);
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

        public abstract Color SelectionTextColor { get; set; }

        public abstract Color CursorColor { get; set; }

        public abstract Color CursorTextColor { get; set; }

        public abstract Texture2D FallbackTexture { get; set; }

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

        public abstract event EventHandler<TerminalKeyDownEventArgs> PreviewKeyDown;

        public abstract event EventHandler<TerminalKeyDownEventArgs> KeyDown;

        public abstract event EventHandler<TerminalKeyPressEventArgs> KeyPress;

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

        public abstract TerminalGridData Save();

        public abstract void Load(TerminalGridData data);

        public abstract Vector2 WorldToGrid(Vector2 position, Camera camera);

        public static TerminalGridBase Current { get; protected set; }

        #region ITerminalGrid

        ITerminal ITerminalGrid.Terminal => this.Terminal;

        GameObject ITerminalGrid.GameObject => this.gameObject;

        #endregion
    }
}
