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

using UnityEngine;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal.Behaviours
{
    public class TerminalScrollbarBehaviour : TerminalBehaviourBase
    {
        protected override void OnAttach(ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var scrollbar = gameObject.GetComponentInChildren<TerminalScrollbar>();
            var scrollbarImage = scrollbar.GetComponent<Image>();
            var handleRect = scrollbar.handleRect;
            var handleRectImage = handleRect.GetComponent<Image>();
            var animator = scrollbar.GetComponent<Animator>();
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            var color = handleRectImage.color;
#endif
            animator.enabled = true;
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            scrollbarImage.color = new Color(0.54509803f, 0.54509803f, 0.54509803f, 0.0f);
            scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
            handleRectImage.color = new Color(color.r, color.g, color.b, 0.0f);
            handleRectImage.pixelsPerUnitMultiplier = 0.5f;
#endif
        }

        protected override void OnDetach(ITerminalGrid grid)
        {

        }
    }
}