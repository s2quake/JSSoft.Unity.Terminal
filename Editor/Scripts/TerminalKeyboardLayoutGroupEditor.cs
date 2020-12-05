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

using UnityEditor;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalKeyboardLayoutGroup))]
    public class TerminalKeyboardLayoutGroupEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyField(nameof(TerminalKeyboardLayoutGroup.TerminalLayout));
            this.notifier.PropertyField(nameof(TerminalKeyboardLayoutGroup.KeyboardLayout));
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalKeyboardLayoutGroup.TerminalLayout));
            this.notifier.Add(nameof(TerminalKeyboardLayoutGroup.KeyboardLayout));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
