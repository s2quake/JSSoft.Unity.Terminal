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
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalCursor : MaskableGraphic
    {
        private static readonly int lineWidth = 2;
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

        private int volume = 1;
        private readonly TerminalRect terminalRect = new TerminalRect();

        public TerminalCursor()
        {

        }

        public int CursorLeft
        {
            get => this.cursorLeft;
            set
            {
                if (value < 0 || value >= this.ColumnCount)
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
                if (value < 0 || value >= this.RowCount)
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

        public Terminal Terminal => this.grid?.Terminal;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect);
            var itemWidth = TerminalGridUtility.GetItemWidth(this.grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(this.grid);
            var x = this.cursorLeft * itemWidth;
            var y = this.cursorTop * itemHeight;
            var itemRect = new GlyphRect(x, y, itemWidth * this.volume, itemHeight);
            if (this.isVisible == false)
            {
                this.terminalRect.Count = 0;
            }
            else if (this.isFocused == true)
            {
                this.terminalRect.Count = 1;
                this.terminalRect.SetVertex(0, itemRect, rect);
                this.terminalRect.SetUV(0, (Vector2.zero, Vector2.one));
                this.terminalRect.SetColor(0, base.color);
            }
            else
            {
                var right = x + itemWidth * this.volume;
                var bottom = y + itemHeight;
                var size = lineWidth;
                var lt1 = new Vector2(x, y);
                var rt1 = new Vector2(right, y);
                var lb1 = new Vector2(x, bottom);
                var rb1 = new Vector2(right, bottom);
                var lt2 = new Vector2(x + size, y + size);
                var rt2 = new Vector2(right - size, y + size);
                var lb2 = new Vector2(x + size, bottom - size);
                var rb2 = new Vector2(right - size, bottom - size);
                this.terminalRect.Count = 4;
                this.terminalRect.SetVertex(0, lt1, rt1, lt2, rt2, rect);
                this.terminalRect.SetVertex(1, lt1, lt2, lb1, lb2, rect);
                this.terminalRect.SetVertex(2, lb2, rb2, lb1, rb1, rect);
                this.terminalRect.SetVertex(3, rt2, rt1, rb2, rb1, rect);

                for (var i = 0; i < this.terminalRect.Count; i++)
                {
                    this.terminalRect.SetUV(i, (Vector2.zero, Vector2.one));
                    this.terminalRect.SetColor(i, TerminalColors.Gray);
                }
            }
            this.material.color = base.color;
            this.terminalRect.Fill(vh);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            this.cursorLeft = Math.Min(this.ColumnCount - 1, this.cursorLeft);
            this.cursorLeft = Math.Max(0, this.cursorLeft);
            this.cursorTop = Math.Min(this.RowCount - 1, this.cursorTop);
            this.cursorTop = Math.Max(0, this.cursorTop);
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetVerticesDirty();
            TerminalGridEvents.CursorPointChanged += TerminalGrid_CursorPointChanged;
            TerminalGridEvents.LayoutChanged += TerminalGrid_LayoutChanged;
            TerminalGridEvents.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
            TerminalGridEvents.GotFocus += TerminalGrid_GotFocus;
            TerminalGridEvents.LostFocus += TerminalGrid_LostFocus;
            TerminalGridEvents.Validated += TerminalGrid_Validated;
            if (this.grid != null)
            {
                this.isVisible = this.grid.IsCursorVisible;
                base.color = TerminalGridUtility.GetCursorColor(this.grid);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalGridEvents.CursorPointChanged -= TerminalGrid_CursorPointChanged;
            TerminalGridEvents.LayoutChanged -= TerminalGrid_LayoutChanged;
            TerminalGridEvents.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
            TerminalGridEvents.GotFocus -= TerminalGrid_GotFocus;
            TerminalGridEvents.LostFocus -= TerminalGrid_LostFocus;
            TerminalGridEvents.Validated -= TerminalGrid_Validated;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        private void TerminalGrid_CursorPointChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.UpdateLayout();
            }
        }

        private void TerminalGrid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.color = this.grid.CursorColor;
                this.UpdateLayout();
            }
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.UpdateLayout();
            }
        }

        private void TerminalGrid_GotFocus(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.IsFocused = this.grid.IsFocused;
            }
        }

        private void TerminalGrid_LostFocus(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.IsFocused = this.grid.IsFocused;
            }
        }

        private void TerminalGrid_Validated(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid == this.grid)
            {
                this.UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            var point = this.grid.CursorPoint;
            this.cursorLeft = this.grid.CursorPoint.X;
            this.cursorTop = this.grid.CursorPoint.Y - this.grid.VisibleIndex;
            if (this.grid.GetCell(point) is TerminalCell cell)
            {
                this.volume = Math.Max(cell.Volume, 1);
            }
            this.isVisible = this.grid.IsCursorVisible;
            base.color = TerminalGridUtility.GetCursorColor(this.grid);
            this.SetVerticesDirty();
        }

        private int ColumnCount => this.grid != null ? this.grid.ColumnCount : 0;

        private int RowCount => this.grid != null ? this.grid.RowCount : 0;
    }
}
