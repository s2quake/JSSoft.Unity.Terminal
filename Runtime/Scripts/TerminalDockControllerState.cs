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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(TerminalDockController))]
    [DefaultExecutionOrder(int.MaxValue)]
    public class TerminalDockControllerState : TerminalStateBase
    {
        public TerminalDockControllerState()
            : base("TerminalLayout")
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            var controller = this.GetComponent<TerminalDockController>();
            if (this.TryGetState<TerminalDockController.DockData>(out var state) == true)
            {
                controller.Load(state);
                this.ResetState();
            }
        }

        protected override void OnDisable()
        {
            var controller = this.GetComponent<TerminalDockController>();
            this.SetState(controller.Save());
            base.OnDisable();
        }
    }
}
