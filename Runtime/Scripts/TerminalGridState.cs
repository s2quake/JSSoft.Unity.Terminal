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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(TerminalBase))]
    [RequireComponent(typeof(TerminalGridBase))]
    [DefaultExecutionOrder(int.MaxValue)]
    public class TerminalGridState : TerminalStateBase
    {
        public TerminalGridState()
            : base("Terminal")
        {
        }

        protected override void Awake()
        {
            base.Awake();
            var terminalGrid = GetComponent<TerminalGridBase>();
            terminalGrid.LayoutChanged += TerminalGrid_LayoutChanged;
        }

        protected override void OnDestroy()
        {
            var terminalGrid = GetComponent<TerminalGridBase>();
            terminalGrid.LayoutChanged -= TerminalGrid_LayoutChanged;
            this.OnStateSave();
            base.OnDestroy();
        }

        protected virtual void OnStateSave()
        {
            var terminalGrid = GetComponent<TerminalGridBase>();
            this.SetState(terminalGrid.Save());
        }

        protected virtual void OnStateLoad()
        {
            var terminalGrid = GetComponent<TerminalGridBase>();
            if (this.TryGetState<TerminalGridData>(out var state))
            {
                terminalGrid.Load(state);
                this.ResetState();
            }
        }

        private void TerminalGrid_LayoutChanged(object sender, EventArgs e)
        {
            this.OnStateLoad();
        }
    }
}
