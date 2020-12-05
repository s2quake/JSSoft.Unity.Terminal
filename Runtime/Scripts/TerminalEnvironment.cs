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

using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalEnvironment
    {
        static TerminalEnvironment()
        {
            IsMac = (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);
            IsWindows = (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);
            IsIPhone = (Application.platform == RuntimePlatform.IPhonePlayer);
            IsAndroid = (Application.platform == RuntimePlatform.Android);
            IsStandalone = IsMac == true || IsWindows == true;
            IsMobile = IsIPhone == true || IsAndroid == true;
        }

        public static bool IsMac { get; }

        public static bool IsWindows { get; }

        public static bool IsIPhone { get; }

        public static bool IsAndroid { get; }

        public static bool IsStandalone {get; internal set;}

        public static bool IsMobile {get; internal set;}
    }
}
