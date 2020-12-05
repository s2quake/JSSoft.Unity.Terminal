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
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalSlidingController))]
    public class TerminalSlidingControllerEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;
        private GUIContent showContent;
        private GUIContent hideContent;
        private GUIContent resetContent;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalSlidingController.Grid));
            this.notifier.PropertyField(nameof(TerminalSlidingController.Direction));
            this.notifier.PropertyField(nameof(TerminalSlidingController.KeyCode));
            this.notifier.PropertyField(nameof(TerminalSlidingController.Modifiers));
            this.notifier.End();

            GUI.enabled = Application.isPlaying == false;
            if (GUILayout.Button(this.showContent) == true)
            {
                if (this.target is TerminalSlidingController controller)
                {
                    controller.Show();
                    EditorUtility.SetDirty(controller);
                }
            }
            if (GUILayout.Button(this.hideContent) == true)
            {
                if (this.target is TerminalSlidingController controller)
                {
                    controller.Hide();
                    EditorUtility.SetDirty(controller);
                }
            }
            if (GUILayout.Button(this.resetContent) == true)
            {
                if (this.target is SlidingController controller)
                {
                    controller.ResetPosition();
                    EditorUtility.SetDirty(controller);
                }
            }
            GUI.enabled = true;
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalSlidingController.Grid));
            this.notifier.Add(nameof(TerminalSlidingController.Direction));
            this.notifier.Add(nameof(TerminalSlidingController.KeyCode));
            this.notifier.Add(nameof(TerminalSlidingController.Modifiers));
            this.showContent = new GUIContent("Show", TerminalStrings.GetString("SlidingController.Show"));
            this.hideContent = new GUIContent("Hide", TerminalStrings.GetString("SlidingController.Hide"));
            this.resetContent = new GUIContent("Reset", TerminalStrings.GetString("SlidingController.Reset"));
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
        }
    }
}
