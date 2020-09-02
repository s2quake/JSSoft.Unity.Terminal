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
using UnityEngine;

namespace JSSoft.Terminal
{
    class TerminalBlock
    {
        private TerminalColor?[] foregroundColors = new TerminalColor?[] { };
        private TerminalColor?[] backgroundColors = new TerminalColor?[] { };

        public int Index { get; set; }

        public int Length { get; set; }

        public void Resize(int capacity)
        {
            ArrayUtility.Resize(ref this.foregroundColors, capacity);
            ArrayUtility.Resize(ref this.backgroundColors, capacity);
        }

        public TerminalColor? GetForegroundColor(int index)
        {
            var localIndex = index - this.Index;
            if (localIndex >= 0 && localIndex < this.Length)
            {
                if (localIndex < this.foregroundColors.Length)
                    return this.foregroundColors[localIndex];
            }
            return null;
        }

        public TerminalColor? GetBackgroundColor(int index)
        {
            var localIndex = index - this.Index;
            if (localIndex >= 0 && localIndex < this.Length)
            {
                if (localIndex < this.backgroundColors.Length)
                    return this.backgroundColors[localIndex];
            }
            return null;
        }
    }
}