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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    [DisallowMultipleComponent]
    public class TerminalKeyboardLayoutGroup : UIBehaviour
    {
        [SerializeField]
        private LayoutElement terminalLayout;
        [SerializeField]
        private LayoutElement keyboardLayout;

        private readonly HashSet<ITerminalGrid> grids = new HashSet<ITerminalGrid>();
        private VerticalLayoutGroup layoutGroup;
        private ITerminalKeyboard keyboard;

        [FieldName(nameof(terminalLayout))]
        public LayoutElement TerminalLayout
        {
            get => this.terminalLayout;
            set
            {
                if (this.terminalLayout != value)
                {
                    this.terminalLayout = value;
                }
            }
        }

        [FieldName(nameof(keyboardLayout))]
        public LayoutElement KeyboardLayout
        {
            get => this.keyboardLayout;
            set
            {
                if (this.keyboardLayout != value)
                {
                    this.keyboardLayout = value;
                }
            }
        }

        protected virtual void Update()
        {
            if (this.keyboard != null && this.keyboard.Area.height != this.keyboardLayout.preferredHeight)
            {
                this.keyboardLayout.preferredHeight = this.keyboard.Area.height;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalKeyboardEvents.Opened += Keyboard_Opened;
            TerminalKeyboardEvents.Done += Keyboard_Done;
            TerminalKeyboardEvents.Canceled += Keyboard_Canceled;
            TerminalKeyboardEvents.Changed += Keyboard_Changed;
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            this.layoutGroup = this.GetComponent<VerticalLayoutGroup>();
            this.keyboardLayout.ignoreLayout = true;
        }

        protected override void OnDisable()
        {
            this.layoutGroup = null;
            TerminalKeyboardEvents.Opened -= Keyboard_Opened;
            TerminalKeyboardEvents.Done -= Keyboard_Done;
            TerminalKeyboardEvents.Canceled -= Keyboard_Canceled;
            TerminalKeyboardEvents.Changed -= Keyboard_Changed;
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            base.OnDisable();
        }

        private void Keyboard_Opened(object sender, TerminalKeyboardEventArgs e)
        {
            if (sender is ITerminalKeyboard keyboard)
            {
                this.grids.Add(keyboard.Grid);
                this.UpdateLayout(keyboard);
                this.keyboard = keyboard;
            }
        }

        private void Keyboard_Done(object sender, TerminalKeyboardEventArgs e)
        {
            if (sender is ITerminalKeyboard keyboard)
            {
                this.keyboardLayout.ignoreLayout = true;
                this.grids.Remove(keyboard.Grid);
                this.keyboard = null;
            }
        }

        private void Keyboard_Canceled(object sender, EventArgs e)
        {
            if (sender is ITerminalKeyboard keyboard)
            {
                this.keyboardLayout.ignoreLayout = true;
                this.grids.Remove(keyboard.Grid);
                this.keyboard = null;
            }
        }

        private void Keyboard_Changed(object sender, TerminalKeyboardEventArgs e)
        {
            if (sender is ITerminalKeyboard keyboard)
            {
                this.UpdateLayout(keyboard);
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid)
            {
                if (this.grids.Contains(grid) == true)
                {
                    grid.ScrollToCursor();
                }
            }
        }

        private void UpdateLayout(ITerminalKeyboard keyboard)
        {
            this.layoutGroup.childForceExpandHeight = false;
            this.terminalLayout.flexibleHeight = 1;
            this.keyboardLayout.ignoreLayout = false;
            this.keyboardLayout.flexibleHeight = 0;
            this.keyboardLayout.preferredHeight = keyboard.Area.height;
        }
    }
}
