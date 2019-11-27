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
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace JSSoft.UI
{
    class TerminalCharacterInfoCollection : IReadOnlyList<TerminalCharacterInfo>
    {
        private readonly TerminalGrid grid;
        private TerminalCharacterInfo[] items = new TerminalCharacterInfo[] { };
        private TerminalPoint lt;
        private TerminalPoint rb;
        private TerminalFont font;
        private string text = string.Empty;
        private int columnCount;
        private int rowCount;

        public TerminalCharacterInfoCollection(TerminalGrid grid)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public void Update()
        {
            var font = this.grid.Font;
            var text = this.grid.Text + char.MinValue;
            var columnCount = this.grid.ColumnCount;
            var rowCount = this.grid.RowCount;
            if (this.text != text || this.columnCount != columnCount || this.rowCount != rowCount)
            {
                var index = this.FindUpdateIndex(font, text, columnCount, rowCount);
                var point = this.items.Any() ? this.items[index].Point : TerminalPoint.Zero;

                if (this.items.Length < text.Length)
                {
                    Array.Resize(ref this.items, text.Length);
                }
                do
                {
                    var characterInfo = new TerminalCharacterInfo();
                    var character = text[index];
                    var charInfo = font[character];
                    var volume = FontUtility.GetCharacterVolume(font, character);
                    if (point.X + volume > columnCount)
                    {
                        point.X = 0;
                        point.Y++;
                    }
                    characterInfo.Character = character;
                    characterInfo.Volume = volume;
                    characterInfo.Point = point;
                    characterInfo.BackgroundColor = this.grid.IndexToBackgroundColor(index);
                    characterInfo.ForegroundColor = this.grid.IndexToForegroundColor(index);
                    characterInfo.Texture = charInfo.Texture;
                    characterInfo.TextIndex = index;
                    point.X += volume;
                    if (point.X >= columnCount || character == '\n')
                    {
                        point.X = 0;
                        point.Y++;
                    }
                    this.items[index] = characterInfo;
                } while (++index < text.Length);
                this.lt.Y = 0;
                this.rb.Y = point.Y + 1;
            }
            this.font = font;
            this.text = text;
            this.columnCount = columnCount;
        }

        public int PointToIndex(TerminalPoint point)
        {
            for (var i = 0; i < this.Count; i++)
            {
                var item = this.items[i];
                if (point == item.Point)
                    return i;
            }
            return -1;
        }

        public static int GetIndex(string text1, string text2)
        {
            var oldValue = text1 ?? throw new ArgumentNullException(nameof(text1));
            var newValue = text2 ?? throw new ArgumentNullException(nameof(text2));
            var minLength = Math.Min(newValue.Length, oldValue.Length);
            var index = minLength;

            for (var i = 0; i < minLength; i++)
            {
                if (newValue[i] != oldValue[i])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public int Count => this.font != null ? this.text.Length : 0;

        public TerminalCharacterInfo this[int index]
        {
            get
            {
                if (index < 0 || index > this.Count + 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return this.items[index];
            }
        }

        public (int Left, int Top, int Right, int Bottom) Volume => (this.lt.X, this.lt.Y, this.rb.X, this.rb.Y);

        private int FindUpdateIndex(TerminalFont font, string text, int columnCount, int rowCount)
        {
            if (this.font != font || this.columnCount != columnCount || this.rowCount != rowCount)
                return 0;
            var index = GetIndex(this.text, text);
            if (index >= this.items.Length)
                return 0;
            return index;
        }

        #region IEnumerable

        IEnumerator<TerminalCharacterInfo> IEnumerable<TerminalCharacterInfo>.GetEnumerator()
        {
            if (this.font != null)
            {
                for (var i = 0; i < this.text.Length; i++)
                {
                    yield return this.items[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.font != null)
            {
                for (var i = 0; i < this.text.Length; i++)
                {
                    yield return this.items[i];
                }
            }
        }

        #endregion
    }
}
