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
using UnityEditor.UI;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalDockController))]
    class TerminalDockControllerEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;

        public override void OnInspectorGUI()
        {
            if (this.serializedObject.targetObject is TerminalDockController controller)
            {
                this.notifier.Begin();
                this.notifier.PropertyField(nameof(TerminalDockController.Dock));
                this.notifier.PropertyField(nameof(TerminalDockController.IsRatio));
                if (controller.IsRatio == true)
                    this.notifier.PropertyField(nameof(TerminalDockController.Ratio));
                else
                    this.notifier.PropertyField(nameof(TerminalDockController.Length));
                this.notifier.End();
                if (this.notifier.IsModified == true)
                {
                    controller.UpdateLayout();
                }
            }
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalDockController.Dock));
            this.notifier.Add(nameof(TerminalDockController.IsRatio));
            this.notifier.Add(nameof(TerminalDockController.Length));
            this.notifier.Add(nameof(TerminalDockController.Ratio));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
