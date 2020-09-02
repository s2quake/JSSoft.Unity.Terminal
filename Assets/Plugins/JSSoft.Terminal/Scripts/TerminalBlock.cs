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

namespace JSSoft.Terminal
{
    class TerminalBlock
    {
        private TerminalColor?[] foregroundColors = new TerminalColor?[] { };
        private TerminalColor?[] backgroundColors = new TerminalColor?[] { };
        private string text = string.Empty;

        public string Text
        {
            get => this.text;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.Text != value)
                {
                    ArrayUtility.Resize(ref this.foregroundColors, value.Length);
                    ArrayUtility.Resize(ref this.backgroundColors, value.Length);
                    this.text = value;
                }
            }
        }

        public void Highlight(ISyntaxHighlighter highlighter, TerminalTextType textType)
        {
            highlighter.Highlight(textType, this.Text, this.foregroundColors, this.backgroundColors);
        }

        public bool Contains(int index)
        {
            return index >= 0 && index < this.text.Length;
        }

        public TerminalColor? GetForegroundColor(int index)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index < this.foregroundColors.Length)
                return this.foregroundColors[index];
            return null;
        }

        public TerminalColor? GetBackgroundColor(int index)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index < this.backgroundColors.Length)
                return this.backgroundColors[index];
            return null;
        }

        public void SetForegroundColor(int index, TerminalColor? color)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.foregroundColors[index] = color;
        }

        public void SetBackgroundColor(int index, TerminalColor? color)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.backgroundColors[index] = color;
        }
    }
}