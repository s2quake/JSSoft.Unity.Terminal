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
    [CustomEditor(typeof(Terminal))]
    public class TerminalEditor : UnityEditor.Editor
    {
        private SerializedProperty outputTextProperty;
        private SerializedProperty promptProperty;
        private SerializedProperty commandProperty;
        private SerializedProperty isReadOnlyProperty;
        private SerializedProperty isVerboseProperty;
        private SerializedProperty dispatcherProperty;

        public void OnEnable()
        {
            this.outputTextProperty = this.serializedObject.FindProperty("outputText");
            this.promptProperty = this.serializedObject.FindProperty("prompt");
            this.commandProperty = this.serializedObject.FindProperty("command");
            this.isReadOnlyProperty = this.serializedObject.FindProperty("isReadOnly");
            this.isVerboseProperty = this.serializedObject.FindProperty("isVerbose");
            this.dispatcherProperty = this.serializedObject.FindProperty("dispatcher");
        }

        public override void OnInspectorGUI()
        {
            if (this.target is Terminal terminal)
            {
                var grid = terminal.GetComponent<TerminalGrid>();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.outputTextProperty);
                EditorGUILayout.PropertyField(this.promptProperty);
                EditorGUILayout.PropertyField(this.commandProperty);
                this.serializedObject.ApplyModifiedProperties();
                if (EditorGUI.EndChangeCheck())
                {
                    // grid.SetText(terminal.Text);
                }
                EditorGUILayout.PropertyField(this.isReadOnlyProperty);
                EditorGUILayout.PropertyField(this.isVerboseProperty);
                EditorGUILayout.PropertyField(this.dispatcherProperty);

                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}