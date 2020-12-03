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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public abstract class KeyBindingBase<T> : IKeyBinding where T : class
    {
        protected KeyBindingBase(EventModifiers modifiers, KeyCode keyCode)
        {
            this.Modifiers = modifiers;
            this.KeyCode = keyCode;
        }

        protected abstract bool OnVerify(T obj);

        protected abstract bool OnAction(T obj);

        public EventModifiers Modifiers { get; }

        public KeyCode KeyCode { get; }

        public Type Type => typeof(T);

        public bool IsPreview { get; set; }

        #region IKeyBinding

        bool IKeyBinding.Action(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (this.Type.IsAssignableFrom(obj.GetType()) == false)
                throw new ArgumentException("invalid type", nameof(obj));
            return this.OnAction((T)obj);
        }

        bool IKeyBinding.Verify(object obj)
        {
            if (obj == null)
                return false;
            if (this.Type.IsAssignableFrom(obj.GetType()) == false)
                return false;
            return this.OnVerify((T)obj);
        }

        bool IKeyBinding.IsPreview => this.IsPreview;

        #endregion
    }
}
