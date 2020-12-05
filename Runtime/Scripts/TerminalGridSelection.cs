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
using System.Collections.ObjectModel;

namespace JSSoft.Unity.Terminal
{
    class TerminalGridSelection : ObservableCollection<TerminalRange>
    {
        private readonly TerminalGrid grid;

        public TerminalGridSelection(TerminalGrid grid)
        {
            this.grid = grid;
        }

        protected override void ClearItems()
        {
            if (this.Count > 0)
                base.ClearItems();
        }

        protected override void InsertItem(int index, TerminalRange item)
        {
            if (item == TerminalRange.Empty)
                throw new ArgumentException("invalid selection", nameof(item));
            if (this.Contains(item) == true)
                throw new ArgumentException("already exists.");
            base.InsertItem(index, item);
        }
    }
}
