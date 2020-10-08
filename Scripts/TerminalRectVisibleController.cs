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
            if (Event.current is Event current && current.type == EventType.KeyDown && current.modifiers == this.modifiers && Input.GetKeyDown(this.keyCode) == true)
            {
                if (this.Grid != null && this.Grid.IsFocused == false && this.IsVisible == true)
                {
                    this.Grid.Focus();
                }
                else if (this.CanToggle == true)
                {
                    this.Toggle();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            TerminalGridEvents.KeyDown += Grid_KeyDown;
        }

        protected override void OnDisable()
        {
            TerminalGridEvents.KeyDown -= Grid_KeyDown;
            base.OnDisable();
        }

        private void Grid_KeyDown(object sender, TerminalKeyDownEventArgs e)
        {
            if (e.Modifiers == this.modifiers && e.KeyCode == this.keyCode && e.Handled == false)
            {
                e.Handled = true;
            }
        }
    }
}
