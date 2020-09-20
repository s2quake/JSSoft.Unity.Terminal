using System.Collections;
using System.Collections.Generic;
using JSSoft.Terminal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Terminal.Scenes
{
    [ExecuteAlways]
    [RequireComponent(typeof(Animator))]
    public class GridVisibleController : UIBehaviour
    {
        public const string StateNone = "None";
        public const string StateHide = "Hide";
        public const string StateShow = "Show";

        [SerializeField]
        private float value;
        [SerializeField]
        private KeyCode keyCode = KeyCode.BackQuote;
        [SerializeField]
        private EventModifiers modifiers = EventModifiers.None;
        [SerializeField]
        private TerminalGridBase grid;

        private Animator animator;
        private float value2;
        private bool isTrigger;

        public float Value
        {
            get => this.value;
            set
            {
                this.value = value;
            }
        }

        public KeyCode KeyCode
        {
            get => this.keyCode;
            set
            {
                this.keyCode = value;
            }
        }

        public EventModifiers Modifiers
        {
            get => this.modifiers;
            set
            {
                this.modifiers = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.animator = this.GetComponent<Animator>();
            this.value2 = this.value;
            TerminalGridEvents.KeyPreview += Grid_KeyPreview;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.KeyPreview -= Grid_KeyPreview;
            base.OnDisable();
        }

        protected virtual void OnGUI()
        {
            if (Event.current is Event current && current.modifiers == this.modifiers && Input.GetKeyDown(this.keyCode) == true)
            {
                this.isTrigger = true;
            }
        }

        protected virtual void Update()
        {
            if (this.isTrigger == true)
            {
                var stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
                var currentObject = this.grid != null ? this.grid.gameObject : null;
                if (stateInfo.IsName(StateHide) != true && this.value <= 0)
                {
                    this.animator.ResetTrigger(StateShow);
                    this.animator.SetTrigger(StateHide);
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<TerminalGridBase>() is TerminalGridBase grid)
                        this.grid = grid;
                    EventSystem.current.SetSelectedGameObject(null);
                }
                else if (stateInfo.IsName(StateShow) != true && this.value >= 1)
                {
                    this.animator.ResetTrigger(StateHide);
                    this.animator.SetTrigger(StateShow);
                    this.grid = null;
                    EventSystem.current.SetSelectedGameObject(currentObject);
                }
                this.isTrigger = false;
            }
            this.UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (this.value != this.value2)
            {
                var rect = this.GetComponent<RectTransform>();
                var pos = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(pos.x, rect.rect.height * this.value);
                this.value2 = this.value;
            }
        }

        private void Grid_KeyPreview(object sender, TerminalKeyPreviewEventArgs e)
        {
            if (e.Modifiers == this.modifiers && e.KeyCode == this.keyCode && e.Handled == false)
            {
                e.Handled = true;
            }
        }
    }
}
