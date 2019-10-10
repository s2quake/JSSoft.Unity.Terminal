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
        private char character;

        public TerminalCell(TerminalRow row, int index)
        {
            this.Index = index;
            this.Row = row;
            this.UpdateLayout();
        }

        public override string ToString()
        {
            return $"{{{this.Index},{this.Row.Index}}}: '{this.character}'";
        }

        public static Color32 GetBackgroundColor(TerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            return cell.BackgroundColor ?? TerminalRow.GetBackgroundColor(cell.Row);
        }

        public static Color32 GetForegroundColor(TerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            return cell.ForegroundColor ?? TerminalRow.GetForegroundColor(cell.Row);
        }

        public bool Intersect(Vector2 position)
        {
            return this.BackgroundRect.Intersect(position);
        }

        public int Index { get; }

        public TerminalRow Row { get; }

        public TerminalGrid Grid => this.Row.Grid;

        public TMP_FontAsset FontAsset { get; private set; }

        public char Character
        {
            get => this.character;
            set
            {
                this.character = value;
                this.UpdateLayout();
            }
        }

        public int Volume { get; private set; }

        public bool IsSelected { get; set; }

        public GlyphRect BackgroundRect { get; private set; }

        public Rect ForegroundRect { get; private set; }

        public (Vector2, Vector2) BackgroundUV { get; private set; }

        public (Vector2, Vector2) ForegroundUV { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public TerminalPoint Point => new TerminalPoint(this.Index, this.Row.Index);

        private void UpdateLayout()
        {
            var originAsset = this.Grid.FontAsset;
            var character = this.character;
            var rect = TerminalGrid.GetCellRect(this.Grid, this);
            var fontAsset = FontUtility.GetFontAsset(originAsset, character);
            if (fontAsset != null)
            {
                var characterWidth = TerminalGrid.GetItemWidth(this.Grid, character);
                this.Volume = FontUtility.GetCharacterVolume(fontAsset, character);
                this.BackgroundRect = new GlyphRect(rect.x, rect.y, characterWidth, rect.height);
                this.ForegroundRect = FontUtility.GetForegroundRect(fontAsset, character, rect.x, rect.y);
                this.BackgroundUV = (Vector2.zero, Vector2.zero);
                this.ForegroundUV = FontUtility.GetUV(fontAsset, character);
                this.FontAsset = fontAsset;
            }
            else
            {
                var uv = (Vector2.zero, Vector2.zero);
                this.Volume = 0;
                this.BackgroundRect = rect;
                this.ForegroundRect = new Rect(rect.x, rect.y, rect.width, rect.height);
                this.BackgroundUV = uv;
                this.ForegroundUV = uv;
            }
            // this.TextIndex = -1;
        }
    }
}
