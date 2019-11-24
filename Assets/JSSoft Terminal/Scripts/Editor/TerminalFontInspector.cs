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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using JSSoft.UI.Fonts;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace JSSoft.UI.Editor
{
    [CustomEditor(typeof(TerminalFont))]
    public class TerminalFontInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            
            var fontsProperty = this.serializedObject.FindProperty("fonts");
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(fontsProperty);
            for (int i = 0; i < fontsProperty.arraySize; i++)
            {
                EditorGUILayout.PropertyField(fontsProperty.GetArrayElementAtIndex(i));
            }

            // EditorGUILayout.PropertyField(fontsProperty);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("height"));
            // EditorGUILayout.PropertyField(this.serializedObject.FindProperty("objects"));

        }
    }
}
