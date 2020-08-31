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
using UnityEngine.UI;

namespace JSSoft.Terminal
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
            }
        }

        private void Keyboard_Done(object sender, TerminalKeyboardEventArgs e)
        {
            if (sender is ITerminalKeyboard keyboard)
            {
                this.keyboardLayout.ignoreLayout = true;
                this.grids.Remove(keyboard.Grid);
            }
        }

        private void Keyboard_Canceled(object sender, EventArgs e)
        {
            if (sender is ITerminalKeyboard keyboard)
            {
                this.keyboardLayout.ignoreLayout = true;
                this.grids.Remove(keyboard.Grid);
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
