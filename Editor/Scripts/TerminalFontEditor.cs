﻿// MIT License
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

using UnityEditor;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalFont))]
    public class TerminalFontEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalFont.DescriptorList), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFont.Width));
            this.notifier.Add(nameof(TerminalFont.Height));
            this.notifier.Add(nameof(TerminalFont.Line));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyFieldAll();
            this.notifier.End();
            if (GUILayout.Button("Apply desired size") == true)
            {
                foreach (var item in this.targets)
                {
                    if (item is TerminalFont font)
                    {
                        font.UpdateSize();
                    }
                }
            }
        }
    }
}