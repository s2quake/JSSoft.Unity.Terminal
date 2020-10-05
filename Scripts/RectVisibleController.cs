﻿using System.Collections;
using System.Collections.Generic;
using JSSoft.Unity.Terminal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    [ExecuteAlways]
    [RequireComponent(typeof(Animator))]
    public class RectVisibleController : UIBehaviour, IValidatable
    {
        public const string StateNone = "None";
        public const string StateHide = "Hide";
        public const string StateShow = "Show";

        [SerializeField]
        [Range(0, 1)]
        private float value;
        [SerializeField]
        private Vector2 position;
        [SerializeField]
        private RectVisibleDirection direction = RectVisibleDirection.Top;
        [SerializeField]
        private TerminalGridBase grid = null;

        private Animator animator;
        private float value2;
        private bool isTrigger;

        public void Show()
        {
            this.value = 0;
            this.UpdatePosition(true);
        }

        public void Hide()
        {
            this.value = 1;
            this.UpdatePosition(true);
        }

        public void ResetPosition()
        {
            this.position = this.GetComponent<RectTransform>().anchoredPosition;
            this.value = 0;
            this.UpdatePosition(true);
        }

        public void Toggle()
        {
            if (this.isTrigger == false)
            {
                this.isTrigger = true;
            }
        }

        public bool? IsVisible
        {
            get
            {
                if (this.value == 0)
                    return true;
                else if (this.value == 1)
                    return false;
                return null;
            }
        }

        [FieldName(nameof(direction))]
        public RectVisibleDirection Direction
        {
            get => this.direction;
            set
            {
                this.direction = value;
                this.UpdatePosition(true);
            }
        }

        [FieldName(nameof(grid))]
        public TerminalGridBase Grid
        {
            get => this.grid;
            set => this.grid = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.animator = this.GetComponent<Animator>();
            this.value2 = float.MinValue;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (Application.isPlaying == false)
                return;
            var stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
            if (this.isTrigger == true && stateInfo.normalizedTime >= stateInfo.length)
            {
                if (stateInfo.IsName(StateHide) != true && this.value <= 0)
                {
                    this.animator.ResetTrigger(StateShow);
                    this.animator.SetTrigger(StateHide);
                    if (EventSystem.current.currentSelectedGameObject != null)
                        this.grid = EventSystem.current.currentSelectedGameObject.GetComponent<TerminalGridBase>();
                    EventSystem.current.SetSelectedGameObject(null);
                }
                else if (stateInfo.IsName(StateShow) != true && this.value >= 1)
                {
                    this.animator.ResetTrigger(StateHide);
                    this.animator.SetTrigger(StateShow);
                    if (this.grid != null)
                        EventSystem.current.SetSelectedGameObject(this.grid.gameObject);
                }
                this.isTrigger = false;
            }
            this.UpdatePosition();
        }

        private void UpdatePosition()
        {
            this.UpdatePosition(false);
        }

        private void UpdatePosition(bool force)
        {
            if (this.value != this.value2 || force == true)
            {
                var rect = this.GetComponent<RectTransform>();
                if (rect != null)
                {
                    var pos = this.position;
                    var direction = this.direction;
                    var value = this.value;
                    switch (direction)
                    {
                        case RectVisibleDirection.Left:
                            {
                                pos = new Vector2(-rect.rect.width * value, pos.y);
                            }
                            break;
                        case RectVisibleDirection.Top:
                            {
                                pos = new Vector2(pos.x, rect.rect.height * value);
                            }
                            break;
                        case RectVisibleDirection.Right:
                            {
                                pos = new Vector2(rect.rect.width * value, pos.y);
                            }
                            break;
                        case RectVisibleDirection.Bottom:
                            {
                                pos = new Vector2(pos.x, -rect.rect.height * value);
                            }
                            break;
                    }
                    rect.anchoredPosition = pos;
                    this.value2 = value;
                }
            }
        }

        internal void Validate()
        {
            this.UpdatePosition(true);
        }

        #region IValidatable

        void IValidatable.Validate()
        {
            this.Validate();
        }

        #endregion
    }
}