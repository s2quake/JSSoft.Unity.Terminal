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
using UnityEditorInternal;
using UnityEngine;

namespace JSSoft.Terminal.Editor
{
    [CustomEditor(typeof(TerminalComposition))]
    public class TerminalCompositionEditor : UnityEditor.Editor
    {
        private PropertyNotifier notifier;

        public void OnEnable()
        {
            this.notifier = new PropertyNotifier(this.serializedObject, this.InvokeEvent);
            this.notifier.Add(nameof(TerminalComposition.Text));
            this.notifier.Add(nameof(TerminalComposition.ForegroundColor));
            this.notifier.Add(nameof(TerminalComposition.BackgroundColor));
            this.notifier.Add(nameof(TerminalComposition.ForegroundMargin), true);
            this.notifier.Add(nameof(TerminalComposition.BackgroundMargin), true);
            this.notifier.Add(nameof(TerminalComposition.ColumnIndex));
            this.notifier.Add(nameof(TerminalComposition.RowIndex));
        }

        public void OnDisable()
        {
            this.notifier = null;
        }

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyFieldAll();
            this.notifier.End();
        }

        private void InvokeEvent(string[] propertyNames)
        {
            if (this.target is TerminalComposition composition)
            {
                foreach (var item in propertyNames)
                {
                    composition.InvokePropertyChangedEvent(item);
                }
            }
        }
    }
}