﻿// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JSSoft.Unity.Terminal
{
    public static class TerminalEnvironment
    {
        public static bool IsMac => (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);

        public static bool IsWindows => (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);

        public static bool IsIPhone => (Application.platform == RuntimePlatform.IPhonePlayer);

        public static bool IsAndroid => (Application.platform == RuntimePlatform.Android);

#if UNITY_EDITOR
        public static bool IsStandalone => EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX ||
                                           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                                           EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;

        public static bool IsMobile => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                                       EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#else
        public static bool IsStandalone => IsMac == true || IsWindows == true;

        public static bool IsMobile => IsIPhone == true || IsAndroid == true;
#endif

    }
}
