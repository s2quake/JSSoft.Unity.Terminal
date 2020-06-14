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
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JSSoft.Terminal.Editor
{
    public class PropertyNotifier
    {
        private readonly SerializedObject serializedObject;
        private readonly Action<string[]> action;
        private readonly Dictionary<string, PropertyInfo> propertyByName = new Dictionary<string, PropertyInfo>();
        private readonly List<PropertyInfo> propertyList = new List<PropertyInfo>();
        private readonly List<string> nameList = new List<string>();

        public PropertyNotifier(SerializedObject serializedObject, Action<string[]> action)
        {
            this.serializedObject = serializedObject;
            this.action = action;
        }

        public void Add(string propertyName)
        {
            this.Add(propertyName, false);
        }

        public void Add(string propertyName, bool includeChildren)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            var targetType = this.serializedObject.targetObject.GetType();
            var property = targetType.GetProperty(propertyName);
            if (property is null)
                throw new ArgumentException($"{targetType} does not have {propertyName} property.", nameof(propertyName));
            if (Attribute.GetCustomAttribute(property, typeof(FieldNameAttribute)) is FieldNameAttribute attribute)
            {
                this.Add(attribute.FieldName, propertyName, includeChildren);
            }
            else
            {
                Debug.LogWarning($"{propertyName} does not have {nameof(FieldNameAttribute)}.");
            }
        }

        public void Add(string fieldName, string propertyName)
        {
            this.Add(fieldName, propertyName, false);
        }

        public void Add(string fieldName, string propertyName, bool includeChildren)
        {
            var property = this.serializedObject.FindProperty(fieldName);
            var propertyInfo = new PropertyInfo()
            {
                Property = property,
                Name = propertyName,
                IncludeChildren = includeChildren,
            };
            this.propertyByName.Add(fieldName, propertyInfo);
            this.propertyList.Add(propertyInfo);
        }

        public void Begin()
        {
            this.nameList.Clear();
        }

        public void End()
        {
            this.action(this.nameList.ToArray());
            this.nameList.Clear();
        }

        public void PropertyFieldAll()
        {
            foreach (var item in this.propertyList)
            {
                this.PropertyField(item);
            }
        }

        public void PropertyField(string fieldName)
        {
            if (this.propertyByName.ContainsKey(fieldName) == false)
                throw new ArgumentException($"{fieldName} does not exists.", nameof(fieldName));
            this.PropertyField(this.propertyByName[fieldName]);
        }

        public SerializedProperty GetProperty(string fieldName)
        {
            var propertyInfo = this.propertyByName[fieldName];
            return propertyInfo.Property;
        }

        private void PropertyField(PropertyInfo propertyInfo)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propertyInfo.Property, propertyInfo.IncludeChildren);
            this.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck() && propertyInfo.Name != string.Empty)
            {
                this.nameList.Add(propertyInfo.Name);
            }
        }
    }
}