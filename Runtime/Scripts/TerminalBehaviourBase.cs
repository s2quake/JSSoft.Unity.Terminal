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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Unity.Terminal
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
