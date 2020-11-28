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
using UnityEditor.UI;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalDockController))]
    class TerminalDockControllerEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            if (this.serializedObject.targetObject is TerminalDockController controller)
            {
                this.notifier.Begin();
                this.notifier.PropertyField(nameof(TerminalDockController.Dock));
                this.notifier.PropertyField(nameof(TerminalDockController.IsRatio));
                if (controller.IsRatio == true)
                    this.notifier.PropertyField(nameof(TerminalDockController.Ratio));
                else
                    this.notifier.PropertyField(nameof(TerminalDockController.Length));
                this.notifier.End();
                if (this.notifier.IsModified == true)
                {
                    controller.UpdateLayout();
                }
            }
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalDockController.Dock));
            this.notifier.Add(nameof(TerminalDockController.IsRatio));
            this.notifier.Add(nameof(TerminalDockController.Length));
            this.notifier.Add(nameof(TerminalDockController.Ratio));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
