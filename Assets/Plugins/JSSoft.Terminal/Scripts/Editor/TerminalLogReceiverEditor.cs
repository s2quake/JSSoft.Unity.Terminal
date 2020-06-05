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

using System;
using System.Collections.Generic;
using JSSoft.Terminal;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;

namespace JSSoft.Communication.Services.Editor
{
    [CustomEditor(typeof(TerminalLogReceiver))]
    public class TerminalLogReceiverEditor : UnityEditor.Editor
    {
        private static readonly string[] colors;
        private SerializedProperty logTypeProperty;
        private SerializedProperty useForegroundColorProperty;
        private SerializedProperty useBackgroundColorProperty;
        private SerializedProperty foregroundColorProperty;
        private SerializedProperty backgroundColorProperty;

        static TerminalLogReceiverEditor()
        {
            var colorList = new List<string>();
            foreach (var item in Enum.GetNames(typeof(TerminalColor)))
            {
                colorList.Add(item);
            }
            colorList.Add("None");
            colors = colorList.ToArray();
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.logTypeProperty);
            EditorGUILayout.PropertyField(this.useForegroundColorProperty);
            if (this.useForegroundColorProperty.boolValue == true)
                EditorGUILayout.PropertyField(this.foregroundColorProperty);
            EditorGUILayout.PropertyField(this.useBackgroundColorProperty);
            if (this.useBackgroundColorProperty.boolValue == true)
                EditorGUILayout.PropertyField(this.backgroundColorProperty);
            this.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnEnable()
        {
            this.logTypeProperty = this.serializedObject.FindProperty("logType");
            this.useForegroundColorProperty = this.serializedObject.FindProperty("useForegroundColor");
            this.useBackgroundColorProperty = this.serializedObject.FindProperty("useBackgroundColor");
            this.foregroundColorProperty = this.serializedObject.FindProperty("foregroundColor");
            this.backgroundColorProperty = this.serializedObject.FindProperty("backgroundColor");
        }
    }
}
