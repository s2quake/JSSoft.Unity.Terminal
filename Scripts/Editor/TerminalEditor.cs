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

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(Terminal))]
    public class TerminalEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyFieldAll();
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(Terminal.OutputText));
            this.notifier.Add(nameof(Terminal.Prompt));
            this.notifier.Add(nameof(Terminal.Command));
            this.notifier.Add(nameof(Terminal.IsReadOnly));
            this.notifier.Add(nameof(Terminal.IsVerbose));
            this.notifier.Add(nameof(Terminal.Dispatcher));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
