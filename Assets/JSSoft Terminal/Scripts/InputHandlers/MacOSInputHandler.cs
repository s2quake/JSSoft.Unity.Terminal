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
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI.InputHandlers
{
    public class MacOSInputHandler : InputHandler
    {
        public MacOSInputHandler()
        {
        }

        protected override bool OnBeginDrag(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.BeginDrag(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnDrag(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.Drag(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnEndDrag(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.EndDrag(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnPointerClick(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.PointerClick(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnPointerDown(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.PointerDown(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnPointerEnter(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.PointerEnter(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnPointerExit(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.PointerExit(eventData);
            }
            throw new NotImplementedException();
        }

        protected override bool OnPointerUp(InputHandlerContext context, PointerEventData eventData)
        {
            if (context is MacOSInputHandlerContext obj)
            {
                return obj.PointerUp(eventData);
            }
            throw new NotImplementedException();
        }

        protected override InputHandlerContext CreateContext(ITerminalGrid grid)
        {
            return new MacOSInputHandlerContext(grid);
        }
    }
}
