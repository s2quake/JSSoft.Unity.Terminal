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

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JSSoft.Terminal.Editor
{
    [CustomEditor(typeof(TerminalCursor))]
    public class TerminalCursorEditor : UnityEditor.Editor
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
            this.notifier = new EditorPropertyNotifier(this, this.InvokeEvent);
            this.notifier.Add(nameof(TerminalCursor.CursorLeft));
            this.notifier.Add(nameof(TerminalCursor.CursorTop));
            this.notifier.Add(nameof(TerminalCursor.IsVisible));
            this.notifier.Add(nameof(TerminalCursor.IsFocused), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalCursor.Style), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalCursor.Thickness));
            this.notifier.Add(nameof(TerminalCursor.IsBlinkable));
            this.notifier.Add(nameof(TerminalCursor.BlinkDelay));
        }

        protected virtual void OnDisable()
        {
            this.notifier = null;
        }

        private void InvokeEvent(string[] propertyNames)
        {
            if (this.target is TerminalCursor cursor)
            {
                foreach (var item in propertyNames)
                {
                    cursor.InvokePropertyChangedEvent(item);
                }
            }
        }
    }
}
