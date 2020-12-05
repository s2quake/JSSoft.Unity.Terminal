////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JSSoft.Unity.Terminal
{
    [RequireComponent(typeof(Animator))]
    public class TerminalScrollbar : Scrollbar
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

        [FieldName(nameof(grid))]
        public TerminalGrid Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        [FieldName(nameof(visibleTime))]
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
            TerminalGridEvents.Validated += Grid_Validated;
            this.animator = this.GetComponent<Animator>();
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.LayoutChanged -= Grid_LayoutChanged;
            TerminalGridEvents.PropertyChanged -= Grid_PropertyChanged;
            TerminalGridEvents.Validated -= Grid_Validated;
            this.onValueChanged.RemoveListener(VerticalScrollbar_OnValueChanged);
            base.OnDisable();
        }

#if UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
        protected override void Update()
#else
        protected virtual void Update()
#endif
        {
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
            if (this.animator.isActiveAndEnabled == true && Application.isPlaying == true)
            {
                this.animator.SetFloat("Time", this.time);
            }
        }

        private void Grid_LayoutChanged(object sender, EventArgs e)
        {
            if (sender is TerminalGrid grid && grid == this.grid)
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
            else if (propertyName == nameof(ITerminalGrid.IsScrollForwardEnabled))
            {
                this.UpdateScrollbarSize();
            }
            else if (propertyName == nameof(ITerminalGrid.Text))
            {
                this.UpdateScrollbarVisible();
                this.UpdateScrollbarSize();
            }
        }

        private void Grid_Validated(object sender, EventArgs e)
        {
            if (this.grid != null)
            {
                this.UpdateScrollbarSize();
                this.UpdateScrollbarValue();
            }
        }

        private void VerticalScrollbar_OnValueChanged(float arg0)
        {
            if (this.grid != null && this.grid.IsScrolling == false)
            {
                this.UpdateVisibleIndex();
            }
        }

        private void UpdateScrollbarVisible()
        {
            var gameObject = this.gameObject;
            var grid = this.grid;
            var isActive = grid.MaxBufferHeight >= grid.BufferHeight;
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
            var size1 = (float)this.grid.BufferHeight;
            var size2 = (float)this.grid.MaximumVisibleIndex - this.grid.MinimumVisibleIndex + this.grid.BufferHeight;
            var size = size1 / size2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            if (this.size != size)
                this.size = size;
        }

        private async void UpdateScrollbarValue()
        {
            var value1 = (float)this.grid.VisibleIndex - this.grid.MinimumVisibleIndex;
            var value2 = (float)Math.Max(1, this.grid.MaximumVisibleIndex - this.grid.MinimumVisibleIndex);
            var value = value1 / value2;
            if (Application.isPlaying == false)
                await Task.Delay(1);
            this.value = value;
            if (this.grid.IsScrolling == true)
                this.PointerOnParam = true;
        }

        private void UpdateVisibleIndex()
        {
            var value1 = (float)this.value;
            var value2 = (float)(this.grid.MaximumVisibleIndex - this.grid.MinimumVisibleIndex);
            var value = value1 * value2;
            this.isScrolling = true;
            try
            {
                this.grid.VisibleIndex = (int)value + this.grid.MinimumVisibleIndex;
            }
            catch
            {
                
            }
            this.isScrolling = false;
        }

        private bool PointerOnParam
        {
            get
            {
                if (Application.isPlaying == true)
                    return this.animator.GetBool(pointerOnParam);
                return false;
            }
            set
            {
                this.time = this.visibleTime;
                this.animator.SetBool(pointerOnParam, value);
            }
        }
    }
}
