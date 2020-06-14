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
    [ExecuteAlways]
    public class TerminalCompositionForeground : MaskableGraphic
    {
        [SerializeField]
        private TerminalComposition composition;

        private readonly TerminalMesh terminalMesh = new TerminalMesh();
        private Texture texture;

        public TerminalCompositionForeground()
        {

        }

        public TerminalGrid Grid => this.composition?.Grid;

        public TerminalComposition Composition
        {
            get => this.composition;
            internal set
            {
                this.composition = value ?? throw new ArgumentNullException(nameof(value));
                this.color = this.composition.ForegroundColor;
            }
        }

        public override Texture mainTexture => this.texture;

        protected override void OnEnable()
        {
            base.OnEnable();
            base.material = new Material(Shader.Find("UI/Default"));
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalValidationEvents.PropertyChanged += Composition_PropertyChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalValidationEvents.PropertyChanged -= Composition_PropertyChanged;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            if (this.composition == null)
            {
                vh.Clear();
                return;
            }

            var text = this.composition.Text;
            var columnIndex = this.composition.ColumnIndex;
            var rowIndex = this.composition.RowIndex;
            var grid = this.composition.Grid;
            var bufferWidth = grid != null ? grid.ActualBufferWidth : 0;
            var bufferHeight = grid != null ? grid.ActualBufferHeight : 0;
            var font = composition.Font;
            var offset = composition.Offset;
            var foregroundMargin = composition.ForegroundMargin;
            if (columnIndex < bufferWidth && rowIndex < bufferHeight && text != string.Empty)
            {
                var rect = TerminalGridUtility.TransformRect(grid, this.rectTransform.rect, false);
                var character = text.First();
                var characterInfo = font[character];
                var texture = characterInfo.Texture;
                var itemWidth = TerminalGridUtility.GetItemWidth(grid);
                var itemHeight = TerminalGridUtility.GetItemHeight(grid);
                var volume = FontUtility.GetCharacterVolume(font, character);
                var padding = TerminalGridUtility.GetPadding(grid);
                var bx = columnIndex * itemWidth + padding.Left + (int)offset.x;
                var by = rowIndex * itemHeight + padding.Top + (int)offset.y;
                var foregroundRect = FontUtility.GetForegroundRect(font, character, bx, by) + foregroundMargin;
                var uv = FontUtility.GetUV(font, character);
                this.texture = texture;
                this.terminalMesh.Count = 1;
                this.terminalMesh.SetVertex(0, foregroundRect, rect);
                this.terminalMesh.SetUV(0, uv);
                this.terminalMesh.SetColor(0, this.color);
                this.terminalMesh.Fill(vh);
            }
            else
            {
                vh.Clear();
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // if (sender is TerminalGrid grid && grid == this.Grid)
            // {
            //     switch (e.PropertyName)
            //     {
            //         case nameof(ITerminalGrid.VisibleIndex):
            //         case nameof(ITerminalGrid.CursorPoint):
            //             {
            //                 this.SetVerticesDirty();
            //             }
            //             break;
            //         case nameof(ITerminalGrid.CompositionString):
            //             {
            //                 this.SetVerticesDirty();
            //                 this.SetMaterialDirty();
            //             }
            //             break;
            //     }
            // }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
        }

        private void Composition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalComposition composition && composition == this.composition)
            {
                switch (e.PropertyName)
                {
                    case nameof(TerminalComposition.ForegroundColor):
                        {
                            this.color = this.composition.ForegroundColor;
                            this.SetVerticesDirty();
                        }
                        break;
                    case nameof(TerminalComposition.ForegroundMargin):
                    case nameof(TerminalComposition.Text):
                    case nameof(TerminalComposition.ColumnIndex):
                    case nameof(TerminalComposition.RowIndex):
                        {
                            this.SetVerticesDirty();
                        }
                        break;
                }
            }
        }
    }
}
