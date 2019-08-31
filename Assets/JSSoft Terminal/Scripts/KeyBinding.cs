using System;
using UnityEngine;

namespace JSSoft.UI
{
    public class KeyBinding : KeyBindingBase<Terminal>
    {
        private readonly Func<Terminal, bool> action;
        private readonly Func<Terminal, bool> verify;

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<Terminal, bool> action)
            : this(modifiers, key, action, (obj) => true)
        {

        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<Terminal> action)
            : this(modifiers, key, action, (obj) => true)
        {

        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<Terminal> action, Func<Terminal, bool> verify)
            : base(modifiers, key)
        {
            this.action = (t) =>
            {
                action(t);
                return true;
            };
            this.verify = verify;
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<Terminal, bool> action, Func<Terminal, bool> verify)
            : base(modifiers, key)
        {
            this.action = action;
            this.verify = verify;
        }

        protected override bool OnVerify(Terminal obj)
        {
            return this.verify(obj);
        }

        protected override bool OnAction(Terminal obj)
        {
            return this.action(obj);
        }
    }
}
