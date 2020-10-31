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
            terminalGrid.Disabled += TerminalGrid_Disabled;
        }

        protected override void OnDestroy()
        {
            var terminalGrid = GetComponent<TerminalGridBase>();
            terminalGrid.LayoutChanged -= TerminalGrid_LayoutChanged;
            terminalGrid.Disabled -= TerminalGrid_Disabled;
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

        private void TerminalGrid_Disabled(object sender, EventArgs e)
        {
            this.OnStateSave();
        }
    }
}
