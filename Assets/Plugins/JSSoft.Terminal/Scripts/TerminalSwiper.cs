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

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.Terminal
{
    [RequireComponent(typeof(Image))]
    public class TerminalSwiper : Selectable
    {
        private ITerminalGrid grid;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            // Debug.Log("qwerwqer");
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (this.grid != null)
            {
                Debug.Log("yes");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.GetComponent<Image>().enabled = false;
            TerminalKeyboardEvents.Opened += TerminalKeyboard_Opened;
            TerminalKeyboardEvents.Done += TerminalKeyboard_Done;
            TerminalKeyboardEvents.Canceled += TerminalKeyboard_Canceled;
        }

        protected override void OnDisable()
        {
            TerminalKeyboardEvents.Opened -= TerminalKeyboard_Opened;
            TerminalKeyboardEvents.Done -= TerminalKeyboard_Done;
            TerminalKeyboardEvents.Canceled -= TerminalKeyboard_Canceled;
            base.OnDisable();
        }

        private void TerminalKeyboard_Opened(object sender, TerminalKeyboardEventArgs e)
        {
            this.GetComponent<Image>().enabled = true;
            if (sender is TerminalKeyboardBase keyboard)
            {
                this.grid = keyboard.Grid;
            }
        }

        private void TerminalKeyboard_Done(object sender, TerminalKeyboardEventArgs e)
        {
            this.GetComponent<Image>().enabled = false;
            this.grid = null;
        }

        private void TerminalKeyboard_Canceled(object sender, EventArgs e)
        {
            this.GetComponent<Image>().enabled = false;
            this.grid = null;
        }
    }
}
