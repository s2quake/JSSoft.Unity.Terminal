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

using UnityEngine;

namespace JSSoft.Unity.Terminal.InputHandlers
{
    public static class ITerminalGridExtensions
    {
        public static void SetCommand(this ITerminalGrid grid, string command)
        {
            var terminal = grid.Terminal;
            terminal.Command = command;
        }

        public static void SelectCommand(this ITerminalGrid grid, RangeInt selection)
        {
            var terminal = grid.Terminal;
            if (selection.length == 0)
            {
                terminal.CursorPosition = selection.start;
            }
            else
            {
                var cursorPosition = selection.start;
                var index1 = cursorPosition + Terminal.CombineLength(terminal.OutputText, terminal.ProgressText, terminal.Prompt);
                var index2 = index1 + selection.length;
                var point1 = grid.IndexToPoint(index1);
                var point2 = grid.IndexToPoint(index2);
                terminal.CursorPosition = selection.start;
                grid.Selections.Clear();
                grid.Selections.Add(new TerminalRange(point1, point2));
            }
        }
    }
}
