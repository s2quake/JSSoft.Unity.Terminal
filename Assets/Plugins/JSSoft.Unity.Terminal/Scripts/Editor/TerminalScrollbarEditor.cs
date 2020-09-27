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
    [CustomEditor(typeof(TerminalScrollbar))]
    class TerminalScrollbarEditor : ScrollbarEditor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.notifier.Begin();
            this.notifier.PropertyField(nameof(TerminalScrollbar.Grid));
            this.notifier.PropertyField(nameof(TerminalScrollbar.VisibleTime));
            this.notifier.End();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalScrollbar.Grid));
            this.notifier.Add(nameof(TerminalScrollbar.VisibleTime));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.notifier = null;
        }
    }
}
