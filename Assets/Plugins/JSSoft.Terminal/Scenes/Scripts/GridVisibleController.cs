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
        [SerializeField]
        private float value;
        private Animator animator;
        private GameObject selectedObject;
        private float value2;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.animator = this.GetComponent<Animator>();
            this.value2 = this.value;
            TerminalGridEvents.KeyPressed += Grid_KeyPressed;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.KeyPressed -= Grid_KeyPressed;
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (TerminalEnvironment.IsStandalone == true && Input.GetKeyDown(KeyCode.BackQuote) == true)
            {
                var stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
                var currentObject = this.selectedObject;
                if (stateInfo.IsName("None") == true || stateInfo.IsName("Show") == true)
                {
                    this.animator.ResetTrigger("Show");
                    this.animator.SetTrigger("Hide");
                    this.selectedObject = EventSystem.current.currentSelectedGameObject;
                    EventSystem.current.SetSelectedGameObject(null);
                }
                else if (stateInfo.IsName("Hide") == true)
                {
                    this.animator.ResetTrigger("Hide");
                    this.animator.SetTrigger("Show");
                    this.selectedObject = null;
                    EventSystem.current.SetSelectedGameObject(currentObject);
                }
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

        private void Grid_KeyPressed(object sender, TerminalKeyPressEventArgs e)
        {
            if (TerminalEnvironment.IsStandalone == true && e.Character == '`' && e.Handled == false)
            {
                e.Handled = true;
            }
        }
    }
}
