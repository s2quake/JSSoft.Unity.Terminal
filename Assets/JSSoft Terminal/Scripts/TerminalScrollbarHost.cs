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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;

namespace JSSoft.UI
{
    [RequireComponent(typeof(TerminalScrollbar))]
    [ExecuteAlways]
    class TerminalScrollbarHost : UIBehaviour
    {
        [SerializeField]
        private TerminalGrid grid = null;
        [Header("Fade Settings")]
        [SerializeField]
        private bool isFadable = true;
        [SerializeField]
        [Range(0, 10)]
        private float watingTime = 1.0f;
        [SerializeField]
        [Range(0, 5)]
        private float fadingTime = 0.25f;

        private TerminalScrollbar verticalScrollbar;
        private IEnumerator fader;
        private float time = 0.0f;
        private bool isScrolling;

        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        public bool IsFadable
        {
            get => this.isFadable;
            set
            {
                this.isFadable = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.verticalScrollbar = this.GetComponent<TerminalScrollbar>();
            this.verticalScrollbar.onValueChanged.AddListener(VerticalScrollbar_OnValueChanged);
            this.verticalScrollbar.PointerUp += VerticalScrollbar_PointerUp;
            this.AttachEvent();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.verticalScrollbar.onValueChanged.RemoveListener(VerticalScrollbar_OnValueChanged);
            this.verticalScrollbar.PointerUp -= VerticalScrollbar_PointerUp;
            this.DetachEvent();
        }

        private void AttachEvent()
        {
            TerminalGridEvents.LayoutChanged += Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged += Grid_PropertyChanged;
        }

        private void DetachEvent()
        {
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
        }

        private void UpdateScrollbarVisible()
        {
            var gameObject = this.gameObject;
            var grid = this.grid;
            var isActive = grid.Rows.Count >= grid.BufferHeight;
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
            var size1 = (float)Math.Max(1, grid.BufferHeight);
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
            var value2 = (float)Math.Max(1, grid.Rows.Count - grid.BufferHeight);
            var value = value1 / value2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            this.verticalScrollbar.SetValueWithoutNotify(value);
        }

        private void UpdateVisibleIndex()
        {
            var value1 = (float)this.verticalScrollbar.value;
            var value2 = (float)Math.Max(1, this.grid.Rows.Count - this.grid.BufferHeight);
            var value = value1 * value2;
            Debug.Log(value);
            this.isScrolling = true;
            this.grid.VisibleIndex = (int)value;
            this.isScrolling = false;
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
                if (this.grid.IsScrolling == true && this.isFadable == true)
                    this.BeginFade();
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
            if (this.isFadable == true)
            {
                this.BeginFade();
            }
        }

        private void VerticalScrollbar_PointerUp(object sender, EventArgs e)
        {
            if (this.grid != null)
            {
                EventSystem.current.SetSelectedGameObject(this.grid.gameObject);
            }
        }

        private void BeginFade()
        {
            if (Application.isPlaying == false)
                return;
                this.GetComponent<Animator>().SetTrigger("FadeOut");
            // this.time = this.watingTime + this.fadingTime;
            // if (this.fader == null)
            // {
            //     this.fader = this.Fade();
            //     this.StartCoroutine(this.fader);
            // }
        }

        private IEnumerator Fade()
        {
            var handleRect = this.verticalScrollbar.handleRect;
            if (handleRect != null)
            {
                var handleImage = handleRect.GetComponent<Image>();
                var totalTime = this.watingTime + this.fadingTime;
                var color = handleImage.color;
                color.a = 1.0f;
                handleImage.color = color;

                do
                {
                    color.a = this.time > this.watingTime ? 1.0f : (this.time / this.fadingTime);
                    handleImage.color = color;
                    this.time -= Time.deltaTime;
                    yield return null;
                }
                while (this.time >= 0);
                color.a = 0.0f;
                handleImage.color = color;
            }
            this.fader = null;
        }
    }
}
