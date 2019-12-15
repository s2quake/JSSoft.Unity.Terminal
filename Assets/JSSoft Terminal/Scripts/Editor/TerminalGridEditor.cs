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

namespace JSSoft.UI.Editor
{
    [CustomEditor(typeof(TerminalGrid))]
    public class TerminalGridEditor : UnityEditor.Editor
    {
        private SerializedProperty fontProperty;
        private SerializedProperty styleProperty;
        private SerializedProperty behaviourProperty;

        private SerializedProperty backgroundColorProperty;
        private SerializedProperty foregroundColorProperty;
        private SerializedProperty selectionColorProperty;
        private SerializedProperty cursorColorProperty;
        private SerializedProperty compositionColorProperty;
        private SerializedProperty colorPaletteProperty;
        private SerializedProperty paddingProperty;
        private SerializedProperty cursorStyleProperty;
        private SerializedProperty cursorThicknessProperty;
        private SerializedProperty isCursorBlinkableProperty;
        private SerializedProperty cursorBlinkDelayProperty;

        private SerializedProperty bufferWidthProperty;
        private SerializedProperty bufferHeightProperty;

        private bool isDebug = false;

        public void OnEnable()
        {
            this.fontProperty = this.serializedObject.FindProperty("font");
            this.styleProperty = this.serializedObject.FindProperty("style");
            this.behaviourProperty = this.serializedObject.FindProperty("behaviour");

            this.backgroundColorProperty = this.serializedObject.FindProperty("backgroundColor");
            this.foregroundColorProperty = this.serializedObject.FindProperty("foregroundColor");
            this.selectionColorProperty = this.serializedObject.FindProperty("selectionColor");
            this.cursorColorProperty = this.serializedObject.FindProperty("cursorColor");
            this.compositionColorProperty = this.serializedObject.FindProperty("compositionColor");
            this.colorPaletteProperty = this.serializedObject.FindProperty("colorPalette");
            this.paddingProperty = this.serializedObject.FindProperty("padding");
            this.cursorStyleProperty = this.serializedObject.FindProperty("cursorStyle");
            this.cursorThicknessProperty = this.serializedObject.FindProperty("cursorThickness");
            this.isCursorBlinkableProperty = this.serializedObject.FindProperty("isCursorBlinkable");
            this.cursorBlinkDelayProperty = this.serializedObject.FindProperty("cursorBlinkDelay");

            this.bufferWidthProperty = this.serializedObject.FindProperty("bufferWidth");
            this.bufferHeightProperty = this.serializedObject.FindProperty("bufferHeight");
        }

        public override void OnInspectorGUI()
        {
            this.isDebug = GUILayout.Toggle(this.isDebug, "Debug Mode");
            if (isDebug == true)
            {
                base.OnInspectorGUI();
                return;
            }

            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.fontProperty);
            EditorGUILayout.PropertyField(this.styleProperty);
            EditorGUILayout.PropertyField(this.behaviourProperty);
            GUILayout.Space(10);

            if (this.styleProperty.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("Property cannot be changed when style is applied.", MessageType.Info);
            }
            GUI.enabled = this.styleProperty.objectReferenceValue == null;
            EditorGUILayout.PropertyField(this.backgroundColorProperty);
            EditorGUILayout.PropertyField(this.foregroundColorProperty);
            EditorGUILayout.PropertyField(this.selectionColorProperty);
            EditorGUILayout.PropertyField(this.cursorColorProperty);
            EditorGUILayout.PropertyField(this.compositionColorProperty);
            EditorGUILayout.PropertyField(this.colorPaletteProperty);
            EditorGUILayout.PropertyField(this.paddingProperty, true);
            EditorGUILayout.PropertyField(this.cursorStyleProperty);
            EditorGUILayout.PropertyField(this.cursorThicknessProperty);
            EditorGUILayout.PropertyField(this.isCursorBlinkableProperty);
            EditorGUILayout.PropertyField(this.cursorBlinkDelayProperty);
            GUI.enabled = true;

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(this.bufferWidthProperty);
            EditorGUILayout.PropertyField(this.bufferHeightProperty);

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
