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
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JSSoft.Terminal.Editor
{
    public class EditorPropertyNotifier
    {
        private readonly UnityEditor.Editor editor;
        private readonly SerializedObject serializedObject;
        private readonly Action<string[]> action;
        private readonly Dictionary<string, EditorProperty> propertyByName = new Dictionary<string, EditorProperty>();
        private readonly List<EditorProperty> propertyList = new List<EditorProperty>();
        private readonly List<string> nameList = new List<string>();
        private readonly SerializedProperty scriptProperty;

        public EditorPropertyNotifier(UnityEditor.Editor editor)
            : this(editor, (items) => InvokeEvent(editor, items))
        {
        }

        public EditorPropertyNotifier(UnityEditor.Editor editor, Action<string[]> action)
        {
            this.editor = editor ?? throw new ArgumentNullException(nameof(editor));
            this.serializedObject = editor.serializedObject;
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.scriptProperty = this.serializedObject.FindProperty("m_Script");
        }

        public void Add(string propertyName)
        {
            this.Add(propertyName, EditorPropertyUsage.None);
        }

        public void Add(string propertyName, EditorPropertyUsage usage)
        {
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            var targetType = this.serializedObject.targetObject.GetType();
            var property = targetType.GetProperty(propertyName);
            if (property is null)
                throw new ArgumentException($"{targetType} does not have {propertyName} property.", nameof(propertyName));
            if (Attribute.GetCustomAttribute(property, typeof(FieldNameAttribute)) is FieldNameAttribute attribute)
            {
                this.Add(attribute.FieldName, propertyName, usage);
            }
            else
            {
                Debug.LogWarning($"{propertyName} does not have {nameof(FieldNameAttribute)}.");
            }
        }

        public void Add(string fieldName, string propertyName)
        {
            this.Add(fieldName, propertyName, EditorPropertyUsage.None);
        }

        public void Add(string fieldName, string propertyName, EditorPropertyUsage usage)
        {
            if (fieldName is null)
                throw new ArgumentNullException(nameof(fieldName));
            if (fieldName == string.Empty)
                throw new ArgumentException("empty strings are not allowed.", nameof(fieldName));
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (propertyName == string.Empty)
                throw new ArgumentException("empty strings are not allowed.", nameof(propertyName));
            var property = this.serializedObject.FindProperty(fieldName);
            var propertyInfo = new EditorProperty()
            {
                Property = property,
                Name = propertyName,
                Usage = usage,
            };
            this.propertyByName.Add(propertyName, propertyInfo);
            this.propertyList.Add(propertyInfo);
        }

        public void Begin()
        {
            this.nameList.Clear();
            this.serializedObject.Update();

        }

        public void End()
        {
            if (this.nameList.Any())
            {
                this.action(this.nameList.ToArray());
                foreach (var item in this.nameList)
                {
                    this.OnPropertyChanged(new PropertyChangedEventArgs(item));
                }
                this.nameList.Clear();
            }
        }

        public void PropertyScript()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(this.scriptProperty);
            GUI.enabled = true;
        }

        public void PropertyFieldAll()
        {
            this.PropertyScript();
            foreach (var item in this.propertyList)
            {
                this.PropertyField(item);
            }
        }

        public void PropertyField(string propertyName)
        {
            if (this.propertyByName.ContainsKey(propertyName) == false)
                throw new ArgumentException($"{propertyName} does not exists.", nameof(propertyName));
            this.PropertyField(this.propertyByName[propertyName]);
        }

        public SerializedProperty GetProperty(string propertyName)
        {
            var propertyInfo = this.propertyByName[propertyName];
            return propertyInfo.Property;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private void PropertyField(EditorProperty propertyInfo)
        {
            var property = propertyInfo.Property;
            var includeChildren = propertyInfo.IncludeChildren;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, includeChildren);
            this.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck() && propertyInfo.CanNotify == true)
            {
                this.nameList.Add(propertyInfo.Name);
            }
        }

        private static void InvokeEvent(UnityEditor.Editor editor, string[] propertyNames)
        {
            foreach (var target in editor.targets)
            {
                if (target is IPropertyChangedNotifyable obj)
                {
                    foreach (var propertyName in propertyNames)
                    {
                        obj.InvokePropertyChangedEvent(propertyName);
                    }
                }
            }
        }
    }
}
