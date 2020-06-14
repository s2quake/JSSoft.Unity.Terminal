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
    [CustomEditor(typeof(TerminalFontDescriptor))]
    public class TerminalFontDescriptorEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;
        private Vector2 scrollPos;

        public override void OnInspectorGUI()
        {
            // this.notifier.Begin();
            // this.notifier.PropertyFieldAll();
            // this.notifier.End();
            Debug.Log(EditorGUIUtility.singleLineHeight);
            GUILayoutUtility.GetLastRect();
            

            return;
            var obj = this.serializedObject;
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();

            // EditorGUILayout.GetControlRect()
            // Loop through properties and create one field (including children) for each top level property.
            SerializedProperty property = obj.GetIterator();
            bool expanded = true;
            int i = 0;
            while (property.NextVisible(expanded))
            {
                using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
                {
                    EditorGUILayout.PropertyField(property, true, GUILayout.MaxHeight(100));
                }
                expanded = false;
                i++;
                // if (i > 1)
                //     break;
            }

            obj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this, this.InvokeEvent);
            this.notifier.Add(nameof(TerminalFontDescriptor.BaseInfo), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFontDescriptor.CommonInfo), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFontDescriptor.CharInfos), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFontDescriptor.Textures), EditorPropertyUsage.IncludeChildren);
        }

        protected virtual void OnDisable()
        {
            this.notifier = null;
        }

        private void InvokeEvent(string[] propertyNames)
        {
            if (this.target is TerminalComposition composition)
            {
                foreach (var item in propertyNames)
                {
                    composition.InvokePropertyChangedEvent(item);
                }
            }
        }
    }
}
