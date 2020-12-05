////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using JSSoft.Unity.Terminal;
using JSSoft.Unity.Terminal.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace JSSoft.Unity.Terminal.Editor
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
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalLogReceiver.LogType));
            this.notifier.PropertyField(nameof(TerminalLogReceiver.UseForegroundColor));
            if (useForegroundColorProperty.boolValue == true)
                this.notifier.PropertyField(nameof(TerminalLogReceiver.ForegroundColor));
            this.notifier.PropertyField(nameof(TerminalLogReceiver.UseBackgroundColor));
            if (useBackgroundColorProperty.boolValue == true)
                this.notifier.PropertyField(nameof(TerminalLogReceiver.BackgroundColor));
            this.notifier.PropertyField(nameof(TerminalLogReceiver.Pattern));
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
            this.notifier.Add(nameof(TerminalLogReceiver.Pattern));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
