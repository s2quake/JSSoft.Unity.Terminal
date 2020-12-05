﻿////////////////////////////////////////////////////////////////////////////////
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
using System.Collections.Specialized;
using System.ComponentModel;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalGridEvents
    {
        private static readonly HashSet<ITerminalGrid> grids = new HashSet<ITerminalGrid>();

        public static void Register(ITerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grids.Contains(grid) == true)
                throw new ArgumentException($"{nameof(grid)} is already exists.");
            grids.Add(grid);
            grid.LayoutChanged += Grid_LayoutChanged;
            grid.SelectionChanged += Grid_SelectionChanged;
            grid.GotFocus += Grid_GotFocus;
            grid.LostFocus += Grid_LostFocus;
            grid.Validated += Grid_Validated;
            grid.PropertyChanged += Grid_PropertyChanged;
            grid.Enabled += Grid_Enabled;
            grid.Disabled += Grid_Disabled;
            grid.PreviewKeyDown += Grid_PreviewKeyDown;
            grid.KeyDown += Grid_KeyDown;
            grid.KeyPress += Grid_KeyPress;
        }

        public static void Unregister(ITerminalGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));
            if (grids.Contains(grid) == false)
                throw new ArgumentException($"{nameof(grid)} does not exists.");
            grid.LayoutChanged -= Grid_LayoutChanged;
            grid.SelectionChanged -= Grid_SelectionChanged;
            grid.GotFocus -= Grid_GotFocus;
            grid.LostFocus -= Grid_LostFocus;
            grid.Validated -= Grid_Validated;
            grid.PropertyChanged -= Grid_PropertyChanged;
            grid.Enabled -= Grid_Enabled;
            grid.Disabled -= Grid_Disabled;
            grid.PreviewKeyDown -= Grid_PreviewKeyDown;
            grid.KeyDown -= Grid_KeyDown;
            grid.KeyPress -= Grid_KeyPress;
            grids.Remove(grid);
        }

        public static event EventHandler LayoutChanged;

        public static event NotifyCollectionChangedEventHandler SelectionChanged;

        public static event EventHandler GotFocus;

        public static event EventHandler LostFocus;

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        public static event EventHandler Enabled;

        public static event EventHandler Disabled;

        public static event EventHandler<TerminalKeyDownEventArgs> PreviewKeyDown;

        public static event EventHandler<TerminalKeyDownEventArgs> KeyDown;

        public static event EventHandler<TerminalKeyPressEventArgs> KeyPress;

        private static void Grid_LayoutChanged(object sender, EventArgs e)
        {
            LayoutChanged?.Invoke(sender, e);
        }

        private static void Grid_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        private static void Grid_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(sender, e);
        }

        private static void Grid_LostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(sender, e);
        }

        private static void Grid_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private static void Grid_Enabled(object sender, EventArgs e)
        {
            Enabled?.Invoke(sender, e);
        }

        private static void Grid_Disabled(object sender, EventArgs e)
        {
            Disabled?.Invoke(sender, e);
        }

        private static void Grid_PreviewKeyDown(object sender, TerminalKeyDownEventArgs e)
        {
            PreviewKeyDown?.Invoke(sender, e);
        }

        private static void Grid_KeyDown(object sender, TerminalKeyDownEventArgs e)
        {
            KeyDown?.Invoke(sender, e);
        }

        private static void Grid_KeyPress(object sender, TerminalKeyPressEventArgs e)
        {
            KeyPress?.Invoke(sender, e);
        }
    }
}
