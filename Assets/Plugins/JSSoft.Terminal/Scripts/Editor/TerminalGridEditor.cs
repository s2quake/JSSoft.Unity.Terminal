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
        private SerializedProperty styleProperty;

        private SerializedProperty fontProperty;
        private SerializedProperty backgroundColorProperty;
        private SerializedProperty foregroundColorProperty;
        private SerializedProperty selectionColorProperty;
        private SerializedProperty cursorColorProperty;
        private SerializedProperty colorPaletteProperty;
        private SerializedProperty paddingProperty;
        private SerializedProperty cursorStyleProperty;
        private SerializedProperty cursorThicknessProperty;
        private SerializedProperty isCursorBlinkableProperty;
        private SerializedProperty cursorBlinkDelayProperty;
        private SerializedProperty isScrollForwardEnabledProperty;
        private SerializedProperty behaviourListProperty;

        private SerializedProperty bufferWidthProperty;
        private SerializedProperty bufferHeightProperty;
        private SerializedProperty maxBufferHeightProperty;
        private SerializedProperty horizontalAlignmentProperty;
        private SerializedProperty verticalAlignmentProperty;
        private SerializedProperty marginProperty;

        private bool isDebug = false;

        public void OnEnable()
        {
            this.styleProperty = this.serializedObject.FindProperty("style");

            this.fontProperty = this.serializedObject.FindProperty("font");
            this.backgroundColorProperty = this.serializedObject.FindProperty("backgroundColor");
            this.foregroundColorProperty = this.serializedObject.FindProperty("foregroundColor");
            this.selectionColorProperty = this.serializedObject.FindProperty("selectionColor");
            this.cursorColorProperty = this.serializedObject.FindProperty("cursorColor");
            this.colorPaletteProperty = this.serializedObject.FindProperty("colorPalette");
            this.paddingProperty = this.serializedObject.FindProperty("padding");
            this.cursorStyleProperty = this.serializedObject.FindProperty("cursorStyle");
            this.cursorThicknessProperty = this.serializedObject.FindProperty("cursorThickness");
            this.isCursorBlinkableProperty = this.serializedObject.FindProperty("isCursorBlinkable");
            this.cursorBlinkDelayProperty = this.serializedObject.FindProperty("cursorBlinkDelay");
            this.isScrollForwardEnabledProperty = this.serializedObject.FindProperty("isScrollForwardEnabled");
            this.behaviourListProperty = this.serializedObject.FindProperty("behaviourList");

            this.bufferWidthProperty = this.serializedObject.FindProperty("bufferWidth");
            this.bufferHeightProperty = this.serializedObject.FindProperty("bufferHeight");
            this.maxBufferHeightProperty = this.serializedObject.FindProperty("maxBufferHeight");
            this.horizontalAlignmentProperty = this.serializedObject.FindProperty("horizontalAlignment");
            this.verticalAlignmentProperty = this.serializedObject.FindProperty("verticalAlignment");
            this.marginProperty = this.serializedObject.FindProperty("margin");
        }

        public override void OnInspectorGUI()
        {
            var grid = this.target as TerminalGrid;
            this.isDebug = GUILayout.Toggle(this.isDebug, "Debug Mode");
            if (isDebug == true)
            {
                base.OnInspectorGUI();
                return;
            }

            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.styleProperty);
            GUILayout.Space(10);

            if (this.styleProperty.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("Property cannot be changed when style is applied.", MessageType.Info);
            }
            GUI.enabled = this.styleProperty.objectReferenceValue == null;
            EditorGUILayout.PropertyField(this.fontProperty);
            EditorGUILayout.PropertyField(this.backgroundColorProperty);
            EditorGUILayout.PropertyField(this.foregroundColorProperty);
            EditorGUILayout.PropertyField(this.selectionColorProperty);
            EditorGUILayout.PropertyField(this.cursorColorProperty);
            EditorGUILayout.PropertyField(this.colorPaletteProperty);
            EditorGUILayout.PropertyField(this.paddingProperty, true);
            EditorGUILayout.PropertyField(this.cursorStyleProperty);
            EditorGUILayout.PropertyField(this.cursorThicknessProperty);
            EditorGUILayout.PropertyField(this.isCursorBlinkableProperty);
            EditorGUILayout.PropertyField(this.cursorBlinkDelayProperty);
            EditorGUILayout.PropertyField(this.isScrollForwardEnabledProperty);
            EditorGUILayout.PropertyField(this.behaviourListProperty, true);
            GUI.enabled = true;

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(this.bufferWidthProperty);
            EditorGUILayout.PropertyField(this.bufferHeightProperty);
            EditorGUILayout.PropertyField(this.maxBufferHeightProperty);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.horizontalAlignmentProperty);
            EditorGUILayout.PropertyField(this.verticalAlignmentProperty);
            EditorGUILayout.PropertyField(this.marginProperty, true);
            this.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                grid?.UpdateLayout();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("expand width"))
            {
                grid.BufferWidth++;
            }
            if (GUILayout.Button("collapse width"))
            {
                grid.BufferWidth--;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("expand height"))
            {
                grid.BufferHeight++;
            }
            if (GUILayout.Button("collapse height"))
            {
                grid.BufferHeight--;
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Test"))
            {
                grid.SetLayoutDirty();
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}