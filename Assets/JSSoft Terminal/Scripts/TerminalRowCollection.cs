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

namespace JSSoft.UI
{
    class TerminalRowCollection : List<TerminalRow>
    {
        private readonly TerminalGrid grid;
        private readonly Stack<TerminalRow> pool = new Stack<TerminalRow>();
        private TerminalFont font;
        private string text = string.Empty;
        private int bufferWidth;
        private int bufferHeight;

        public TerminalRowCollection(TerminalGrid grid)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public void Udpate(TerminalCharacterInfoCollection characterInfos)
        {
            var font = this.grid.Font;
            var text = this.grid.Text + char.MinValue;
            var bufferWidth = this.grid.BufferWidth;
            var bufferHeight = this.grid.BufferHeight;
            if (this.text != text || this.bufferWidth != bufferWidth || this.bufferHeight != bufferHeight)
            {
                var volume = characterInfos.Volume;
                var index = this.FindUpdateIndex(font, text, bufferWidth, bufferHeight);
                var dic = new Dictionary<int, int>(this.Count);
                this.Resize(bufferWidth, volume.Bottom);
                for (var i = index; i < text.Length; i++)
                {
                    var characterInfo = characterInfos[i];
                    var point = characterInfo.Point;
                    var row = this.Prepare(point.Y);
                    var cell = row.Cells[point.X];
                    cell.SetCharacter(characterInfo);
                    dic[point.Y] = point.X;
                }
                foreach (var item in dic)
                {
                    var row = this[item.Key];
                    row.ResetAfter(item.Value + 1);
                    row.Update();
                }
                for (var i = this.Count; i < this.grid.BufferHeight; i++)
                {
                    var row = this.Prepare(i);
                    row.Reset();
                }
            }

            this.font = font;
            this.text = text;
            this.bufferWidth = bufferWidth;
            this.bufferHeight = bufferHeight;
        }

        public TerminalRow Prepare(int index)
        {
            if (index >= this.Count)
            {
                var row = this.pool.Any() == true ? this.pool.Pop() : new TerminalRow(this.grid, this.Count);
                this.Add(row);
            }

            return this[index];
        }

        public void SetDirty()
        {
            this.font = null;
        }

        private void Resize(int bufferWidth, int bufferHeight)
        {
            for (var i = this.Count - 1; i >= bufferHeight; i--)
            {
                this.pool.Push(this[i]);
                this.RemoveAt(i);
            }
            for (var i = this.Count; i < bufferHeight; i++)
            {
                var item = this.pool.Any() ? this.pool.Pop() : new TerminalRow(this.grid, i);
                item.Reset();
                this.Add(item);
            }
            for (var i = this.Count - 1; i >= 0; i--)
            {
                var row = this[i];
                row.Resize(bufferWidth);
            }
        }

        private int FindUpdateIndex(TerminalFont font, string text, int bufferWidth, int bufferHeight)
        {
            if (this.font != font || this.bufferWidth != bufferWidth || this.bufferHeight != bufferHeight)
                return 0;
            return GetIndex(this.text, text);
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
    }
}
