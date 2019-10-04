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
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalCell
    {
        private Color32? backgroundColor;
        private Color32? foregroundColor;

        public TerminalCell(TerminalRow row, int index)
        {
            this.Index = index;
            this.Row = row;
            this.UpdateRect();
        }

        public void Refresh(TMP_FontAsset fontAsset, char character)
        {
            this.FontAsset = FontUtility.GetFontAsset(fontAsset, character);
            this.OriginAsset = fontAsset;
            this.Character = character;
            this.Volume = FontUtility.GetCharacterVolume(fontAsset, character);
            this.UpdateLayout();
        }

        public void UpdateLayout()
        {
            var itemWidth = FontUtility.GetItemWidth(this.OriginAsset);
            var characterWidth = FontUtility.GetItemWidth(this.OriginAsset, this.Character);
            var characterInfo = this.FontAsset.characterLookupTable[this.Character];
            var texture = this.FontAsset.atlasTexture;
            var glyph = characterInfo.glyph;
            var glyphRect = glyph.glyphRect;
            var textWidth = (float)texture.width;
            var textHeight = (float)texture.height;
            var bx = this.Index * itemWidth;
            var by = this.FontAsset.faceInfo.lineHeight * this.Row.Index;
            var fx = bx + glyph.metrics.horizontalBearingX;
            var fy = by + this.FontAsset.faceInfo.ascentLine - glyph.metrics.horizontalBearingY;
            this.BackgroundRect = new GlyphRect((int)bx, (int)by, characterWidth, (int)this.FontAsset.faceInfo.lineHeight);
            this.ForegroundRect = new GlyphRect((int)fx, (int)fy, glyphRect.width, glyphRect.height);
            this.BackgroundUV = (Vector2.zero, Vector2.zero);
            this.ForegroundUV = FontUtility.GetUV(this.FontAsset, this.Character);
        }

        public void Clear()
        {
            this.FontAsset = null;
            this.OriginAsset = null;
            this.Character = char.MinValue;
            this.Volume = 0;
        }

        public bool Intersect(Vector2 position)
        {
            return this.BackgroundRect.Intersect(position);
        }

        public int Index { get; }

        public TerminalRow Row { get; }

        public TMP_FontAsset FontAsset { get; private set; }

        public TMP_FontAsset OriginAsset { get; private set; }

        public char Character { get; private set; }

        public int Volume { get; private set; }

        public bool IsSelected { get; set; }

        public GlyphRect BackgroundRect { get; private set; }

        public GlyphRect ForegroundRect { get; private set; }

        public (Vector2, Vector2) BackgroundUV { get; private set; }

        public (Vector2, Vector2) ForegroundUV { get; private set; }

        public Color32? BackgroundColor
        {
            get => this.backgroundColor ?? this.Row.BackgroundColor;
            set => this.backgroundColor = value;
        }

        public Color32? ForegroundColor
        {
            get => this.foregroundColor ?? this.Row.ForegroundColor;
            set => this.foregroundColor = value;
        }

        private void UpdateRect()
        {
            var itemWidth = TerminalGrid.GetItemWidth(this.Row.Grid);
            var itemHeight = TerminalGrid.GetItemHeight(this.Row.Grid);
            var x = this.Index * itemWidth;
            var y = this.Row.Index * itemHeight;
            var width = itemWidth;
            var height = itemHeight;
            this.BackgroundRect = new GlyphRect(x, y, width, height);
        }
    }
}
