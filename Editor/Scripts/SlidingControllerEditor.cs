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
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(SlidingController))]
    public class SlidingControllerEditor : UnityEditor.Editor
    {
        private EditorPropertyNotifier notifier;
        private GUIContent showContent;
        private GUIContent hideContent;
        private GUIContent resetContent;

        public override void OnInspectorGUI()
        {
            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(SlidingController.Grid));
            this.notifier.PropertyField(nameof(TerminalSlidingController.Direction));
            this.notifier.End();

            GUI.enabled = Application.isPlaying == false;
            if (GUILayout.Button(this.showContent) == true)
            {
                if (this.target is SlidingController controller)
                {
                    controller.Show();
                    EditorUtility.SetDirty(controller);
                }
            }
            if (GUILayout.Button(this.hideContent) == true)
            {
                if (this.target is SlidingController controller)
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
            this.notifier.Add(nameof(SlidingController.Grid));
            this.notifier.Add(nameof(SlidingController.Direction));
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
