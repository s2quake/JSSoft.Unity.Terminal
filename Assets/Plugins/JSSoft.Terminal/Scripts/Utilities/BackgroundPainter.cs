﻿// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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
using System.Threading.Tasks;
using JSSoft.Terminal;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;

namespace JSSoft.Terminal.Utilities
{
    public class BackgroundPainter : MaskableGraphic
    {
        [SerializeField]
        private TerminalBase grid = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.Validated += TerminalGrid_Validated;
            TerminalGridEvents.LayoutChanged += TerminalGrid_LayoutChanged;
            TerminalGridEvents.PropertyChanged += TerminalGrid_PropertyChanged;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.Validated -= TerminalGrid_Validated;
            TerminalGridEvents.LayoutChanged -= TerminalGrid_LayoutChanged;
            TerminalGridEvents.PropertyChanged -= TerminalGrid_PropertyChanged;
            base.OnDisable();
        }

        private void TerminalGrid_Validated(object sender, EventArgs e)
        {
            if (sender is TerminalGridBase grid && this.grid == grid)
            {
                base.color = grid.BackgroundColor;
            }
        }

        private void TerminalGrid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGridBase grid && this.grid == grid)
            {
                var canvas = this.GetComponentInParent<Canvas>();
                var canvasRect = canvas.GetComponent<RectTransform>();
                var rect = this.GetComponent<RectTransform>();
                rect.sizeDelta = canvasRect.sizeDelta;
            }
        }

        private void TerminalGrid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGridBase grid && this.grid == grid)
            {
                base.color = grid.BackgroundColor;
            }
        }
    }
}