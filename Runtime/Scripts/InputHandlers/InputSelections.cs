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
using System.Collections.Specialized;
using System.ComponentModel;

namespace JSSoft.Unity.Terminal.InputHandlers
{
    class InputSelections : IDisposable
    {
        private readonly List<TerminalRange> selections = new List<TerminalRange>();
        private readonly Dictionary<TerminalRange, RangeInfo> objByRange = new Dictionary<TerminalRange, RangeInfo>();
        private readonly ITerminalGrid grid;

        public InputSelections(ITerminalGrid grid)
        {
            this.grid = grid;
            this.Grid.SelectionChanged += Grid_SelectionChanged;
            this.Grid.PropertyChanged += Grid_PropertyChanged;
        }

        public void Dispose()
        {
            this.Grid.SelectionChanged -= Grid_SelectionChanged;
            this.Grid.PropertyChanged -= Grid_PropertyChanged;
        }

        public ITerminalGrid Grid => this.grid;

        private void Grid_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            var range = (TerminalRange)item;
                            var obj = InputHandlerUtility.RangeToObject(this.grid, range);
                            this.selections.Add(range);
                            this.objByRange.Add(range, obj);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            var range = (TerminalRange)item;
                            this.selections.Remove(range);
                            this.objByRange.Remove(range);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var oldItem = (TerminalRange)e.OldItems[i];
                            var newItem = (TerminalRange)e.NewItems[i];
                            var obj = this.objByRange[oldItem];
                            var index = this.selections.IndexOf(oldItem);
                            this.selections[index] = newItem;
                            this.objByRange.Remove(oldItem);
                            this.objByRange.Add(newItem, obj);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var item = this.selections[e.OldStartingIndex];
                        this.selections.RemoveAt(e.OldStartingIndex);
                        this.selections.Insert(e.NewStartingIndex, item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.selections.Clear();
                        this.objByRange.Clear();
                    }
                    break;
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITerminalGrid.BufferWidth):
                case nameof(ITerminalGrid.BufferHeight):
                    {
                        this.RangeToSelection();
                    }
                    break;
            }
        }

        private void RangeToSelection()
        {
            var selectionList = new List<TerminalRange>(this.objByRange.Count);
            var objByRange = new Dictionary<TerminalRange, RangeInfo>(this.objByRange.Count);
            foreach (var item in this.objByRange.Values)
            {
                var range = InputHandlerUtility.ObjectToRange(this.grid, item);
                selectionList.Add(range);
            }
            foreach (var item in selectionList)
            {
                var range = InputHandlerUtility.RangeToObject(this.grid, item);
                objByRange.Add(item, range);
            }

            this.Grid.SelectionChanged -= Grid_SelectionChanged;
            this.Grid.Selections.Clear();
            this.objByRange.Clear();
            foreach (var item in selectionList)
            {
                this.Grid.Selections.Add(item);
            }
            foreach (var item in objByRange)
            {
                this.objByRange.Add(item.Key, item.Value);
            }
            this.Grid.SelectionChanged += Grid_SelectionChanged;
        }
    }
}
