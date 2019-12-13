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
using System.ComponentModel;

namespace JSSoft.UI
{
    public static class TerminalColorPaletteEvents
    {
        private static readonly HashSet<TerminalColorPalette> palettes = new HashSet<TerminalColorPalette>();

        public static void Register(TerminalColorPalette palette)
        {
            if (palette == null)
                throw new ArgumentNullException(nameof(palette));
            if (palettes.Contains(palette) == true)
                throw new ArgumentException($"{nameof(palette)} is already exists.");
            palettes.Add(palette);
            palette.Validated += Palette_Validated;
            palette.PropertyChanged += Palette_PropertyChanged;
        }

        public static void Unregister(TerminalColorPalette paletter)
        {
            if (paletter == null)
                throw new ArgumentNullException(nameof(paletter));
            if (palettes.Contains(paletter) == false)
                throw new ArgumentException($"{nameof(paletter)} does not exists.");
            paletter.Validated -= Palette_Validated;
            paletter.PropertyChanged -= Palette_PropertyChanged;
            palettes.Remove(paletter);
        }

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Palette_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Palette_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
