using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using JSSoft.UI;
using UnityEngine.UI;

namespace JSSoft.UI.Behaviours
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
            scrollbarImage.pixelsPerUnitMultiplier = 10.0f;
            handleRectImage.color = new Color(color.r, color.g, color.b, 1.0f);
            handleRectImage.pixelsPerUnitMultiplier = 10.0f;
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
            scrollbarImage.pixelsPerUnitMultiplier = 0.5f;
            handleRectImage.color = new Color(color.r, color.g, color.b, 0.0f);
            handleRectImage.pixelsPerUnitMultiplier = 0.5f;
        }
    }
}