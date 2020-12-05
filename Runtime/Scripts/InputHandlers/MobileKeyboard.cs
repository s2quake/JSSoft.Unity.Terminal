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
using UnityEngine;

namespace JSSoft.Unity.Terminal.InputHandlers
{
    class MobileKeyboard : TerminalKeyboardBase
    {
        private TouchScreenKeyboard keyboard;

        public override string Text
        {
            get => this.keyboard != null ? this.keyboard.text : string.Empty;
            set
            {
                if (this.keyboard != null)
                {
                    this.keyboard.text = value;
                }
            }
        }

        public override RangeInt Selection
        {
            get => this.keyboard != null ? this.keyboard.selection : default(RangeInt);
            set
            {
                if (this.keyboard != null)
                {
                    this.keyboard.selection = value;
                }
            }
        }

        public override Rect Area
        {
            get
            {
                if (TerminalEnvironment.IsIPhone == true)
                {
#if UNITY_EDITOR || UNITY_STANDALONE
                    throw new NotImplementedException();
#else
                    return TouchScreenKeyboard.area;
#endif
                }
                else if (TerminalEnvironment.IsAndroid == true)
                {
                    using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        var view = unityClass.GetStatic<AndroidJavaObject>("currentActivity")
                            .Get<AndroidJavaObject>("mUnityPlayer")
                            .Call<AndroidJavaObject>("getView");

                        var height = 0;
                        try
                        {
                            var dialog = unityClass.GetStatic<AndroidJavaObject>("currentActivity")
                                .Get<AndroidJavaObject>("mUnityPlayer")
                                .Get<AndroidJavaObject>("b");

                            var decorView = dialog.Call<AndroidJavaObject>("getWindow")
                                .Call<AndroidJavaObject>("getDecorView");

                            height = decorView.Call<int>("getHeight");
                        }
                        catch
                        {
                        }
                        using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                        {
                            view.Call("getWindowVisibleDisplayFrame", rect);
                            var h = (float)(Screen.height - rect.Call<int>("height") + height);
                            return new Rect(0, Screen.height - h, Screen.width, h);
                        }
                    }
                }
                else
                {
                    return default(Rect);
                }
            }
        }

        protected override void OnOpen(string text)
        {
            this.keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default, false, false, false, false, "type command");
            if (TerminalEnvironment.IsIPhone)
            {
                this.keyboard.active = true;
                this.keyboard.text = text;
            }
        }

        protected override void OnClose()
        {
            this.keyboard.active = false;
            this.keyboard = null;
        }

        protected override bool? OnUpdate()
        {
            if (this.keyboard != null)
            {
                if (this.keyboard.status == TouchScreenKeyboard.Status.Done)
                {
                    this.keyboard.active = false;
                    this.keyboard = null;
                    return true;
                }
                else if (this.keyboard.status == TouchScreenKeyboard.Status.Canceled)
                {
                    this.keyboard.active = false;
                    this.keyboard = null;
                    return false;
                }
                else if (this.keyboard.status == TouchScreenKeyboard.Status.Visible)
                {
                }
                else if (this.keyboard.status == TouchScreenKeyboard.Status.LostFocus)
                {
                    this.keyboard.active = false;
                    this.keyboard = null;
                    return false;
                }
            }
            return null;
        }
    }
}
