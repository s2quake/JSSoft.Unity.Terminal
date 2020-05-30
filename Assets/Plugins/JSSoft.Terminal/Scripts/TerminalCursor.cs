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
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.Terminal
{
    class TerminalCursor : MaskableGraphic
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
        private char character;
        private bool isInView = true;

        public TerminalCursor()
        {

        }

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        public int CursorLeft
        {
            get => this.cursorLeft;
            set
            {
                if (value < 0 || value >= this.BufferWidth)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.cursorLeft = value;
                this.SetVerticesDirty();
            }
        }

        public int CursorTop
        {
            get => this.cursorTop;
            set
            {
                if (value < 0 || value >= this.BufferHeight)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.cursorTop = value;
                this.SetVerticesDirty();
            }
        }

        public bool IsVisible
        {
            get => this.isVisible;
            set
            {
                this.isVisible = value;
                this.SetVerticesDirty();
            }
        }

        public bool IsFocused
        {
            get => this.isFocused;
            set
            {
                this.isFocused = value;
                this.SetVerticesDirty();
            }
        }

        public TerminalCursorStyle Style
        {
            get => this.style;
            set
            {
                this.style = value;
                this.SetVerticesDirty();
            }
        }

        public int Thickness
        {
            get => this.thickness;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.thickness = value;
                this.SetVerticesDirty();
            }
        }

        public bool IsBlinkable
        {
            get => this.isBlinkable;
            set
            {
                this.isBlinkable = value;
                this.SetVerticesDirty();
            }
        }

        public float BlinkDelay
        {
            get => this.blinkDelay;
            set
            {
                if (value < 0.0f)
                    throw new ArgumentOutOfRangeException(nameof(value));
                this.blinkDelay = value;
                this.SetVerticesDirty();
            }
        }

        public TerminalBase Terminal => this.grid?.Terminal;

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
                    this.terminalMesh.SetColor(i, TerminalColors.Gray);
                }
            }
            this.material.color = base.color;
            this.terminalMesh.Fill(vh);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.cursorLeft = Math.Min(this.BufferWidth - 1, this.cursorLeft);
            this.cursorLeft = Math.Max(0, this.cursorLeft);
            this.cursorTop = Math.Min(this.BufferHeight - 1, this.cursorTop);
            this.cursorTop = Math.Max(0, this.cursorTop);
            this.SetVerticesDirty();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetVerticesDirty();
            TerminalEvents.Validated += Terminal_Validated;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalGridEvents.GotFocus += Grid_GotFocus;
            TerminalGridEvents.LostFocus += Grid_LostFocus;
            TerminalGridEvents.Validated += Grid_Validated;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalValidationEvents.Validated += Object_Validated;
            this.isVisible = this.grid != null ? this.grid.IsCursorVisible : true;
            base.color = TerminalGridUtility.GetCursorColor(this.grid);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalEvents.Validated -= Terminal_Validated;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.GotFocus -= Grid_GotFocus;
            TerminalGridEvents.LostFocus -= Grid_LostFocus;
            TerminalGridEvents.Validated -= Grid_Validated;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
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

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (object.Equals(sender, this.grid) == false)
                return;

            var propertyName = e.PropertyName;
            if (propertyName == nameof(ITerminalGrid.CursorStyle))
            {
                this.style = this.grid.CursorStyle;
            }
            else if (propertyName == nameof(ITerminalGrid.CursorThickness))
            {
                this.thickness = this.grid.CursorThickness;
            }
            else if (propertyName == nameof(ITerminalGrid.IsCursorBlinkable))
            {
                this.isBlinkable = this.grid.IsCursorBlinkable;
            }
            else if (propertyName == nameof(ITerminalGrid.CursorBlinkDelay))
            {
                this.blinkDelay = this.grid.CursorBlinkDelay;
            }
            else if (propertyName == nameof(ITerminalGrid.IsCursorVisible))
            {
                this.isVisible = this.grid.IsCursorVisible;
                this.SetVerticesDirty();
            }
            else if (propertyName == nameof(ITerminalGrid.CursorPoint))
            {
                this.UpdateLayout();
            }
            else if (propertyName == nameof(ITerminalGrid.VisibleIndex))
            {
                this.UpdateLayout();
            }
            else if (propertyName == nameof(ITerminalGrid.Text))
            {
                var command = this.Terminal.Command;
                var position = this.Terminal.CursorPosition;
                var character = position < command.Length ? command[position] : char.MinValue;
                if (this.character != character)
                {
                    this.UpdateLayout();
                }
                this.character = character;
            }
            this.SetVerticesDirty();
        }

        private void Terminal_Validated(object sender, EventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.grid?.Terminal)
            {
                this.color = this.grid.CursorColor;
                this.UpdateLayout();
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.color = this.grid.CursorColor;
                this.UpdateLayout();
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

        private async void Grid_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                await Task.Delay(1);
                this.UpdateLayout();
            }
        }

        private async void Object_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalStyle style && style == this.grid?.Style)
            {
                await Task.Delay(1);
                this.UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            if (this.IsDestroyed() == true)
                return;
            var point = this.grid.CursorPoint;
            this.cursorLeft = this.grid.CursorPoint.X;
            this.cursorTop = this.grid.CursorPoint.Y - this.grid.VisibleIndex;
            if (this.grid.GetCell(point) is TerminalCell cell)
            {
                this.volume = Math.Max(cell.Volume, 1);
            }
            this.isVisible = this.grid.IsCursorVisible;
            this.isInView = this.grid.CursorPoint.Y >= this.grid.VisibleIndex && this.grid.CursorPoint.Y < this.grid.VisibleIndex + this.grid.BufferHeight;
            base.color = TerminalGridUtility.GetCursorColor(this.grid);
            this.style = this.grid.CursorStyle;
            this.thickness = this.grid.CursorThickness;
            this.isBlinkable = this.grid.IsCursorBlinkable;
            this.blinkDelay = this.grid.CursorBlinkDelay;
            this.delay = this.grid.CursorBlinkDelay;
            this.blinkToggle = false;
            this.SetVerticesDirty();
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

        private int BufferWidth => this.grid != null ? this.grid.BufferWidth : 0;

        private int BufferHeight => this.grid != null ? this.grid.BufferHeight : 0;
    }
}
