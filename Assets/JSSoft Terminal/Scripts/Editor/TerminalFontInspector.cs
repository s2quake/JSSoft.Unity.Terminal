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
    [CustomEditor(typeof(TerminalFont))]
    public class TerminalFontInspector : UnityEditor.Editor
    {
        private SerializedProperty fontProperty;
        private ReorderableList fontList;

        public void OnEnable()
        {
            this.fontProperty = this.serializedObject.FindProperty("fontList");
            // Debug.Log(this.fontProperty);
            this.fontList = new ReorderableList(this.serializedObject, this.fontProperty, true, true, true, true);
            this.fontList.drawElementBackgroundCallback = drawElementCallback;
            // this.fontList.drawHeaderCallback = item => EditorGUI.LabelField(item, "Fallback List");
        }

        private void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= 0 && index < this.fontProperty.arraySize)
            {
                var element = this.fontProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            }
            // var element = this.fontProperty.GetArrayElementAtIndex(index);
            // rect.y += 2;
            // EditorGUI.PropertyField(
            //     new Rect(rect.x+60, rect.y, rect.width -60, EditorGUIUtility.singleLineHeight),
            //     element, GUIContent.none);
            // EditorGUI.PropertyField(
            //     new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
            //     element.FindPropertyRelative("Prefab"), GUIContent.none);
            // EditorGUI.PropertyField(
            //     new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
            //     element.FindPropertyRelative("Count"), GUIContent.none);
        }



        public override void OnInspectorGUI()
        {

            this.serializedObject.Update();

            // EditorGUILayout.PropertyField(fontsProperty);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("height"));
            this.fontList.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();
            // EditorGUILayout.PropertyField(this.serializedObject.FindProperty("objects"));

        }
    }
}
