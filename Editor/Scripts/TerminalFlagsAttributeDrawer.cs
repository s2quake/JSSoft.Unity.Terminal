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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomPropertyDrawer(typeof(TerminalFlagsAttribute))]
    class TerminalFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (this.attribute is TerminalFlagsAttribute attr)
            {
                var enumType = attr.Type;
                var itemList = new List<string>(property.enumNames.Length);
                for (var i = 0; i < 32; i++)
                {
                    var text = Enum.GetName(enumType, 1 << i);
                    if (property.enumNames.Contains(text) == true)
                    {
                        itemList.Add(text);
                    }
                }
                property.intValue = EditorGUI.MaskField(position, label, property.intValue, itemList.ToArray());
            }
            else
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}
