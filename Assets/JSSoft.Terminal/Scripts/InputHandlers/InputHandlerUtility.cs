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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI.InputHandlers
{
    static class InputHandlerUtility
    {
        public static RangeInfo RangeToObject(ITerminalGrid grid, TerminalRange range)
        {
            var terminal = grid.Terminal;
            var beginPoint = range.BeginPoint;
            var endPoint = range.EndPoint;
            var beginIndex = grid.PointToIndex(beginPoint);
            var endIndex = grid.PointToIndex(endPoint);
            var rangeInfo = new RangeInfo();
            if (beginIndex >= 0)
            {
                rangeInfo.Start = beginIndex;
                if (endIndex < 0)
                {
                    var row = grid.Rows[endPoint.Y];
                    var lastPoint = SelectionUtility.LastPoint(row, false);
                    var text = terminal.Text;
                    endIndex = grid.PointToIndex(lastPoint);
                    if (endIndex < 0 && grid.CharacterInfos.Any() == true)
                    {
                        lastPoint = grid.CharacterInfos.Last().Point;
                        rangeInfo.End = new TerminalPoint(-1, lastPoint.Y - endPoint.Y);
                    }
                    else
                    {
                        endIndex = text.IndexOf('\n', endIndex);
                        if (endIndex < 0)
                            endIndex = text.Length;
                        endIndex++;
                        rangeInfo.End = endIndex;
                    }
                }
                else
                {
                    rangeInfo.End = endIndex;
                }
            }
            else
            {
                var row = grid.Rows[beginPoint.Y];
                var lastPoint = grid.CharacterInfos.Last().Point;
                rangeInfo.Start = new TerminalPoint(beginPoint.X, lastPoint.Y - beginPoint.Y);
                rangeInfo.End = new TerminalPoint(endPoint.X, lastPoint.Y - endPoint.Y);
            }
            return rangeInfo;
        }

        public static TerminalRange ObjectToRange(ITerminalGrid grid, RangeInfo rangeInfo)
        {
            var range = TerminalRange.Empty;
            var terminal = grid.Terminal;
            var text = terminal.Text;
            if (rangeInfo.Start is int start)
            {
                range.BeginPoint = grid.IndexToPoint(start);
                if (rangeInfo.End is int end)
                {
                    if (text.Length == end - 1)
                    {
                        var p2 = grid.IndexToPoint(end - 1);
                        p2.X = grid.BufferWidth;
                        range.EndPoint = p2;
                    }
                    else if (text[end - 1] == '\n')
                    {
                        var p2 = grid.IndexToPoint(end - 1);
                        p2.X = grid.BufferWidth;
                        range.EndPoint = p2;
                    }
                    else
                    {
                        var p2 = grid.IndexToPoint(end);
                        range.EndPoint = p2;
                    }
                }
            }
            else if (rangeInfo.Start is TerminalPoint beginPoint)
            {
                var lastPoint = grid.CharacterInfos.Last().Point;
                beginPoint.Y = lastPoint.Y - beginPoint.Y;
                if (beginPoint.X < 0)
                    beginPoint.X = grid.BufferWidth;
                range.BeginPoint = beginPoint;
            }
            if (rangeInfo.End is TerminalPoint endPoint)
            {
                var lastPoint = grid.CharacterInfos.Last().Point;
                endPoint.Y = lastPoint.Y - endPoint.Y;
                if (endPoint.X < 0)
                    endPoint.X = grid.BufferWidth;
                range.EndPoint = endPoint;
            }
            // Debug.Log($"{nameof(ObjectToRange)}: {rangeInfo} -> {range}");
            return range;
        }
    }
}
