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

namespace JSSoft.UI
{
    public abstract class InputHandler : IInputHandler
    {
        private static readonly IInputHandler macOSInputHandler;
        private static readonly IInputHandler windowsInputHandler;
        private readonly Dictionary<ITerminalGrid, InputHandlerContext> contextByGrid = new Dictionary<ITerminalGrid, InputHandlerContext>();

        static InputHandler()
        {
            macOSInputHandler = new InputHandlers.MacOSInputHandler();
            windowsInputHandler = new InputHandlers.WindowsInputHandler();
        }

        public static IInputHandler GetDefaultHandler()
        {
            if (TerminalEnvironment.IsMac == true)
                return macOSInputHandler;
            else if (TerminalEnvironment.IsWindows == true)
                return windowsInputHandler;
            throw new NotImplementedException();
        }

        protected abstract bool OnBeginDrag(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnDrag(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnEndDrag(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnPointerClick(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnPointerDown(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnPointerUp(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnPointerEnter(InputHandlerContext context, PointerEventData eventData);

        protected abstract bool OnPointerExit(InputHandlerContext context, PointerEventData eventData);

        protected virtual InputHandlerContext CreateContext(ITerminalGrid grid)
        {
            return new InputHandlerContext(grid);
        }

        private InputHandlerContext this[ITerminalGrid grid]
        {
            get
            {
                if (this.contextByGrid.ContainsKey(grid) == false)
                {
                    this.contextByGrid.Add(grid, this.CreateContext(grid));
                }
                return this.contextByGrid[grid];
            }
        }

        #region IInputHandler

        bool IInputHandler.BeginDrag(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnBeginDrag(this[grid], eventData);
        }

        bool IInputHandler.Drag(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnDrag(this[grid], eventData);
        }

        bool IInputHandler.EndDrag(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnEndDrag(this[grid], eventData);
        }

        bool IInputHandler.PointerClick(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnPointerClick(this[grid], eventData);
        }

        bool IInputHandler.PointerDown(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnPointerDown(this[grid], eventData);
        }

        bool IInputHandler.PointerUp(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnPointerUp(this[grid], eventData);
        }

        bool IInputHandler.PointerEnter(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnPointerEnter(this[grid], eventData);
        }

        bool IInputHandler.PointerExit(ITerminalGrid grid, PointerEventData eventData)
        {
            return this.OnPointerExit(this[grid], eventData);
        }

        #endregion
    }
}
