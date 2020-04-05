using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using JSSoft.UI;
using UnityEngine.UI;

namespace JSSoft.UI.Behaviours
{
    public class WindowsInputBehaviour : TerminalBehaviourBase
    {
        private static readonly IInputHandler windowsInputHandler = new InputHandlers.WindowsInputHandler();

        protected override void OnAttach(ITerminalGrid grid)
        {
            grid.InputHandler = windowsInputHandler;
        }

        protected override void OnDetach(ITerminalGrid grid)
        {
            grid.InputHandler = null;
        }
    }
}