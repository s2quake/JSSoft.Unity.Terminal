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
    [CustomEditor(typeof(TerminalFont))]
    public class TerminalFontEditor : UnityEditor.Editor
    {
        private SerializedProperty fontProperty;
        private ReorderableList fontList;

        public void OnEnable()
        {
            this.fontProperty = this.serializedObject.FindProperty("descriptorList");
            this.fontList = new ReorderableList(this.serializedObject, this.fontProperty)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = item => EditorGUI.LabelField(item, "Font Descriptors"),
                drawElementCallback = drawElementCallback,
            };
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.fontList.DoLayoutList();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("height"));
            this.serializedObject.ApplyModifiedProperties();
        }

        private void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = this.fontProperty.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            if (index == 0)
                EditorGUI.LabelField(rect, "Main Font: ", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            else
                EditorGUI.LabelField(rect, $"Sub Font {index - 1}: ");
            rect.x += 70;
            rect.width -= 70;
            EditorGUI.PropertyField(rect, element);
        }
    }
}
