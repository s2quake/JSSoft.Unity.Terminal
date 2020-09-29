using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public class TerminalRectVisibleController : RectVisibleController
    {
        [SerializeField]
        private KeyCode keyCode = KeyCode.BackQuote;
        [SerializeField]
        private EventModifiers modifiers = EventModifiers.Control;

        [FieldName(nameof(keyCode))]
        public KeyCode KeyCode
        {
            get => this.keyCode;
            set
            {
                this.keyCode = value;
            }
        }

        [FieldName(nameof(modifiers))]
        public EventModifiers Modifiers
        {
            get => this.modifiers;
            set
            {
                this.modifiers = value;
            }
        }

        protected virtual void OnGUI()
        {
            if (Event.current is Event current && current.modifiers == this.modifiers && Input.GetKeyDown(this.keyCode) == true && this.IsVisible != null)
            {
                this.Toggle();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.KeyPreview += Grid_KeyPreview;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.KeyPreview -= Grid_KeyPreview;
            base.OnDisable();
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
