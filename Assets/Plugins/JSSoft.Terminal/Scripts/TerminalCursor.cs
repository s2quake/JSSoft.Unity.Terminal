﻿// MIT License
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
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.Terminal
{
    public class TerminalCursor : MaskableGraphic, INotifyValidated
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private int cursorLeft;
        [SerializeField]
        private int cursorTop;
        [SerializeField]
        private bool isVisible = true;
        [SerializeField]
        private bool isFocused = false;
        [SerializeField]
        private TerminalCursorStyle style;
        [SerializeField]
        [Range(0, 100)]
        private int thickness = 2;
        [SerializeField]
        private bool isBlinkable;
        [SerializeField]
        [Range(0, 3)]
        private float blinkDelay = 0.6f;

        private readonly TerminalMesh terminalMesh = new TerminalMesh();
        private int volume = 1;
        private float delay;
        private bool blinkToggle;
        private bool isInView = true;

        public TerminalCursor()
        {

        }

        public TerminalGrid Grid
        {
            get => this.grid;
            internal set
            {
                this.grid = value ?? throw new ArgumentNullException(nameof(value));
                this.UpdateProperty();
            }
        }

        [FieldName(nameof(cursorLeft))]
        public int CursorLeft
        {
            get => this.cursorLeft;
            set
            {
                if (value < 0 || value >= this.ActualBufferWidth)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.cursorLeft != value)
                {
                    this.cursorLeft = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(CursorLeft));
                }
            }
        }

        [FieldName(nameof(cursorTop))]
        public int CursorTop
        {
            get => this.cursorTop;
            set
            {
                if (value < 0 || value >= this.ActualBufferHeight)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.cursorTop != value)
                {
                    this.cursorTop = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(CursorTop));
                }
            }
        }

        [FieldName(nameof(isVisible))]
        public bool IsVisible
        {
            get => this.isVisible;
            set
            {
                if (this.isVisible != value)
                {
                    this.isVisible = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(IsVisible));
                }
            }
        }

        [FieldName(nameof(isFocused))]
        public bool IsFocused
        {
            get => this.isFocused;
            set
            {
                if (this.isFocused != value)
                {
                    this.isFocused = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(IsFocused));
                }
            }
        }

        [FieldName(nameof(style))]
        public TerminalCursorStyle Style
        {
            get => this.style;
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(Style));
                }
            }
        }

        [FieldName(nameof(thickness))]
        public int Thickness
        {
            get => this.thickness;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.thickness != value)
                {
                    this.thickness = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(Thickness));
                }
            }
        }

        [FieldName(nameof(isBlinkable))]
        public bool IsBlinkable
        {
            get => this.isBlinkable;
            set
            {
                if (this.isBlinkable != value)
                {
                    this.isBlinkable = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(IsBlinkable));
                }
            }
        }

        [FieldName(nameof(blinkDelay))]
        public float BlinkDelay
        {
            get => this.blinkDelay;
            set
            {
                if (value < 0.0f)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.blinkDelay != value)
                {
                    this.blinkDelay = value;
                    this.SetVerticesDirty();
                    this.InvokePropertyChangedEvent(nameof(BlinkDelay));
                }
            }
        }

        public TerminalBase Terminal => this.grid?.Terminal;

        public event EventHandler Enabled;

        public event EventHandler Disabled;

        public event EventHandler Validated;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void InvokePropertyChangedEvent(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect);
            var itemRect = this.GetItemRect();
            if (this.isVisible == false || this.blinkToggle == true || this.isInView == false)
            {
                this.terminalMesh.Count = 0;
            }
            else if (this.isFocused == true)
            {
                this.terminalMesh.Count = 1;
                this.terminalMesh.SetVertex(0, itemRect, rect);
                this.terminalMesh.SetUV(0, (Vector2.zero, Vector2.one));
                this.terminalMesh.SetColor(0, base.color);
            }
            else
            {
                var thickness = this.thickness;
                var x = itemRect.x;
                var y = itemRect.y;
                var right = x + itemRect.width;
                var bottom = y + itemRect.height;
                var lt1 = new Vector2(x, y);
                var rt1 = new Vector2(right, y);
                var lb1 = new Vector2(x, bottom);
                var rb1 = new Vector2(right, bottom);
                var lt2 = new Vector2(x + thickness, y + thickness);
                var rt2 = new Vector2(right - thickness, y + thickness);
                var lb2 = new Vector2(x + thickness, bottom - thickness);
                var rb2 = new Vector2(right - thickness, bottom - thickness);
                this.terminalMesh.Count = 4;
                this.terminalMesh.SetVertex(0, lt1, rt1, lt2, rt2, rect);
                this.terminalMesh.SetVertex(1, lt1, lt2, lb1, lb2, rect);
                this.terminalMesh.SetVertex(2, lb2, rb2, lb1, rb1, rect);
                this.terminalMesh.SetVertex(3, rt2, rt1, rb2, rb1, rect);
                for (var i = 0; i < this.terminalMesh.Count; i++)
                {
                    this.terminalMesh.SetUV(i, (Vector2.zero, Vector2.one));
                    this.terminalMesh.SetColor(i, base.color);
                }
            }
            this.terminalMesh.Fill(vh);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.cursorLeft = Math.Min(this.ActualBufferWidth - 1, this.cursorLeft);
            this.cursorLeft = Math.Max(0, this.cursorLeft);
            this.cursorTop = Math.Min(this.ActualBufferHeight - 1, this.cursorTop);
            this.cursorTop = Math.Max(0, this.cursorTop);
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            this.UpdateProperty();
            TerminalValidationEvents.Register(this);
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalGridEvents.GotFocus += Grid_GotFocus;
            TerminalGridEvents.LostFocus += Grid_LostFocus;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            this.OnEnabled(EventArgs.Empty);
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.GotFocus -= Grid_GotFocus;
            TerminalGridEvents.LostFocus -= Grid_LostFocus;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            base.OnDisable();
            this.OnDisabled(EventArgs.Empty);
            TerminalValidationEvents.Unregister(this);
        }

        protected override void Start()
        {
            base.Start();
            this.delay = this.blinkDelay;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected virtual void Update()
        {
            if (Application.isPlaying == true && this.blinkDelay > 0 && this.isBlinkable == true && this.isFocused == true)
            {
                this.delay -= Time.deltaTime;
                if (this.delay < 0)
                {
                    this.delay += this.blinkDelay;
                    this.blinkToggle = !this.blinkToggle;
                    this.SetVerticesDirty();
                }
            }
        }

        protected virtual void OnEnabled(EventArgs e)
        {
            this.Enabled?.Invoke(this, e);
        }

        protected virtual void OnDisabled(EventArgs e)
        {
            this.Disabled?.Invoke(this, e);
        }

        protected virtual void OnValidated(EventArgs e)
        {
            this.Validated?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                switch (e.PropertyName)
                {
                    case nameof(ITerminalGrid.CursorStyle):
                        {
                            this.style = this.grid.CursorStyle;
                            this.SetVerticesDirty();
                        }
                        break;
                    case nameof(ITerminalGrid.CursorThickness):
                        {
                            this.thickness = this.grid.CursorThickness;
                            this.SetVerticesDirty();
                        }
                        break;
                    case nameof(ITerminalGrid.IsCursorBlinkable):
                        {
                            this.isBlinkable = this.grid.IsCursorBlinkable;
                            this.SetVerticesDirty();
                        }
                        break;
                    case nameof(ITerminalGrid.CursorBlinkDelay):
                        {
                            this.blinkDelay = this.grid.CursorBlinkDelay;
                            this.SetVerticesDirty();
                        }
                        break;
                    case nameof(ITerminalGrid.IsCursorVisible):
                        {
                            this.isVisible = this.grid.IsCursorVisible;
                            this.SetVerticesDirty();
                        }
                        break;
                    case nameof(ITerminalGrid.CursorPoint):
                    case nameof(ITerminalGrid.VisibleIndex):
                        {
                            this.UpdateCursorPoint();
                        }
                        break;
                    case nameof(ITerminalGrid.Style):
                        {
                            this.UpdateProperty();
                        }
                        break;
                }
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.UpdateProperty();
            }
        }

        private void Grid_GotFocus(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.IsFocused = this.grid.IsFocused;
            }
        }

        private void Grid_LostFocus(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.IsFocused = this.grid.IsFocused;
            }
        }

        private void UpdateCursorPoint()
        {
            this.cursorLeft = this.grid.CursorPoint.X;
            this.cursorTop = this.grid.CursorPoint.Y - this.grid.VisibleIndex;
            if (this.grid.GetCell(this.grid.CursorPoint) is TerminalCell cell)
            {
                this.volume = Math.Max(cell.Volume, 1);
            }
            this.SetVerticesDirty();
        }

        private void UpdateProperty()
        {
            if (this.grid != null)
            {
                this.cursorLeft = this.grid.CursorPoint.X;
                this.cursorTop = this.grid.CursorPoint.Y - this.grid.VisibleIndex;
                if (this.grid.GetCell(this.grid.CursorPoint) is TerminalCell cell)
                {
                    this.volume = Math.Max(cell.Volume, 1);
                }
                this.isVisible = this.grid.IsCursorVisible;
                this.isInView = this.grid.CursorPoint.Y >= this.grid.VisibleIndex && this.grid.CursorPoint.Y < this.grid.VisibleIndex + this.grid.ActualBufferHeight;
                this.color = this.grid.CursorColor;
                this.style = this.grid.CursorStyle;
                this.thickness = this.grid.CursorThickness;
                this.isBlinkable = this.grid.IsCursorBlinkable;
                this.blinkDelay = this.grid.CursorBlinkDelay;
                this.delay = this.grid.CursorBlinkDelay;
                this.blinkToggle = false;
                this.SetVerticesDirty();
            }
        }

        private GlyphRect GetItemRect()
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(this.grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(this.grid);
            var padding = TerminalGridUtility.GetPadding(this.grid);
            var x = this.cursorLeft * itemWidth + padding.Left;
            var y = this.cursorTop * itemHeight + padding.Top;
            var volumn = this.volume;
            var thickness = this.thickness;
            switch (this.style)
            {
                case TerminalCursorStyle.Block:
                    return new GlyphRect(x, y, itemWidth * volume, itemHeight);
                case TerminalCursorStyle.Underline:
                    return new GlyphRect(x, y + itemHeight - thickness, itemWidth * volume, thickness);
                case TerminalCursorStyle.VerticalBar:
                    return new GlyphRect(x, y, thickness, itemHeight);
            }
            throw new NotImplementedException();
        }

        private int ActualBufferWidth => this.grid != null ? this.grid.ActualBufferWidth : 0;

        private int ActualBufferHeight => this.grid != null ? this.grid.ActualBufferHeight : 0;
    }
}
