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
    [CustomEditor(typeof(TerminalFontDescriptor))]
    public class TerminalFontDescriptorEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalFontDescriptor.BaseInfo));
            this.notifier.PropertyField(nameof(TerminalFontDescriptor.CommonInfo));
            this.notifier.PropertyField(nameof(TerminalFontDescriptor.Textures));
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalFontDescriptor.BaseInfo), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFontDescriptor.CommonInfo), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFontDescriptor.CharInfos), EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalFontDescriptor.Textures), EditorPropertyUsage.IncludeChildren);
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
