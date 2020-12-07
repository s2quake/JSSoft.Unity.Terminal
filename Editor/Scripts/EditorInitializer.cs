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

using System;
using System.Collections.Generic;
using System.Text;
using JSSoft.Unity.Terminal.Commands;
using JSSoft.Unity.Terminal.InputHandlers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal.Editor
{
    [InitializeOnLoad]
    internal sealed class EditorInitializer
    {
        static EditorInitializer()
        {
            TerminalEnvironment.IsStandalone = EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX ||
                                               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                                               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 ||
                                               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux ||
                                               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64;

            TerminalEnvironment.IsMobile = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                                           EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;

            ExitCommand.ExitAction = new Action(() => UnityEditor.EditorApplication.isPlaying = false);
            TerminalFont.SetDirtyCallback = new Action<TerminalFont>((font) => EditorUtility.SetDirty(font));
            MobileInputHandlerContext.KeyboardCreator = new Func<TerminalKeyboardBase>(() => new EditorKeyboard());
            EditorSceneManager.sceneOpened += EditorSceneManager_SceneOpened;
        }

        private static void EditorSceneManager_SceneOpened(Scene scene, OpenSceneMode mode)
        {
            var items = GameObject.FindObjectsOfType<TerminalForeground>();
            foreach (var item in items)
            {
                if (item.VerifyRefreshChilds() == true && item.Grid != null)
                {
                    var grid = item.Grid;
                    var sb = new StringBuilder();
                    var path = GameObjectUtility.GetPath(grid.gameObject);
                    var message = TerminalStrings.GetString("TerminalGrid.RefreshForeground");
                    sb.AppendLine(message);
                    sb.AppendLine($"path: {path}");
                    Debug.LogWarning(sb.ToString(), grid);
                }
            }
        }
    }
}