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

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace JSSoft.Terminal
{
    public abstract class InputHandler<T> : IInputHandler where T : InputHandlerContext
    {
        private readonly Dictionary<ITerminalGrid, T> contextByGrid = new Dictionary<ITerminalGrid, T>();

        protected virtual void OnSelect(InputHandlerContext context, BaseEventData eventData)
        {
            context.Select(eventData);
        }

        protected virtual void OnDeselect(InputHandlerContext context, BaseEventData eventData)
        {
            context.Deselect(eventData);
        }

        protected virtual void OnUpdate(InputHandlerContext context, BaseEventData eventData)
        {
            context.Update(eventData);
        }

        protected virtual void OnBeginDrag(InputHandlerContext context, PointerEventData eventData)
        {
            context.BeginDrag(eventData);
        }

        protected virtual void OnDrag(InputHandlerContext context, PointerEventData eventData)
        {
            context.Drag(eventData);
        }

        protected virtual void OnEndDrag(InputHandlerContext context, PointerEventData eventData)
        {
            context.EndDrag(eventData);
        }

        protected virtual void OnPointerClick(InputHandlerContext context, PointerEventData eventData)
        {
            context.PointerClick(eventData);
        }

        protected virtual void OnPointerDown(InputHandlerContext context, PointerEventData eventData)
        {
            context.PointerDown(eventData);
        }

        protected virtual void OnPointerEnter(InputHandlerContext context, PointerEventData eventData)
        {
            context.PointerEnter(eventData);
        }

        protected virtual void OnPointerExit(InputHandlerContext context, PointerEventData eventData)
        {
            context.PointerExit(eventData);
        }

        protected virtual void OnPointerUp(InputHandlerContext context, PointerEventData eventData)
        {
            context.PointerUp(eventData);
        }

        protected virtual void OnAttach(InputHandlerContext context, ITerminalGrid grid)
        {
            context.Attach(grid);
        }

        protected virtual void OnDetach(InputHandlerContext context, ITerminalGrid grid)
        {
            context.Detach(grid);
        }

        protected abstract T CreateContext(ITerminalGrid grid);

        #region IInputHandler

        void IInputHandler.Select(ITerminalGrid grid, BaseEventData eventData)
        {
            this.OnSelect(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.Deselect(ITerminalGrid grid, BaseEventData eventData)
        {
            this.OnDeselect(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.Update(ITerminalGrid grid, BaseEventData eventData)
        {
            this.OnUpdate(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.BeginDrag(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnBeginDrag(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.Drag(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnDrag(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.EndDrag(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnEndDrag(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.PointerClick(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnPointerClick(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.PointerDown(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnPointerDown(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.PointerUp(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnPointerUp(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.PointerEnter(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnPointerEnter(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.PointerExit(ITerminalGrid grid, PointerEventData eventData)
        {
            this.OnPointerExit(this.contextByGrid[grid], eventData);
        }

        void IInputHandler.Attach(ITerminalGrid grid)
        {
            this.contextByGrid.Add(grid, this.CreateContext(grid));
            this.OnAttach(this.contextByGrid[grid], grid);
        }

        void IInputHandler.Detach(ITerminalGrid grid)
        {
            this.OnDetach(this.contextByGrid[grid], grid);
            this.contextByGrid.Remove(grid);
        }

        #endregion
    }
}
