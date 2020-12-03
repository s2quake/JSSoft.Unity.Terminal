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
    [CustomEditor(typeof(TerminalColorPalette))]
    public class TerminalColorPaletteEditor : UnityEditor.Editor
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
            this.notifier.Add(nameof(TerminalColorPalette.Black));
            this.notifier.Add(nameof(TerminalColorPalette.Red));
            this.notifier.Add(nameof(TerminalColorPalette.Green));
            this.notifier.Add(nameof(TerminalColorPalette.Yellow));
            this.notifier.Add(nameof(TerminalColorPalette.Blue));
            this.notifier.Add(nameof(TerminalColorPalette.Magenta));
            this.notifier.Add(nameof(TerminalColorPalette.Cyan));
            this.notifier.Add(nameof(TerminalColorPalette.White));
            this.notifier.Add(nameof(TerminalColorPalette.BrightBlack));
            this.notifier.Add(nameof(TerminalColorPalette.BrightRed));
            this.notifier.Add(nameof(TerminalColorPalette.BrightGreen));
            this.notifier.Add(nameof(TerminalColorPalette.BrightYellow));
            this.notifier.Add(nameof(TerminalColorPalette.BrightBlue));
            this.notifier.Add(nameof(TerminalColorPalette.BrightMagenta));
            this.notifier.Add(nameof(TerminalColorPalette.BrightCyan));
            this.notifier.Add(nameof(TerminalColorPalette.BrightWhite));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
