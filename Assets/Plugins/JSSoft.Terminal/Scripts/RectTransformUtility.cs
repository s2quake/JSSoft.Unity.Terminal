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

namespace JSSoft.Terminal
{
    public static class RectTransformUtility
    {
        public static HorizontalAlignment? GetHorizontalAlignment(RectTransform rectTransform)
        {
            var min = rectTransform.anchorMin;
            var max = rectTransform.anchorMax;
            if (min.x == max.x)
            {
                if (min.x == 0)
                {
                    return HorizontalAlignment.Left;
                }
                else if (min.x == 0.5f)
                {
                    return HorizontalAlignment.Center;
                }
                else if (min.x == 1)
                {
                    return HorizontalAlignment.Right;
                }
            }
            else
            {
                if (min.x == 0 && max.x == 1)
                {
                    return HorizontalAlignment.Stretch;
                }
            }
            return null;
        }

        public static VerticalAlignment? GetVerticalAlignment(RectTransform rectTransform)
        {
            var min = rectTransform.anchorMin;
            var max = rectTransform.anchorMax;
            if (min.y == max.y)
            {
                if (min.y == 1)
                {
                    return VerticalAlignment.Top;
                }
                else if (min.y == 0.5f)
                {
                    return VerticalAlignment.Center;
                }
                else if (min.y == 0)
                {
                    return VerticalAlignment.Bottom;
                }
            }
            else
            {
                if (min.y == 0 && max.y == 1)
                {
                    return VerticalAlignment.Stretch;
                }
            }
            return null;
        }

        public static (Vector2 min, Vector2 max) GetAnchor(HorizontalAlignment horzAlign, VerticalAlignment vertAlign)
        {
            var min = Vector2.zero;
            var max = Vector2.zero;
            switch (horzAlign)
            {
                case HorizontalAlignment.Left:
                    {
                        min.x = 0;
                        max.x = 0;
                    }
                    break;
                case HorizontalAlignment.Center:
                    {
                        min.x = 0.5f;
                        max.x = 0.5f;
                    }
                    break;
                case HorizontalAlignment.Right:
                    {
                        min.x = 1;
                        max.x = 1;
                    }
                    break;
                case HorizontalAlignment.Stretch:
                    {
                        min.x = 0;
                        max.x = 1;
                    }
                    break;
            }
            switch (vertAlign)
            {
                case VerticalAlignment.Top:
                    {
                        min.y = 1;
                        max.y = 1;
                    }
                    break;
                case VerticalAlignment.Center:
                    {
                        min.y = 0.5f;
                        max.y = 0.5f;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        min.y = 0;
                        max.y = 0;
                    }
                    break;
                case VerticalAlignment.Stretch:
                    {
                        min.y = 0;
                        max.y = 1;
                    }
                    break;
            }
            return (min, max);
        }

        public static Vector2 GetPivot(HorizontalAlignment horzAlign, VerticalAlignment vertAlign)
        {
            var pivot = Vector2.zero;
            switch (horzAlign)
            {
                case HorizontalAlignment.Left:
                    {
                        pivot.x = 0;
                    }
                    break;
                case HorizontalAlignment.Center:
                    {
                        pivot.x = 0.5f;
                    }
                    break;
                case HorizontalAlignment.Right:
                    {
                        pivot.x = 1;
                    }
                    break;
            }
            switch (vertAlign)
            {
                case VerticalAlignment.Top:
                    {
                        pivot.y = 1;
                    }
                    break;
                case VerticalAlignment.Center:
                    {
                        pivot.y = 0.5f;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        pivot.y = 0;
                    }
                    break;
            }
            return pivot;
        }

        public static Vector2 GetSize(HorizontalAlignment horzAlign, VerticalAlignment vertAlign, Vector2 size)
        {
            if (horzAlign == HorizontalAlignment.Stretch)
                size.x = 0;
            if (vertAlign == VerticalAlignment.Stretch)
                size.y = 0;
            return size;
        }

        public static Vector2 GetParentSize(RectTransform rectTransform)
        {
            if (rectTransform.parent is RectTransform parent)
            {
                return parent.sizeDelta;
            }
            return new Vector2(Screen.width, Screen.height);
        }
    }
}
