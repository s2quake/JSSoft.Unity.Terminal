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
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace JSSoft.Terminal.Editor
{
    [CustomEditor(typeof(TerminalGrid))]
    public class TerminalGridEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;
        private bool isDebug = false;

        public override void OnInspectorGUI()
        {
            var styleProperty = this.notifier.GetProperty(nameof(TerminalGrid.Style));
            this.isDebug = GUILayout.Toggle(this.isDebug, "Debug Mode");
            if (isDebug == true)
            {
                base.OnInspectorGUI();
                return;
            }

            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalGrid.Style));
            GUILayout.Space(10);
            if (styleProperty.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("Property cannot be changed when style is applied.", MessageType.Info);
            }
            GUI.enabled = styleProperty.objectReferenceValue == null;
            this.notifier.PropertyField(nameof(TerminalGrid.Font));
            this.notifier.PropertyField(nameof(TerminalGrid.BackgroundColor));
            this.notifier.PropertyField(nameof(TerminalGrid.ForegroundColor));
            this.notifier.PropertyField(nameof(TerminalGrid.SelectionColor));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorColor));
            this.notifier.PropertyField(nameof(TerminalGrid.ColorPalette));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorStyle));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorThickness));
            this.notifier.PropertyField(nameof(TerminalGrid.IsCursorBlinkable));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorBlinkDelay));
            this.notifier.PropertyField(nameof(TerminalGrid.IsScrollForwardEnabled));
            this.notifier.PropertyField(nameof(TerminalGrid.BehaviourList));
            GUI.enabled = true;

            GUILayout.Space(10);
            this.notifier.PropertyField(nameof(TerminalGrid.MaxBufferHeight));
            this.notifier.PropertyField(nameof(TerminalGrid.Padding));
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalGrid.Style));
            this.notifier.Add(nameof(TerminalGrid.Font));
            this.notifier.Add(nameof(TerminalGrid.BackgroundColor));
            this.notifier.Add(nameof(TerminalGrid.ForegroundColor));
            this.notifier.Add(nameof(TerminalGrid.SelectionColor));
            this.notifier.Add(nameof(TerminalGrid.CursorColor));
            this.notifier.Add(nameof(TerminalGrid.ColorPalette));
            this.notifier.Add(nameof(TerminalGrid.CursorStyle));
            this.notifier.Add(nameof(TerminalGrid.CursorThickness));
            this.notifier.Add(nameof(TerminalGrid.IsCursorBlinkable));
            this.notifier.Add(nameof(TerminalGrid.CursorBlinkDelay));
            this.notifier.Add(nameof(TerminalGrid.IsScrollForwardEnabled));
            this.notifier.Add(nameof(TerminalGrid.BehaviourList), EditorPropertyUsage.DisallowNotification);
            this.notifier.Add(nameof(TerminalGrid.MaxBufferHeight));
            this.notifier.Add(nameof(TerminalGrid.Padding), EditorPropertyUsage.IncludeChildren);
            this.notifier.PropertyChanged += Notifier_PropertyChanged;
        }

        protected virtual void OnDisable()
        {
            this.notifier = null;
        }

        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TerminalGrid.Padding))
            {
                foreach (var item in this.targets)
                {
                    if (item is TerminalGrid grid)
                    {
                        grid.UpdateLayout();
                    }
                }
            }
        }
    }
}
