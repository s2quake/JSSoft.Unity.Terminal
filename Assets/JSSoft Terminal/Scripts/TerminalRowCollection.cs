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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.UI
{
    class TerminalRowCollection : List<TerminalRow>
    {
        private readonly TerminalGrid grid;
        private readonly TerminalCharacterInfoCollection characterInfos;
        private readonly Terminal terminal;
        private readonly Stack<TerminalRow> pool = new Stack<TerminalRow>();
        private TerminalFont font;
        private TerminalStyle style;
        private string text = string.Empty;
        private int bufferWidth;
        private int bufferHeight;
        private int updateIndex;

        public TerminalRowCollection(TerminalGrid grid, TerminalCharacterInfoCollection characterInfos)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.grid.Enabled += Grid_Enabled;
            this.grid.Disabled += Grid_Disabled;
            this.grid.PropertyChanged += Grid_PropertyChanged;
            this.grid.LayoutChanged += Grid_LayoutChanged;
            this.grid.Validated += Grid_Validated;
            this.characterInfos = characterInfos;
        }

        public void UpdateAll()
        {
            this.Update(0);
        }

        public void Update()
        {
            this.Update(FindIndex());

            int FindIndex()
            {
                if (this.font != this.grid.Font)
                    return 0;
                if (this.style != this.grid.Style)
                    return 0;
                if (this.bufferWidth != grid.BufferWidth)
                    return 0;
                if (this.bufferHeight != grid.BufferHeight)
                    return 0;
                var text = this.grid.Text + char.MinValue;
                return GetIndex(this.text, text);
            }
        }

        public void Update(int index)
        {
            var font = this.grid.Font;
            var style = this.grid.Style;
            var text = this.grid.Text + char.MinValue;
            var bufferWidth = this.grid.BufferWidth;
            var bufferHeight = this.grid.BufferHeight;
            var volume = this.characterInfos.Volume;
            var dic = new Dictionary<int, int>(this.Count);
            this.Resize(bufferWidth, volume.Bottom);
            for (var i = index; i < text.Length; i++)
            {
                var characterInfo = this.characterInfos[i];
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
            this.font = font;
            this.style = style;
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
            this.text = string.Empty;
        }

        private void Grid_Disabled(object sender, EventArgs e)
        {
            TerminalStyleEvents.Validated -= Style_Validated;
            this.text = string.Empty;
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case nameof(ITerminalGrid.Font):
                case nameof(ITerminalGrid.Style):
                case nameof(ITerminalGrid.BufferWidth):
                case nameof(ITerminalGrid.BufferHeight):
                    {
                        Debug.Log(propertyName);
                        this.UpdateAll();
                    }
                    break;
                case nameof(ITerminalGrid.Text):
                    {
                        this.Update();
                    }
                    break;
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            this.UpdateAll();
        }

        private void Grid_Validated(object sender, EventArgs e)
        {
            this.Update();
        }

        private void Style_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalStyle style && this.grid.Style == style)
            {
                this.Update();
            }
        }
    }
}
