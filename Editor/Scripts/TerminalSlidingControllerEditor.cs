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
    [CustomEditor(typeof(TerminalSlidingController))]
    public class TerminalSlidingControllerEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;
        private GUIContent showContent;
        private GUIContent hideContent;
        private GUIContent resetContent;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalSlidingController.Grid));
            this.notifier.PropertyField(nameof(TerminalSlidingController.Direction));
            this.notifier.PropertyField(nameof(TerminalSlidingController.KeyCode));
            this.notifier.PropertyField(nameof(TerminalSlidingController.Modifiers));
            this.notifier.End();

            GUI.enabled = Application.isPlaying == false;
            if (GUILayout.Button(this.showContent) == true)
            {
                if (this.target is TerminalSlidingController controller)
                {
                    controller.Show();
                    EditorUtility.SetDirty(controller);
                }
            }
            if (GUILayout.Button(this.hideContent) == true)
            {
                if (this.target is TerminalSlidingController controller)
                {
                    controller.Hide();
                    EditorUtility.SetDirty(controller);
                }
            }
            if (GUILayout.Button(this.resetContent) == true)
            {
                if (this.target is SlidingController controller)
                {
                    controller.ResetPosition();
                    EditorUtility.SetDirty(controller);
                }
            }
            GUI.enabled = true;
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalSlidingController.Grid));
            this.notifier.Add(nameof(TerminalSlidingController.Direction));
            this.notifier.Add(nameof(TerminalSlidingController.KeyCode));
            this.notifier.Add(nameof(TerminalSlidingController.Modifiers));
            this.showContent = new GUIContent("Show", TerminalStrings.GetString("SlidingController.Show"));
            this.hideContent = new GUIContent("Hide", TerminalStrings.GetString("SlidingController.Hide"));
            this.resetContent = new GUIContent("Reset", TerminalStrings.GetString("SlidingController.Reset"));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
