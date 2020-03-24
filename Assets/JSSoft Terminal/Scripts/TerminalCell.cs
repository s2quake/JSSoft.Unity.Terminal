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

namespace JSSoft.UI
{
    class TerminalCell : ITerminalCell
    {
        private int volume;

        public TerminalCell(TerminalRow row, int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.Row = row ?? throw new ArgumentNullException(nameof(row));
            this.Index = index;
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

        public void SetCharacter(TerminalCharacterInfo characterInfo)
        {
            var rect = GetCellRect(this.Grid, this);
            var character = characterInfo.Character;
            var volume = characterInfo.Volume;
            var texture = characterInfo.Texture;
            var indexOfText = characterInfo.TextIndex;
            var backgroundColor = characterInfo.BackgroundColor;
            var foregroundColor = characterInfo.ForegroundColor;
            this.Character = character;
            this.Volume = volume;
            this.Texture = texture;
            this.TextIndex = indexOfText;
            this.BackgroundColor = backgroundColor;
            this.BackgroundRect = new Rect(rect.x, rect.y, rect.width * volume, rect.height);
            this.BackgroundUV = (Vector2.zero, Vector2.zero);
            if (characterInfo.Texture != null)
            {
                this.ForegroundColor = foregroundColor;
                this.ForegroundRect = FontUtility.GetForegroundRect(this.Font, character, (int)rect.x, (int)rect.y);
                this.ForegroundUV = FontUtility.GetUV(this.Font, character);
            }
            for (var i = 1; i < this.Volume; i++)
            {
                this.Row.Cells[this.Index + i].Reset();
            }
        }

        public void Reset()
        {
            var rect = GetCellRect(this.Grid, this);
            var uv = (Vector2.zero, Vector2.zero);
            this.Character = char.MinValue;
            this.Volume = 1;
            this.TextIndex = -1;
            this.BackgroundColor = null;
            this.ForegroundColor = null;
            this.BackgroundRect = rect;
            this.ForegroundRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            this.BackgroundUV = uv;
            this.ForegroundUV = uv;
        }

        public int Index { get; }

        public TerminalRow Row { get; }

        public TerminalGrid Grid => this.Row.Grid;

        public TerminalFont Font => this.Grid.Font;

        public Texture2D Texture { get; private set; }

        public char Character { get; private set; }

        public int Volume
        {
            get => this.volume;
            private set
            {
                var row = this.Row;
                var cells = row.Cells;
                for (var i = 1; i < this.volume; i++)
                {
                    var cell = cells[i];
                    cell.volume = -i;
                }
                this.volume = value;
            }
        }

        public bool IsSelected => TerminalGridUtility.IsSelected(this.Grid, this.Point);

        public Rect BackgroundRect { get; private set; }

        public Rect ForegroundRect { get; private set; }

        public (Vector2, Vector2) BackgroundUV { get; private set; }

        public (Vector2, Vector2) ForegroundUV { get; private set; }

        public Color32? BackgroundColor { get; set; }

        public Color32? ForegroundColor { get; set; }

        public int TextIndex { get; private set; }

        public TerminalPoint Point => new TerminalPoint(this.Index, this.Row.Index);

        private static Rect GetCellRect(TerminalGrid grid, ITerminalCell cell)
        {
            var itemWidth = TerminalGridUtility.GetItemWidth(grid);
            var itemHeight = TerminalGridUtility.GetItemHeight(grid);
            var x = cell.Index * itemWidth + grid.Padding.Left;
            var y = cell.Row.Index * itemHeight + grid.Padding.Top;
            return new Rect(x, y, itemWidth, itemHeight);
        }

        #region ITerminalCell

        ITerminalRow ITerminalCell.Row => this.Row;

        #endregion
    }
}
