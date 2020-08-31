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
using UnityEngine.UI;

namespace JSSoft.Terminal.Behaviours
{
    public class WindowsScrollbarBehaviour : TerminalBehaviourBase
    {
        protected override void OnAttach(ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var scrollbar = gameObject.GetComponentInChildren<TerminalScrollbar>();
            var scrollbarImage = scrollbar.GetComponent<Image>();
            var handleRect = scrollbar.handleRect;
            var handleRectImage = handleRect.GetComponent<Image>();
            var animator = scrollbar.GetComponent<Animator>();
            var color = handleRectImage.color;
            animator.enabled = false;
            scrollbarImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            // scrollbarImage.pixelsPerUnitMultiplier = 10.0f;
            handleRectImage.color = new Color(color.r, color.g, color.b, 1.0f);
            // handleRectImage.pixelsPerUnitMultiplier = 10.0f;
        }

        protected override void OnDetach(ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var scrollbar = gameObject.GetComponentInChildren<TerminalScrollbar>();
            var scrollbarImage = scrollbar.GetComponent<Image>();
            var handleRect = scrollbar.handleRect;
            var handleRectImage = handleRect.GetComponent<Image>();
            var animator = scrollbar.GetComponent<Animator>();
            var color = handleRectImage.color;
            animator.enabled = true;
            scrollbarImage.color = new Color(0.54509803f, 0.54509803f, 0.54509803f, 0.0f);
            // scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
            handleRectImage.color = new Color(color.r, color.g, color.b, 0.0f);
            // handleRectImage.pixelsPerUnitMultiplier = 0.5f;
        }
    }
}