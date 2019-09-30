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
        private GlyphRect backgroundRect;
        private GlyphRect foregroundRect;
        private (Vector2, Vector2) backgroundUV;
        private (Vector2, Vector2) foregroundUV;

        public TerminalCell(int index, TerminalRow row)
        {
            this.Index = index;
            this.Row = row;
        }

        public void Refresh(TMP_FontAsset fontAsset, char character)
        {
            this.FontAsset = FontUtility.GetFontAsset(fontAsset, character);
            this.Character = character;
            this.Volume = FontUtility.GetCharacterVolume(fontAsset, character);
            this.UpdateRect();
        }

        public void UpdateRect()
        {
            var itemWidth = FontUtility.GetItemWidth(this.FontAsset);
            var characterInfo = this.FontAsset.characterLookupTable[this.Character];
            var texture = this.FontAsset.atlasTexture;
            var glyph = characterInfo.glyph;
            var glyphRect = glyph.glyphRect;
            var textWidth = (float)texture.width;
            var textHeight = (float)texture.height;
            var uv0 = new Vector2(glyphRect.x / textWidth, glyphRect.y / textHeight);
            var uv1 = new Vector2((glyphRect.x + glyphRect.width) / textWidth, (glyphRect.y + glyphRect.height) / textHeight);
            var bx = this.Index * itemWidth;
            var by = this.FontAsset.faceInfo.lineHeight * this.Row.Index;
            var fx = bx + glyph.metrics.horizontalBearingX;
            var fy = by + this.FontAsset.faceInfo.ascentLine - glyph.metrics.horizontalBearingY;
            this.backgroundRect = new GlyphRect((int)bx, (int)by, (int)glyph.metrics.horizontalAdvance, (int)this.FontAsset.faceInfo.lineHeight);
            this.foregroundRect = new GlyphRect((int)fx, (int)fy, glyphRect.width, glyphRect.height);
            this.backgroundUV = (Vector2.zero, Vector2.zero);
            this.foregroundUV = (uv0, uv1);
            this.BackgroundColor = this.Index % 2 == 0 ? TerminalColors.Blue : TerminalColors.Red;
            this.ForegroundColor = TerminalColors.Black;
        }

        public void Clear()
        {
            this.FontAsset = null;
            this.Character = char.MinValue;
            this.Volume = 0;
        }

        public int Index { get; }

        public TerminalRow Row { get; }

        public TMP_FontAsset FontAsset { get; private set; }

        public char Character { get; private set; }

        public int Volume { get; private set; }

        public GlyphRect BackgroundRect => this.backgroundRect;

        public GlyphRect ForegroundRect => this.foregroundRect;

        public (Vector2, Vector2) BackgroundUV => this.backgroundUV;

        public (Vector2, Vector2) ForegroundUV => this.foregroundUV;

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }
    }
}
