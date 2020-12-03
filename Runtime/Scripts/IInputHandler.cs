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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    public interface IInputHandler
    {
        void Select(ITerminalGrid grid, BaseEventData eventData);

        void Deselect(ITerminalGrid grid, BaseEventData eventData);

        void Update(ITerminalGrid grid, BaseEventData eventData);

        void BeginDrag(ITerminalGrid grid, PointerEventData eventData);

        void Drag(ITerminalGrid grid, PointerEventData eventData);

        void EndDrag(ITerminalGrid grid, PointerEventData eventData);

        void PointerClick(ITerminalGrid grid, PointerEventData eventData);

        void PointerDown(ITerminalGrid grid, PointerEventData eventData);

        void PointerUp(ITerminalGrid grid, PointerEventData eventData);

        void PointerEnter(ITerminalGrid grid, PointerEventData eventData);

        void PointerExit(ITerminalGrid grid, PointerEventData eventData);

        void Attach(ITerminalGrid grid);

        void Detach(ITerminalGrid grid);
    }
}
