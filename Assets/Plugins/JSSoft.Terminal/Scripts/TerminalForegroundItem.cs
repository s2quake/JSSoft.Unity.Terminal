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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Terminal
{
    class TerminalForegroundItem : MaskableGraphic
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        private Texture2D texture;

        private TerminalMesh terminalMesh = new TerminalMesh();

        public TerminalForegroundItem()
        {

        }

        public override Texture mainTexture => this.texture;

        public TerminalFont Font => this.grid?.Font;

        public TerminalGrid Grid
        {
            get => this.grid;
            internal set => this.grid = value;
        }

        public Texture2D Texture
        {
            get => this.texture;
            internal set => this.texture = value;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var rect = TerminalGridUtility.TransformRect(this.grid, this.rectTransform.rect, true);
            var visibleCells = TerminalGridUtility.GetVisibleCells(this.grid, item => item.Character != 0 && item.Texture == this.texture);
            var index = 0;
            this.terminalMesh.Count = visibleCells.Count();
            foreach (var item in visibleCells)
            {
                this.terminalMesh.SetVertex(index, item.ForegroundRect, rect);
                this.terminalMesh.SetUV(index, item.ForegroundUV);
                this.terminalMesh.SetColor(index, TerminalCell.GetForegroundColor(item));
                index++;
            }
            this.terminalMesh.Fill(vh);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalEvents.Validated += Terminal_Validated;
            TerminalEvents.Enabled += Terminal_Enabled;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.Validated += Grid_Validated;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalValidationEvents.Validated += Object_Validated;
        }

        protected override void OnDisable()
        {
            TerminalEvents.Validated -= Terminal_Validated;
            TerminalEvents.Enabled -= Terminal_Enabled;
            TerminalGridEvents.Validated -= Grid_Validated;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalValidationEvents.Validated -= Object_Validated;
            base.OnDisable();
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (object.Equals(sender, this.grid) == false)
                return;

            switch (e.PropertyName)
            {
                case nameof(ITerminalGrid.VisibleIndex):
                case nameof(ITerminalGrid.Text):
                case nameof(ITerminalGrid.SelectingRange):
                    {
                        if (this.IsDestroyed() == false)
                            this.SetVerticesDirty();
                    }
                    break;
                case nameof(ITerminalGrid.Style):
                    {
                        if (this.IsDestroyed() == false)
                        {
                            this.SetVerticesDirty();
                            Debug.Log(this.grid);
                        }
                    }
                    break;
            }
        }

        private void Terminal_Validated(object sender, EventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.grid.Terminal)
            {
                this.SetVerticesDirty();
            }
        }

        private void Terminal_Enabled(object sender, EventArgs e)
        {
            if (sender is Terminal terminal && terminal == this.grid.Terminal)
            {
            }
        }

        private void Grid_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
            {
                this.SetVerticesDirty();
            }
        }

        private void Object_Validated(object sender, EventArgs e)
        {
            switch (sender)
            {
                case TerminalStyle style when this.grid?.Style:
                    this.SetVerticesDirty();
                    Debug.Log(this.grid);
                    break;
                case TerminalColorPalette palette when this.grid?.ColorPalette:
                    this.SetVerticesDirty();
                    break;
            }
        }
    }
}
