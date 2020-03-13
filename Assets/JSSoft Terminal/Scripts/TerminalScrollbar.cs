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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;

namespace JSSoft.UI
{
    class TerminalScrollbar : Scrollbar
    {
        private const string pointerOnParam = "PointerOn";

        [SerializeField]
        private TerminalGrid grid = null;
        [SerializeField]
        [Range(0, 10)]
        private float visibleTime = 1.0f;

        private new Animator animator;
        private bool isScrolling;
        private bool isPointerOn;
        private float time;

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        public float VisibleTime
        {
            get => this.visibleTime;
            set => this.visibleTime = value;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            var isPointerOn = this.gameObject == eventData.hovered.FirstOrDefault();
            if (this.isPointerOn == true && isPointerOn == false)
            {
                this.isPointerOn = false;
                this.PointerOnParam = false;
            }
            if (this.grid != null)
            {
                EventSystem.current.SetSelectedGameObject(this.grid.gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (eventData.dragging == false && this.isPointerOn == false)
            {
                this.isPointerOn = true;
                if (this.size < 1)
                {
                    this.PointerOnParam = true;
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (eventData.dragging == false && this.isPointerOn == true)
            {
                this.isPointerOn = false;
                this.PointerOnParam = false;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.onValueChanged.AddListener(VerticalScrollbar_OnValueChanged);
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
            this.animator = this.GetComponent<Animator>();
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            this.onValueChanged.RemoveListener(VerticalScrollbar_OnValueChanged);
            base.OnDisable();
        }

        protected override void Update()
        {
            base.Update();
            if (this.isPointerOn == false && this.PointerOnParam == true)
            {
                var animationState = this.animator.GetCurrentAnimatorStateInfo(0);
                if (animationState.IsName("Pointer On") == true)
                    this.PointerOnParam = false;
            }
            if (this.time >= 0 && this.PointerOnParam == false)
            {
                this.time -= Time.deltaTime;
            }
            this.animator.SetFloat("Time", this.time);
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid == this.grid)
            {
                this.UpdateScrollbarVisible();
                this.UpdateScrollbarSize();
            }
        }

        private void Grid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TerminalGrid grid != this.grid)
                return;

            var propertyName = e.PropertyName;
            if (propertyName == nameof(ITerminalGrid.VisibleIndex))
            {
                if (this.isScrolling == false)
                    this.UpdateScrollbarValue();
            }
            else if (propertyName == nameof(ITerminalGrid.Text))
            {
                this.UpdateScrollbarVisible();
                this.UpdateScrollbarSize();
            }
        }

        private void VerticalScrollbar_OnValueChanged(float arg0)
        {
            if (this.grid != null)
            {
                this.UpdateVisibleIndex();
            }
        }

        private void UpdateScrollbarVisible()
        {
            var gameObject = this.gameObject;
            var grid = this.grid;
            var isActive = grid.Rows.Count >= grid.BufferHeight;
            if (this.enabled != isActive)
            {
                this.enabled = isActive;
                if (this.targetGraphic is Graphic targetGraphic)
                {
                    targetGraphic.enabled = isActive;
                }
                if (this.handleRect is RectTransform rectTransform)
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
            var size1 = (float)Math.Max(1, grid.BufferHeight);
            var size2 = (float)Math.Max(1, grid.Rows.Count);
            var size = size1 / size2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            this.size = size;
        }

        private async void UpdateScrollbarValue()
        {
            var grid = this.grid;
            var value1 = grid.VisibleIndex;
            var value2 = (float)Math.Max(1, grid.Rows.Count - grid.BufferHeight);
            var value = value1 / value2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            this.SetValueWithoutNotify(value);
            if (this.grid.IsScrolling == true)
                this.PointerOnParam = true;
        }

        private void UpdateVisibleIndex()
        {
            var value1 = (float)this.value;
            var value2 = (float)Math.Max(1, this.grid.Rows.Count - this.grid.BufferHeight);
            var value = value1 * value2;
            this.isScrolling = true;
            this.grid.VisibleIndex = (int)value;
            this.isScrolling = false;
        }

        private bool PointerOnParam
        {
            get => this.animator.GetBool(pointerOnParam);
            set
            {
                this.time = this.visibleTime;
                this.animator.SetBool(pointerOnParam, value);
            }
        }
    }
}
