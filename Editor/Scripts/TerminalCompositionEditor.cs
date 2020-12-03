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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using UnityEditor;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalComposition))]
    public class TerminalCompositionEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalComposition.Text));
            this.notifier.Add(nameof(TerminalComposition.ForegroundColor));
            this.notifier.Add(nameof(TerminalComposition.BackgroundColor));
            this.notifier.Add(nameof(TerminalComposition.ForegroundMargin), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalComposition.BackgroundMargin), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalComposition.ColumnIndex));
            this.notifier.Add(nameof(TerminalComposition.RowIndex));
        }

        public void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyFieldAll();
            this.notifier.End();
        }
    }
}
