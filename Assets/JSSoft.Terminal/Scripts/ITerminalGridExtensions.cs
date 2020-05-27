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

using UnityEngine;

namespace JSSoft.UI
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

        public static Rect GetRect(this ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var canvas = gameObject.GetComponentInParent<Canvas>();
            var pixelRect = canvas.pixelRect;
            var worldCorners = GetWorldCorners(grid);
            var width = worldCorners[2].x - worldCorners[0].x;
            var height = worldCorners[2].y - worldCorners[0].y;
            var x = worldCorners[0].x;
            var y = pixelRect.height - worldCorners[2].y;
            return new Rect(x, y, width, height);
        }

        private static Vector3[] GetWorldCorners(ITerminalGrid grid)
        {
            var gameObject = grid.GameObject;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var items = new Vector3[4];
            rectTransform.GetWorldCorners(items);
            return items;
        }
    }
}
