// MIT License
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

#if UNITY_IOS
        public override Rect Area => TouchScreenKeyboard.area;
#elif UNITY_ANDROID
        public override Rect Area
        {
            get
            {
                // using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                // {
                //     var view = unityClass.GetStatic<AndroidJavaObject>("currentActivity")
                //                          .Get<AndroidJavaObject>("mUnityPlayer")
                //                          .Call<AndroidJavaObject>("getView");
                //     using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                //     {
                //         view.Call("getWindowVisibleDisplayFrame", rect);
                //         var height = Screen.height - rect.Call<int>("height");
                //         return new Rect(0, Screen.height - height, Screen.width, height);
                //     }
                // }

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
        }
#else
        public override Rect Area => default(Rect);
#endif

        protected override void OnOpen(string text)
        {
            this.keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default, false, false, false, false, "type command");
#if UNITY_IOS
            this.keyboard.active = true;
            this.keyboard.text = text;
#endif
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
                    this.Text = this.keyboard.text;
                    this.Selection = this.keyboard.selection;
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
