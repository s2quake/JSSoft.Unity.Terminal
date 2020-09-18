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
        }

        protected override void OnDisable()
        {
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
        }
    }
}
