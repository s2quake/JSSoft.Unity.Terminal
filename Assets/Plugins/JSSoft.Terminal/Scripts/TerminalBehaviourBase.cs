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
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Terminal
{
    public abstract class TerminalBehaviourBase : ScriptableObject
    {
        private readonly List<ITerminalGrid> gridList = new List<ITerminalGrid>();

        protected virtual void OnEnable()
        {
            TerminalGridEvents.Enabled += Grid_Enabled;
            TerminalGridEvents.Disabled += Grid_Disabled;
            TerminalGridEvents.Validated += Grid_Validated;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
        }

        protected virtual void OnDisable()
        {
            TerminalGridEvents.Enabled -= Grid_Enabled;
            TerminalGridEvents.Disabled -= Grid_Disabled;
            TerminalGridEvents.Validated -= Grid_Validated;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
        }

        protected abstract void OnAttach(ITerminalGrid grid);

        protected abstract void OnDetach(ITerminalGrid grid);

        private void Attach(ITerminalGrid grid)
        {
            this.OnAttach(grid);
            this.gridList.Add(grid);
        }

        private void Detach(ITerminalGrid grid)
        {
            this.OnDetach(grid);
            this.gridList.Remove(grid);
        }

        private void Refresh(ITerminalGrid grid)
        {
            if (this.gridList.Contains(grid) == true && this.ContainsIn(grid) == false)
            {
                this.Detach(grid);
            }
            else if (this.gridList.Contains(grid) == false && this.ContainsIn(grid) == true)
            {
                this.Attach(grid);
            }
        }

        private void Grid_Enabled(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid && this.ContainsIn(grid) == true)
            {
                this.Attach(grid);
            }
        }

        private void Grid_Disabled(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid && this.gridList.Contains(grid) == true)
            {
                this.Detach(grid);
            }
        }

        private bool ContainsIn(ITerminalGrid grid)
        {
            if (grid.Style is TerminalStyle style)
            {
                if (style.BehaviourList.Contains(this) == true)
                    return true;
            }
            else if (grid.BehaviourList.Contains(this) == true)
            {
                return true;
            }
            return false;
        }

        private void Grid_Validated(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid)
            {
                this.Refresh(grid);
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ITerminalGrid grid && e.PropertyName == nameof(ITerminalGrid.Style))
            {
                this.Refresh(grid);
            }
        }
    }
}
