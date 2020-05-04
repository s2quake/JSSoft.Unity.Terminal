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
using UnityEngine.TextCore;
using JSSoft.UI;

namespace JSSoft.UI.InputHandlers
{
    public static class ITerminalGridExtensions
    {
        public static Vector2 GetPosition(this ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            return rectTransform.anchoredPosition;
        }

        public static void SetPosition(this ITerminalGrid grid, Vector2 position)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        }

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
                var index1 = cursorPosition + terminal.OutputText.Length + terminal.Prompt.Length;
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
