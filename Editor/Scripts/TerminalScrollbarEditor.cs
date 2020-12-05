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
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
