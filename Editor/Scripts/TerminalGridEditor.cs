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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [CustomEditor(typeof(TerminalGrid))]
    [CanEditMultipleObjects]
    public class TerminalGridEditor : UnityEditor.Editor
    {
        private readonly List<TerminalForeground> foregroundList = new List<TerminalForeground>();
        private EditorPropertyNotifier notifier;
        private bool isDebug = false;

        public override void OnInspectorGUI()
        {
            var styleProperty = this.notifier.GetProperty(nameof(TerminalGrid.Style));
            this.isDebug = GUILayout.Toggle(this.isDebug, "Debug Mode");
            if (isDebug == true)
            {
                base.OnInspectorGUI();
                return;
            }

            if (this.foregroundList.Any() == true)
            {
                var message = TerminalStrings.GetString("TerminalGrid.RefreshForeground");
                EditorGUILayout.HelpBox(message, MessageType.Warning);
                if (GUILayout.Button("Refresh Foreground") == true)
                {
                    foreach (var item in this.foregroundList)
                    {
                        item.RefreshChilds();
                    }
                    this.foregroundList.Clear();
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }

            this.notifier.Begin();
            this.notifier.PropertyScript();
            this.notifier.PropertyField(nameof(TerminalGrid.Style));
            GUILayout.Space(10);
            if (styleProperty.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("Property cannot be changed when style is applied.", MessageType.Info);
            }
            GUI.enabled = styleProperty.objectReferenceValue == null;
            this.notifier.PropertyField(nameof(TerminalGrid.Font));
            this.notifier.PropertyField(nameof(TerminalGrid.BackgroundColor));
            this.notifier.PropertyField(nameof(TerminalGrid.ForegroundColor));
            this.notifier.PropertyField(nameof(TerminalGrid.SelectionColor));
            this.notifier.PropertyField(nameof(TerminalGrid.SelectionTextColor));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorColor));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorTextColor));
            this.notifier.PropertyField(nameof(TerminalGrid.FallbackTexture));
            this.notifier.PropertyField(nameof(TerminalGrid.ColorPalette));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorStyle));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorThickness));
            this.notifier.PropertyField(nameof(TerminalGrid.IsCursorBlinkable));
            this.notifier.PropertyField(nameof(TerminalGrid.CursorBlinkDelay));
            this.notifier.PropertyField(nameof(TerminalGrid.IsScrollForwardEnabled));
            this.notifier.PropertyField(nameof(TerminalGrid.BehaviourList));
            GUI.enabled = true;

            GUILayout.Space(10);
            this.notifier.PropertyField(nameof(TerminalGrid.MaxBufferHeight));
            this.notifier.PropertyField(nameof(TerminalGrid.Padding));
            this.notifier.End();
        }

        protected virtual void OnEnable()
        {
            this.notifier = new EditorPropertyNotifier(this);
            this.notifier.Add(nameof(TerminalGrid.Style));
            this.notifier.Add(nameof(TerminalGrid.Font));
            this.notifier.Add(nameof(TerminalGrid.BackgroundColor));
            this.notifier.Add(nameof(TerminalGrid.ForegroundColor));
            this.notifier.Add(nameof(TerminalGrid.SelectionColor));
            this.notifier.Add(nameof(TerminalGrid.SelectionTextColor));
            this.notifier.Add(nameof(TerminalGrid.CursorColor));
            this.notifier.Add(nameof(TerminalGrid.CursorTextColor));
            this.notifier.Add(nameof(TerminalGrid.FallbackTexture));
            this.notifier.Add(nameof(TerminalGrid.ColorPalette));
            this.notifier.Add(nameof(TerminalGrid.CursorStyle));
            this.notifier.Add(nameof(TerminalGrid.CursorThickness));
            this.notifier.Add(nameof(TerminalGrid.IsCursorBlinkable));
            this.notifier.Add(nameof(TerminalGrid.CursorBlinkDelay));
            this.notifier.Add(nameof(TerminalGrid.IsScrollForwardEnabled));
            this.notifier.Add(nameof(TerminalGrid.BehaviourList), EditorPropertyUsage.DisallowNotification | EditorPropertyUsage.IncludeChildren);
            this.notifier.Add(nameof(TerminalGrid.MaxBufferHeight));
            this.notifier.Add(nameof(TerminalGrid.Padding), EditorPropertyUsage.IncludeChildren);
            this.notifier.PropertyChanged += Notifier_PropertyChanged;

            foreach (var item in this.targets)
            {
                if (item is TerminalGrid grid)
                {
                    var foreground = grid.GetComponentInChildren<TerminalForeground>();
                    if (foreground.VerifyRefreshChilds() == true)
                    {
                        this.foregroundList.Add(foreground);
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            this.notifier.Dispose();
            this.notifier = null;
            this.foregroundList.Clear();
        }

        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TerminalGrid.Padding):
                case nameof(TerminalGrid.Font):
                    {
                        foreach (var item in this.targets)
                        {
                            if (item is TerminalGrid grid)
                            {
                                grid.UpdateLayout();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
