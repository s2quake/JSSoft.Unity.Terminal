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
using UnityEngine;
using UnityEngine.Events;

namespace JSSoft.UI
{
    [RequireComponent(typeof(TerminalGrid))]
    public class TerminalOrientationBehaviour : MonoBehaviour
    {
        [SerializeField]
        private OrientationChangedEvent changed;
        private TerminalGrid grid;
        private ScreenOrientation orientation;

        public static bool IsPortait(ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.Portrait:
                case ScreenOrientation.PortraitUpsideDown:
                    return true;
                case ScreenOrientation.Landscape:
                case ScreenOrientation.LandscapeRight:
                    return false;
                default:
                    throw new ArgumentException($"{orientation}: invalid orientation", nameof(orientation));
            }
        }

        public void Start()
        {
            if (this.changed == null)
            {
                this.changed = new OrientationChangedEvent();
            }
        }

        public void OnEnable()
        {
            this.orientation = Screen.orientation;
            this.grid = this.GetComponent<TerminalGrid>();
        }

        public void OnDisable()
        {
            this.grid = null;
        }

        public void Update()
        {
            if (Screen.orientation != this.orientation && this.grid != null)
            {
                var orientation = this.orientation;
                this.orientation = Screen.orientation;
                this.changed?.Invoke(orientation, this.orientation, IsPortait(orientation) != IsPortait(this.orientation));
            }
        }

        public ScreenOrientation Orientation => this.orientation;

        public OrientationChangedEvent Changed
        {
            get => this.changed;
            set
            {
                if (this.changed != value)
                {
                    this.changed = value;
                }
            }
        }

        private void UpdateOrientation(ScreenOrientation oldValue, ScreenOrientation newValue)
        {
            if (IsPortait(oldValue) != IsPortait(newValue))
            {
                var rectangle = this.grid.Rectangle;
                var padding = this.grid.Padding;
                var width = (int)(rectangle.width - (padding.Left + padding.Right));
                var height = (int)(rectangle.height - (padding.Top + padding.Bottom));
                var itemWidth = TerminalGridUtility.GetItemWidth(this.grid);
                var itemHeight = TerminalGridUtility.GetItemHeight(this.grid);
                this.grid.BufferWidth = height / itemWidth;
                this.grid.BufferHeight = width / itemHeight;
            }
        }

        /// <summary>
        /// void Callback(ScreenOrientation oldValue, ScreenOrientation newValue, bool isRotated)
        /// </summary>
        [Serializable]
        public class OrientationChangedEvent : UnityEvent<ScreenOrientation, ScreenOrientation, bool>
        {
        }
    }
}
