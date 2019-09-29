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
        public TerminalRow(int index, int columnCount)
        {
            this.Index = index;
            this.Cells = new TerminalCell[columnCount];
            for (var i = 0; i < columnCount; i++)
            {
                this.Cells[i] = new TerminalCell(i, this);
            }
        }

        public int FillBackgound(int index, Vector3[] vertices, Vector2[] uvs, Color32[] colors, Rect rect)
        {
            foreach (var item in this.ValidCells)
            {
                vertices.SetVertex(index, item.BackgroundRect);
                vertices.Transform(index, rect);
                uvs.SetUV(index, item.BackgroundUV);
                if (item.BackgroundColor is Color32 color)
                    colors.SetColor(index, color);
                index += 4;
            }
            return index;
        }

        public int FillForegound(int index, Vector3[] vertices, Vector2[] uvs, Color32[] colors, Rect rect)
        {
            foreach (var item in this.ValidCells)
            {
                vertices.SetVertex(index, item.ForegroundRect);
                vertices.Transform(index, rect);
                uvs.SetUV(index, item.ForegroundUV);
                if (item.ForegroundColor is Color32 color)
                    colors.SetColor(index, color);
                index += 4;
            }
            return index;
        }

        public int Index { get; }

        public TerminalCell[] Cells { get; }

        private IEnumerable<TerminalCell> ValidCells => this.Cells.Where(item => item.Character != 0);
    }
}
