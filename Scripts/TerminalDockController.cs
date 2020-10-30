﻿// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
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
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    [ExecuteAlways]
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    public class TerminalDockController : UIBehaviour
    {
        [SerializeField]
        private TerminalDock dock;
        [SerializeField]
        [Range(0, 1)]
        private float ratio = 0.5f;
        [SerializeField]
        [Range(0, 10000)]
        private int length = 100;
        [SerializeField]
        private bool isRatio = true;

        public DockData Save()
        {
            return new DockData()
            {
                Dock = this.dock,
                Ratio = this.ratio,
                Length = this.length,
                IsRatio = this.isRatio,
            };
        }

        public void Load(DockData data)
        {
            this.dock = data.Dock;
            this.ratio = data.Ratio;
            this.length = data.Length;
            this.isRatio = data.IsRatio;
            this.UpdateLayout();
        }

        [FieldName(nameof(dock))]
        public TerminalDock Dock
        {
            get => this.dock;
            set
            {
                if (this.dock != value)
                {
                    this.dock = value;
                    this.UpdateLayout();
                }
            }
        }

        [FieldName(nameof(ratio))]
        public float Ratio
        {
            get => this.ratio;
            set
            {
                if (this.ratio != value)
                {
                    this.ratio = value;
                    this.UpdateLayout();
                }
            }
        }

        [FieldName(nameof(length))]
        public int Length
        {
            get => this.length;
            set
            {
                if (this.length != value)
                {
                    this.length = value;
                    this.UpdateLayout();
                }
            }
        }

        [FieldName(nameof(isRatio))]
        public bool IsRatio
        {
            get => this.isRatio;
            set
            {
                if (this.isRatio != value)
                {
                    this.isRatio = value;
                    this.UpdateLayout();
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.UpdateLayout();
        }

        internal void UpdateLayout()
        {
            var layout = this.GetComponent<HorizontalOrVerticalLayoutGroup>();
            var rect = this.GetComponent<RectTransform>();
            var size = rect.rect.size;
            if (size == Vector2.zero)
                return;
            if (this.isRatio == true)
                size *= (1.0f - this.ratio);
            else
                size = new Vector2(size.x - this.length, size.y - this.length);
            switch (this.dock)
            {
                case TerminalDock.None:
                    {
                        layout.padding = new RectOffset(0, 0, 0, 0);
                    }
                    break;
                case TerminalDock.Left:
                    {
                        layout.padding = new RectOffset(0, (int)size.x, 0, 0);
                    }
                    break;
                case TerminalDock.Top:
                    {
                        layout.padding = new RectOffset(0, 0, 0, (int)size.y);
                    }
                    break;
                case TerminalDock.Right:
                    {
                        layout.padding = new RectOffset((int)size.x, 0, 0, 0);
                    }
                    break;
                case TerminalDock.Bottom:
                    {
                        layout.padding = new RectOffset(0, 0, (int)size.y, 0);
                    }
                    break;
            }
        }

        #region DockData

        public struct DockData
        {
            public TerminalDock Dock { get; set; }

            public float Ratio { get; set; }

            public int Length { get; set; }

            public bool IsRatio { get; set; }
        }

        #endregion
    }
}
