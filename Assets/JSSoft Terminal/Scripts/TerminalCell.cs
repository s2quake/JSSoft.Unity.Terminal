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
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.UI
{
    class TerminalCell : ITerminalCell
    {
        private readonly Action modifiedAction;

        public TerminalCell(TerminalRow row, int index, Action modifiedAction)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.Row = row ?? throw new ArgumentNullException(nameof(row));
            this.Index = index;
            this.modifiedAction = modifiedAction ?? throw new ArgumentNullException(nameof(modifiedAction));
            this.Reset();
        }

        public override string ToString()
        {
            return $"{{{this.Index},{this.Row.Index}}}: '{this.Character}'";
        }

        public static Color32 GetBackgroundColor(ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            return cell.BackgroundColor ?? TerminalRow.GetBackgroundColor(cell.Row);
        }

        public static Color32 GetForegroundColor(ITerminalCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));
            return cell.ForegroundColor ?? TerminalRow.GetForegroundColor(cell.Row);
        }

        public bool Intersect(Vector2 position)
        {
            return this.BackgroundRect.Intersect(position);
        }

        public void SetCharacter(TMP_FontAsset fontAsset, char character, int volume)
        {
            if (fontAsset == null)
                throw new ArgumentException(nameof(fontAsset));
            var rect = GetCellRect(this.Grid, this);
            this.IsEnabled = true;
            this.Character = character;
            this.Volume = volume;
            this.BackgroundRect = new GlyphRect(rect.x, rect.y, rect.width * volume, rect.height);
            this.ForegroundRect = FontUtility.GetForegroundRect(fontAsset, character, rect.x, rect.y);
            this.BackgroundUV = (Vector2.zero, Vector2.zero);
            this.ForegroundUV = FontUtility.GetUV(fontAsset, character);
            this.FontAsset = fontAsset;
            this.modifiedAction();
        }

        public void Reset()
        {
            var rect = GetCellRect(this.Grid, this);
            var uv = (Vector2.zero, Vector2.zero);
            this.IsEnabled = false;
            this.Character = char.MinValue;
            this.Volume = 0;
            this.BackgroundRect = rect;
            this.ForegroundRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            this.BackgroundUV = uv;
            this.ForegroundUV = uv;
            this.FontAsset = null;
            this.BackgroundColor = null;
            this.ForegroundColor = null;
            this.FontAsset = null;
            this.modifiedAction();
        }

        public int Index { get; }

        public TerminalRow Row { get; }

        public TerminalGrid Grid => this.Row.Grid;

        public TMP_FontAsset FontAsset { get; private set; }

        public char Character { get; private set; }

        public int Volume { get; private set; }

        public bool IsSelected => TerminalGridUtility.IsSelected(this.Grid, this.Point);

        // public bool IsSelected
        // {
        //     get => this.isSelected;
        //     set
        //     {
        //         if (this.isSelected != value)
        //         {
        //             this.isSelected = value;
        //             this.modifiedAction();
        //         }
        //     }
        // }

        public bool IsEnabled { get; private set; }

        public GlyphRect BackgroundRect { get; private set; }

        public Rect ForegroundRect { get; private set; }

        public (Vector2, Vector2) BackgroundUV { get; private set; }

        public (Vector2, Vector2) ForegroundUV { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public TerminalPoint Point => new TerminalPoint(this.Index, this.Row.Index);

        private static GlyphRect GetCellRect(TerminalGrid grid, ITerminalCell cell)
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(grid);
            var x = cell.Index * itemWidth;
            var y = cell.Row.Index * itemHeight;
            return new GlyphRect(x, y, itemWidth, itemHeight);
        }

        #region ITerminalCell

        ITerminalRow ITerminalCell.Row => this.Row;

        #endregion
    }
}
