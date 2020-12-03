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
using UnityEngine;

namespace JSSoft.Unity.Terminal.Behaviours
{
    public class PowershellCursorBehaviour : TerminalBehaviourBase
    {
        protected override void OnAttach(ITerminalGrid grid)
        {
            grid.GotFocus += Grid_GotFocus;
            grid.LostFocus += Grid_LostFocus;
            if (Application.isPlaying == true)
                grid.IsCursorVisible = grid.IsFocused;
            else
                grid.IsCursorVisible = true;
        }

        protected override void OnDetach(ITerminalGrid grid)
        {
            grid.GotFocus -= Grid_GotFocus;
            grid.LostFocus -= Grid_LostFocus;
            grid.IsCursorVisible = true;
        }

        private void Grid_GotFocus(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid && Application.isPlaying == true)
            {
                grid.IsCursorVisible = true;
            }
        }

        private void Grid_LostFocus(object sender, EventArgs e)
        {
            if (sender is ITerminalGrid grid && Application.isPlaying == true)
            {
                grid.IsCursorVisible = false;
            }
        }
    }
}