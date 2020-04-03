// MIT License
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
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace JSSoft.UI.Behaviours
{
    public class WindowsCursorBehaviour : TerminalBehaviourBase
    {
        protected override void OnAttach(ITerminalGrid grid)
        {
            grid.GotFocus += Grid_GotFocus;
            grid.LostFocus += Grid_LostFocus;
            grid.IsCursorVisible = grid.IsFocused;
        }

        protected override void OnDetach(ITerminalGrid grid)
        {
            grid.GotFocus -= Grid_GotFocus;
            grid.LostFocus -= Grid_LostFocus;
            grid.IsCursorVisible = true;
        }

        private void Grid_GotFocus(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid)
            {
                grid.IsCursorVisible = true;
            }
        }

        private void Grid_LostFocus(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid)
            {
                grid.IsCursorVisible = false;
            }
        }
    }
}