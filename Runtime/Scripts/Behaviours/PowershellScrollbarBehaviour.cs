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
    public class PowershellScrollbarBehaviour : TerminalBehaviourBase
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
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            scrollbarImage.pixelsPerUnitMultiplier = 10.0f;
#endif
            handleRectImage.color = new Color(color.r, color.g, color.b, 1.0f);
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            handleRectImage.pixelsPerUnitMultiplier = 10.0f;
#endif
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
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
#endif
            handleRectImage.color = new Color(color.r, color.g, color.b, 0.0f);
#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            handleRectImage.pixelsPerUnitMultiplier = 0.5f;
#endif
        }
    }
}