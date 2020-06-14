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
    [CustomEditor(typeof(TerminalGrid))]
    public class TerminalGridEditor : UnityEditor.Editor
    {
        private PropertyNotifier notifier;
        private bool isDebug = false;

        public void OnEnable()
        {
            this.notifier = new PropertyNotifier(this.serializedObject, this.InvokeEvent);
            this.notifier.Add("style", nameof(TerminalGrid.Style));
            this.notifier.Add("font", nameof(TerminalGrid.Font));
            this.notifier.Add("backgroundColor", nameof(TerminalGrid.BackgroundColor));
            this.notifier.Add("foregroundColor", nameof(TerminalGrid.ForegroundColor));
            this.notifier.Add("selectionColor", nameof(TerminalGrid.SelectionColor));
            this.notifier.Add("cursorColor", nameof(TerminalGrid.CursorColor));
            this.notifier.Add("colorPalette", nameof(TerminalGrid.ColorPalette));
            this.notifier.Add("cursorStyle", nameof(TerminalGrid.CursorStyle));
            this.notifier.Add("cursorThickness", nameof(TerminalGrid.CursorThickness));
            this.notifier.Add("isCursorBlinkable", nameof(TerminalGrid.IsCursorBlinkable));
            this.notifier.Add("cursorBlinkDelay", nameof(TerminalGrid.CursorBlinkDelay));
            this.notifier.Add("isScrollForwardEnabled", nameof(TerminalGrid.IsScrollForwardEnabled));
            this.notifier.Add("behaviourList", string.Empty);
            this.notifier.Add("maxBufferHeight", nameof(TerminalGrid.MaxBufferHeight));
            this.notifier.Add("padding", nameof(TerminalGrid.Padding), true);
        }

        public void OnDisable()
        {
            this.notifier = null;
        }

        private void InvokeEvent(string[] propertyNames)
        {
            if (this.target is TerminalGrid grid)
            {
                foreach (var item in propertyNames)
                {
                    grid.InvokePropertyChangedEvent(item);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            var styleProperty = this.notifier.GetProperty("style");
            this.isDebug = GUILayout.Toggle(this.isDebug, "Debug Mode");
            if (isDebug == true)
            {
                base.OnInspectorGUI();
                return;
            }

            this.notifier.Begin();
            this.notifier.PropertyField("style");
            GUILayout.Space(10);
            if (styleProperty.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("Property cannot be changed when style is applied.", MessageType.Info);
            }
            GUI.enabled = styleProperty.objectReferenceValue == null;
            this.notifier.PropertyField("font");
            this.notifier.PropertyField("backgroundColor");
            this.notifier.PropertyField("foregroundColor");
            this.notifier.PropertyField("selectionColor");
            this.notifier.PropertyField("cursorColor");
            this.notifier.PropertyField("colorPalette");
            this.notifier.PropertyField("cursorStyle");
            this.notifier.PropertyField("cursorThickness");
            this.notifier.PropertyField("isCursorBlinkable");
            this.notifier.PropertyField("cursorBlinkDelay");
            this.notifier.PropertyField("isScrollForwardEnabled");
            this.notifier.PropertyField("behaviourList");
            GUI.enabled = true;

            GUILayout.Space(10);
            this.notifier.PropertyField("maxBufferHeight");
            this.notifier.PropertyField("padding");
            this.notifier.End();
        }
    }
}
