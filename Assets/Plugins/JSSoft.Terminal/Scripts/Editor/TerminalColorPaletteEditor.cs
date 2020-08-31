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

using UnityEditor;

namespace JSSoft.Terminal.Editor
{
    [CustomEditor(typeof(TerminalColorPalette))]
    public class TerminalColorPaletteEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyFieldAll();
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalColorPalette.Black));
            this.notifier.Add(nameof(TerminalColorPalette.DarkBlue));
            this.notifier.Add(nameof(TerminalColorPalette.DarkGreen));
            this.notifier.Add(nameof(TerminalColorPalette.DarkCyan));
            this.notifier.Add(nameof(TerminalColorPalette.DarkRed));
            this.notifier.Add(nameof(TerminalColorPalette.DarkMagenta));
            this.notifier.Add(nameof(TerminalColorPalette.DarkYellow));
            this.notifier.Add(nameof(TerminalColorPalette.Gray));
            this.notifier.Add(nameof(TerminalColorPalette.DarkGray));
            this.notifier.Add(nameof(TerminalColorPalette.Blue));
            this.notifier.Add(nameof(TerminalColorPalette.Green));
            this.notifier.Add(nameof(TerminalColorPalette.Cyan));
            this.notifier.Add(nameof(TerminalColorPalette.Red));
            this.notifier.Add(nameof(TerminalColorPalette.Magenta));
            this.notifier.Add(nameof(TerminalColorPalette.Yellow));
            this.notifier.Add(nameof(TerminalColorPalette.White));
        }

        protected virtual void OnDisable()
        {
            this.notifier = null;
        }
    }
}