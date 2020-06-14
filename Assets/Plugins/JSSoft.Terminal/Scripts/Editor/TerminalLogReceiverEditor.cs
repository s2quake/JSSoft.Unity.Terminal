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
using JSSoft.Terminal;
using JSSoft.Terminal.Editor;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;

namespace JSSoft.Communication.Services.Editor
{
    [CustomEditor(typeof(TerminalLogReceiver))]
    public class TerminalLogReceiverEditor : UnityEditor.Editor
    {
        private static readonly string[] colors;

        private EditorPropertyNotifier notifier;

        static TerminalLogReceiverEditor()
        {
            var colorList = new List<string>();
            foreach (var item in Enum.GetNames(typeof(TerminalColor)))
            {
                colorList.Add(item);
            }
            colorList.Add("None");
            colors = colorList.ToArray();
        }

        public override void OnInspectorGUI()
        {
            var useForegroundColorProperty = this.notifier.GetProperty(nameof(TerminalLogReceiver.UseForegroundColor));
            var useBackgroundColorProperty = this.notifier.GetProperty(nameof(TerminalLogReceiver.UseBackgroundColor));
            this.notifier.Begin();
            this.notifier.PropertyField(nameof(TerminalLogReceiver.LogType));
            this.notifier.PropertyField(nameof(TerminalLogReceiver.UseForegroundColor));
            if (useForegroundColorProperty.boolValue == true)
                this.notifier.PropertyField(nameof(TerminalLogReceiver.ForegroundColor));
            this.notifier.PropertyField(nameof(TerminalLogReceiver.UseBackgroundColor));
            if (useBackgroundColorProperty.boolValue == true)
                this.notifier.PropertyField(nameof(TerminalLogReceiver.BackgroundColor));
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalLogReceiver.LogType));
            this.notifier.Add(nameof(TerminalLogReceiver.UseForegroundColor));
            this.notifier.Add(nameof(TerminalLogReceiver.UseBackgroundColor));
            this.notifier.Add(nameof(TerminalLogReceiver.ForegroundColor));
            this.notifier.Add(nameof(TerminalLogReceiver.BackgroundColor));
        }

        protected virtual void OnDisable()
        {
            this.notifier = null;
        }
    }
}
