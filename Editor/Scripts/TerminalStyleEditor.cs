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
    [CustomEditor(typeof(TerminalStyle))]
    public class TerminalStyleEditor : UnityEditor.Editor
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
            this.notifier.Add(nameof(TerminalStyle.StyleName));
            this.notifier.Add(nameof(TerminalStyle.Font));
            this.notifier.Add(nameof(TerminalStyle.BackgroundColor));
            this.notifier.Add(nameof(TerminalStyle.ForegroundColor));
            this.notifier.Add(nameof(TerminalStyle.SelectionColor));
            this.notifier.Add(nameof(TerminalStyle.SelectionTextColor));
            this.notifier.Add(nameof(TerminalStyle.CursorColor));
            this.notifier.Add(nameof(TerminalStyle.CursorTextColor));
            this.notifier.Add(nameof(TerminalStyle.FallbackTexture));
            this.notifier.Add(nameof(TerminalStyle.ColorPalette));
            this.notifier.Add(nameof(TerminalStyle.CursorStyle));
            this.notifier.Add(nameof(TerminalStyle.CursorThickness));
            this.notifier.Add(nameof(TerminalStyle.IsCursorBlinkable));
            this.notifier.Add(nameof(TerminalStyle.CursorBlinkDelay));
            this.notifier.Add(nameof(TerminalStyle.IsScrollForwardEnabled));
            this.notifier.Add(nameof(TerminalStyle.BehaviourList), EditorPropertyUsage.IncludeChildren);
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
