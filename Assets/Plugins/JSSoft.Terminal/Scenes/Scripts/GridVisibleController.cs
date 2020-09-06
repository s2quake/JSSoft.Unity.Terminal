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
        private Animator animator;
        private GameObject selectedObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.animator = this.GetComponent<Animator>();
            TerminalGridEvents.KeyPressed += Grid_KeyPressed;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.KeyPressed -= Grid_KeyPressed;
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (this.animator != null && Input.GetKeyDown(KeyCode.BackQuote) == true)
            {
                var stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("None") == true || stateInfo.IsName("Show") == true)
                {
                    animator.ResetTrigger("Show");
                    animator.SetTrigger("Hide");
                    this.selectedObject = EventSystem.current.currentSelectedGameObject;
                    EventSystem.current.SetSelectedGameObject(null);
                }
                else if (stateInfo.IsName("Hide") == true)
                {
                    animator.ResetTrigger("Hide");
                    animator.SetTrigger("Show");
                    EventSystem.current.SetSelectedGameObject(this.selectedObject);
                    this.selectedObject = null;
                }
            }
        }

        private void Grid_KeyPressed(object sender, TerminalKeyPressEventArgs e)
        {
            if (e.Character == '`' && e.Handled == false)
            {
                e.Handled = true;
            }
        }
    }
}
