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

using System;
using System.Collections.Generic;
using JSSoft.Unity.Terminal.Commands;
using JSSoft.Unity.Terminal.InputHandlers;
using UnityEditor;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Editor
{
    [InitializeOnLoad]
    internal sealed class EditorInitializer
    {
        static EditorInitializer()
        {
            TerminalEnvironment.IsStandalone = EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX ||
                                               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                                               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;

            TerminalEnvironment.IsMobile = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                                           EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;

            ExitCommand.ExitAction = new Action(() => UnityEditor.EditorApplication.isPlaying = false);
            TerminalFont.SetDirtyCallback = new Action<TerminalFont>((font) => EditorUtility.SetDirty(font));
            MobileInputHandlerContext.KeyboardCreator = new Func<TerminalKeyboardBase>(() => new EditorKeyboard());
        }
    }
}