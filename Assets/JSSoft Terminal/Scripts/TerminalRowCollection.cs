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
using System.Linq;
using System.Collections.Generic;

namespace JSSoft.UI
{
    class TerminalRowCollection : List<TerminalRow>
    {
        private readonly TerminalGrid grid;
        private readonly Terminal terminal;
        private readonly Stack<TerminalRow> pool = new Stack<TerminalRow>();
        private TerminalFont font;
        private string text = string.Empty;
        private int bufferWidth;
        private int bufferHeight;
        private int updateIndex;

        public TerminalRowCollection(TerminalGrid grid)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.grid.Enabled += Grid_Enabled;
            this.grid.Disabled += Grid_Disabled;
            this.grid.FontChanged += Grid_FontChanged;
            this.grid.LayoutChanged += Grid_LayoutChanged;
            this.grid.TextChanged += Grid_TextChanged;
            this.grid.Validated += Grid_Validated;
        }

        public void Udpate(TerminalCharacterInfoCollection characterInfos)
        {
            var font = this.grid.Font;
            var text = this.grid.Text + char.MinValue;
            var bufferWidth = this.grid.BufferWidth;
            var bufferHeight = this.grid.BufferHeight;
            if (this.updateIndex < text.Length)
            {
                var volume = characterInfos.Volume;
                var index = this.updateIndex;
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
            this.updateIndex = text.Length;
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

        private void Grid_Enabled(object sender, EventArgs e)
        {
            TerminalStyleEvents.Validated += Style_Validated;
        }

        private void Grid_Disabled(object sender, EventArgs e)
        {
            TerminalStyleEvents.Validated -= Style_Validated;
        }

        private void Grid_FontChanged(object sender, EventArgs e)
        {
            this.updateIndex = 0;
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            var bufferWidth = this.grid.BufferWidth;
            var bufferHeight = this.grid.BufferHeight;
            if (this.bufferWidth != bufferWidth || this.bufferHeight != bufferHeight)
            {
                this.updateIndex = 0;
            }
            // this.bufferWidth = bufferWidth;
            // this.bufferHeight = bufferHeight;
        }

        private void Grid_TextChanged(object sender, EventArgs e)
        {
            var text = this.grid.Text + char.MinValue;
            this.updateIndex = GetIndex(this.text, text);
            // this.text = text;
        }

        private void Grid_Validated(object sender, EventArgs e)
        {
            this.updateIndex = 0;
        }

        private void Style_Validated(object sender, EventArgs e)
        {
            this.updateIndex = 0;
        }
    }
}
