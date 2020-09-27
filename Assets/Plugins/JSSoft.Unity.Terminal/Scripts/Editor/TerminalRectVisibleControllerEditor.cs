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

using UnityEditor;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalRectVisibleController))]
    public class TerminalRectVisibleControllerEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalRectVisibleController.Grid));
            this.notifier.PropertyField(nameof(TerminalRectVisibleController.Direction));
            this.notifier.PropertyField(nameof(TerminalRectVisibleController.KeyCode));
            this.notifier.PropertyField(nameof(TerminalRectVisibleController.Modifiers));
            this.notifier.End();
            if (GUILayout.Button("Show") == true)
            {
                if (this.target is TerminalRectVisibleController controller)
                {
                    controller.Show();
                }
            }
            if (GUILayout.Button("Hide") == true)
            {
                if (this.target is TerminalRectVisibleController controller)
                {
                    controller.Hide();
                }
            }
            if (GUILayout.Button("Reset") == true)
            {
                if (this.target is RectVisibleController controller)
                {
                    controller.ResetPosition();
                }
            }
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalRectVisibleController.Grid));
            this.notifier.Add(nameof(TerminalRectVisibleController.Direction));
            this.notifier.Add(nameof(TerminalRectVisibleController.KeyCode));
            this.notifier.Add(nameof(TerminalRectVisibleController.Modifiers));
        }

        protected virtual void OnDisable()
        {
            this.notifier = null;
        }
    }
}
