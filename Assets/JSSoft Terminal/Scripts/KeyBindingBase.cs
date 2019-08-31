using System;
using UnityEngine;

namespace JSSoft.UI
{
    public abstract class KeyBindingBase<T> : IKeyBinding
    {
        protected KeyBindingBase(EventModifiers modifiers, KeyCode keyCode)
        {
            this.Modifiers = modifiers;
            this.KeyCode= keyCode;
        }

        protected abstract bool OnVerify(T obj);

        protected abstract bool OnAction(T obj);

        public EventModifiers Modifiers { get; }

        public KeyCode KeyCode { get; }

        public Type Type => typeof(T);

        #region IKeyBinding

        bool IKeyBinding.Action(object obj)
        {
            return this.OnAction((T)obj);
        }

        bool IKeyBinding.Verify(object obj)
        {
            return this.OnVerify((T)obj);
        }

        #endregion
    }
}
