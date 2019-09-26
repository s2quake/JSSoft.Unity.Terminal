// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class CustomTerminal : Selectable, IUpdateSelectedHandler
    {
        [SerializeField]
        private CompositionRenderer compositionRenderer;
        private Event processingEvent = new Event();
        private string text = string.Empty;

        private BaseInput inputSystem
        {
            get
            {
                if (EventSystem.current && EventSystem.current.currentInputModule)
                    return EventSystem.current.currentInputModule.input;
                return null;
            }
        }

        private string compositionString
        {
            get { return inputSystem != null ? inputSystem.compositionString : Input.compositionString; }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            this.inputSystem.imeCompositionMode = IMECompositionMode.On;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            Debug.Log(nameof(OnDeselect));
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            while (Event.PopEvent(this.processingEvent))
            {
                if (this.processingEvent.rawType == EventType.KeyDown)
                {
                    var keyCode = this.processingEvent.keyCode;
                    var modifiers = this.processingEvent.modifiers;
                    var key = $"{modifiers}+{keyCode}: '{this.processingEvent.character}'";
                    if (this.processingEvent.character != 0)
                    {
                        this.text += this.inputSystem.compositionString;
                        // if (this.inputSystem.compositionString != string.Empty)
                        // {
                        // }
                        // else
                        // {
                        //     int wqer = 0;
                        // }
                        if (this.compositionRenderer != null)
                        {
                            this.compositionRenderer.Character = this.processingEvent.character;
                        }
                    }
                    else
                    {
                        // this.text += this.processingEvent.character;
                    }
                    // this.text += this.processingEvent.character;
                    Debug.Log(key);
                    // Debug.Log($"imeCompositionMode: {this.inputSystem.imeCompositionMode}");
                    // Debug.Log($"compositionString: {this.inputSystem.compositionString}");
                    // Debug.Log(this.text);
                }
            }
            eventData.Use();
        }
    }
}
