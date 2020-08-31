// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Terminal
{
    class TerminalCharacterInfoCollection : IReadOnlyList<TerminalCharacterInfo>
    {
        private readonly TerminalGrid grid;
        private TerminalCharacterInfo[] items = new TerminalCharacterInfo[] { };
        private TerminalPoint lt;
        private TerminalPoint rb;
        private TerminalFont font;
        private TerminalStyle style;
        private string text = string.Empty;
        private int bufferWidth;
        private int bufferHeight;
        private int maxBufferHeight;
        private TerminalThickness padding;
        private IReadOnlyList<TerminalFontDescriptor> descriptors;

        public TerminalCharacterInfoCollection(TerminalGrid grid)
        {
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.grid.Enabled += Grid_Enabled;
            this.grid.Disabled += Grid_Disabled;
            this.grid.PropertyChanged += Grid_PropertyChanged;
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
                if (this.maxBufferHeight != grid.MaxBufferHeight)
                    return 0;
                if (this.padding != grid.Padding)
                    return 0;
                var text = this.grid.Text + char.MinValue;
                var index = GetIndex(this.text, text);
                if (index >= this.items.Length)
                    return 0;
                return index;
            }
        }

        public void Update(int index)
        {
            var text = this.grid.Text + char.MinValue;
            if (index >= text.Length)
                return;
            var style = this.grid.Style;
            var font = this.grid.Font;
            var bufferWidth = this.grid.BufferWidth;
            var bufferHeight = this.grid.BufferHeight;
            var maxBufferHeight = this.grid.MaxBufferHeight;
            var padding = this.grid.Padding;
            var point = this.text.Length > 0 ? this.items[index].Point : TerminalPoint.Zero;
            var grid = this.grid;
            ArrayUtility.Resize(ref this.items, text.Length);
            while (index < text.Length)
            {
                var characterInfo = new TerminalCharacterInfo();
                var character = text[index];
                var charInfo = FontUtility.GetCharacter(font, character);
                var volume = FontUtility.GetCharacterVolume(font, character);
                if (point.X >= bufferWidth && character == '\n')
                {
                    volume = 0;
                }
                if ((point.X >= bufferWidth && volume > 0) || (point.X + volume > bufferWidth))
                {
                    point.X = 0;
                    if (index > 0)
                        point.Y++;
                }
                characterInfo.Character = character;
                characterInfo.Volume = volume;
                characterInfo.Point = point;
                characterInfo.BackgroundColor = grid.IndexToBackgroundColor(index);
                characterInfo.ForegroundColor = grid.IndexToForegroundColor(index);
                characterInfo.Texture = charInfo.Texture;
                characterInfo.TextIndex = index;
                this.items[index] = characterInfo;
                index++;
                point.X += volume;
                if (character == '\n')
                {
                    point.X = 0;
                    point.Y++;
                }
            }
            this.lt.Y = 0;
            this.rb.Y = point.Y + 1;
            this.font = font;
            this.style = style;
            this.text = text;
            this.bufferWidth = bufferWidth;
            this.bufferHeight = bufferHeight;
            this.maxBufferHeight = maxBufferHeight;
            this.padding = padding;
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

        public int Count => this.text.Length;

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

        private void Grid_Enabled(object sender, EventArgs e)
        {
            TerminalValidationEvents.Validated += Object_Validated;
            TerminalValidationEvents.Enabled += Object_Enabled;
        }

        private void Grid_Disabled(object sender, EventArgs e)
        {
            TerminalValidationEvents.Validated -= Object_Validated;
            TerminalValidationEvents.Enabled -= Object_Enabled;
            this.text = string.Empty;
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TerminalGrid.Font):
                case nameof(TerminalGrid.Style):
                    {
                        this.UpdateAll();
                    }
                    break;
            }
        }

        private void Object_Validated(object sender, EventArgs e)
        {
            switch (sender)
            {
                case TerminalStyle style when this.grid.Style == style:
                    this.Update();
                    break;
                case TerminalColorPalette palette when this.grid.ColorPalette == palette:
                    this.Update();
                    break;
                case TerminalFont font when this.font == font:
                    this.UpdateAll();
                    break;
            }
        }

        private void Object_Enabled(object sender, EventArgs e)
        {
            switch (sender)
            {
                case TerminalFont font when this.font == font:
                    this.UpdateAll();
                    break;
            }
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
