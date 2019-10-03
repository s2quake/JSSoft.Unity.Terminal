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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace JSSoft.UI
{
    public class TerminalRow
    {
        private readonly List<TerminalCell> cellList = new List<TerminalCell>();
        private Color32? backgroundColor;
        private Color32? foregroundColor;

        public TerminalRow(TerminalGrid grid, int index)
        {
            this.Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.Index = index;
            this.cellList.Capacity = grid.ColumnCount;
            for (var i = 0; i < grid.ColumnCount; i++)
            {
                this.cellList.Add(new TerminalCell(this, i));
            }
        }

        public TerminalGrid Grid { get; }

        public int Index { get; }

        public IReadOnlyList<TerminalCell> Cells => this.cellList;

        public Color32? BackgroundColor
        {
            get => this.backgroundColor ?? this.Grid.BackgroundColor;
            set => this.backgroundColor = value;
        }

        public Color32? ForegroundColor
        {
            get => this.foregroundColor ?? this.Grid.ForegroundColor;
            set => this.foregroundColor = value;
        }

        private IEnumerable<TerminalCell> ValidCells => this.Cells.Where(item => item.Character != 0);
    }
}
