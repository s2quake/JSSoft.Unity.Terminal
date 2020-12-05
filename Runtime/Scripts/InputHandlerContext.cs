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

using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    public class InputHandlerContext
    {
        public virtual void Select(BaseEventData eventData)
        {
        }

        public virtual void Deselect(BaseEventData eventData)
        {
        }

        public virtual void Update(BaseEventData eventData)
        {
        }

        public virtual void BeginDrag(PointerEventData eventData)
        {
        }

        public virtual void Drag(PointerEventData eventData)
        {
        }

        public virtual void EndDrag(PointerEventData eventData)
        {
        }

        public virtual void PointerClick(PointerEventData eventData)
        {
        }

        public virtual void PointerDown(PointerEventData eventData)
        {
        }

        public virtual void PointerEnter(PointerEventData eventData)
        {
        }

        public virtual void PointerExit(PointerEventData eventData)
        {
        }

        public virtual void PointerUp(PointerEventData eventData)
        {
        }

        public virtual void Attach(ITerminalGrid grid)
        {
            this.Grid = grid;
        }

        public virtual void Detach(ITerminalGrid grid)
        {
            this.Grid = null;
        }

        public ITerminalGrid Grid { get; private set; }

        public ITerminal Terminal => this.Grid != null ? this.Grid.Terminal : null;
    }
}
