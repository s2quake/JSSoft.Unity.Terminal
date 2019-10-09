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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.ObjectModel;
using UnityEngine.TextCore;
using System.Threading.Tasks;

namespace JSSoft.UI
{
    [RequireComponent(typeof(Scrollbar))]
    [ExecuteAlways]
    public class TerminalScrollbar : UIBehaviour
    {
        [SerializeField]
        private TerminalGrid grid = null;
        private Scrollbar verticalScrollbar;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.verticalScrollbar = this.GetComponent<Scrollbar>();
            this.verticalScrollbar.onValueChanged.AddListener(VerticalScrollbar_OnValueChanged);
            this.AttachEvent();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.verticalScrollbar.onValueChanged.RemoveListener(VerticalScrollbar_OnValueChanged);
            this.DetachEvent();
        }

        private void AttachEvent()
        {
            if (this.grid != null)
            {
                this.grid.TextChanged += TerminalGrid_TextChanged;
                this.grid.LayoutChanged += TerminalGrid_LayoutChanged;
                this.grid.VisibleIndexChanged += TerminalGrid_VisibleIndexChanged;
            }
        }

        private void DetachEvent()
        {
            if (this.grid != null)
            {
                this.grid.TextChanged -= TerminalGrid_TextChanged;
                this.grid.LayoutChanged -= TerminalGrid_LayoutChanged;
                this.grid.VisibleIndexChanged -= TerminalGrid_VisibleIndexChanged;
            }
        }

        private void UpdateScrollbarVisible()
        {
            var gameObject = this.gameObject;
            var grid = this.grid;
            var isActive = grid.Rows.Count >= grid.RowCount;
            if (this.verticalScrollbar.enabled != isActive)
            {
                this.verticalScrollbar.enabled = isActive;
                if (this.verticalScrollbar.targetGraphic is Graphic targetGraphic)
                {
                    targetGraphic.enabled = isActive;
                }
                if (this.verticalScrollbar.handleRect is RectTransform rectTransform)
                {
                    if (rectTransform.GetComponent<Image>() is Image image)
                    {
                        image.enabled = isActive;
                    }
                }
            }
        }

        private async void UpdateScrollbarSize()
        {
            var size1 = (float)Math.Max(1, grid.RowCount);
            var size2 = (float)Math.Max(1, grid.Rows.Count);
            var size = size1 / size2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            this.verticalScrollbar.size = size;
        }

        private async void UpdateScrollbarValue()
        {
            var grid = this.grid;
            var value1 = grid.VisibleIndex;
            var value2 = (float)Math.Max(1, grid.Rows.Count - grid.RowCount);
            var value = value1 / value2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            this.verticalScrollbar.SetValueWithoutNotify(value);
        }

        private void TerminalGrid_TextChanged(object sender, EventArgs e)
        {
            this.UpdateScrollbarVisible();
            this.UpdateScrollbarSize();
        }

        private void TerminalGrid_LayoutChanged(object sender, EventArgs e)
        {
            this.UpdateScrollbarVisible();
            this.UpdateScrollbarSize();
        }

        private void TerminalGrid_VisibleIndexChanged(object sender, EventArgs e)
        {
            this.UpdateScrollbarValue();
        }

        private void VerticalScrollbar_OnValueChanged(float arg0)
        {
            if (this.grid != null)
            {
                var value1 = (float)this.verticalScrollbar.value;
                var value2 = (float)Math.Max(1, this.grid.Rows.Count - this.grid.RowCount);
                this.grid.VisibleIndex = (int)(value1 * value2);
            }
        }
    }
}
