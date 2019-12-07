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
    public static class TerminalFontEvents
    {
        private static readonly HashSet<TerminalFont> fonts = new HashSet<TerminalFont>();

        public static void Register(TerminalFont font)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (fonts.Contains(font) == true)
                throw new ArgumentException($"{nameof(font)} is already exists.");
            fonts.Add(font);
            font.Validated += Font_Validated;
            font.PropertyChanged += Font_PropertyChanged;
        }

        public static void Unregister(TerminalFont font)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (fonts.Contains(font) == false)
                throw new ArgumentException($"{nameof(font)} does not exists.");
            font.Validated -= Font_Validated;
            font.PropertyChanged -= Font_PropertyChanged;
            fonts.Remove(font);
        }

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Font_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }
        
        private static void Font_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
