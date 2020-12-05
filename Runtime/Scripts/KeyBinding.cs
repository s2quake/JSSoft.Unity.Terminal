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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public class KeyBinding<T> : KeyBindingBase<T> where T : class
    {
        private readonly Func<T, bool> action;
        private readonly Func<T, bool> verify;

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<T, bool> action)
            : this(modifiers, key, action, (obj) => true)
        {
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<T> action)
            : this(modifiers, key, action, (obj) => true)
        {
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Action<T> action, Func<T, bool> verify)
            : base(modifiers, key)
        {
            this.action = (t) =>
            {
                action(t);
                return true;
            };
            this.verify = verify;
        }

        public KeyBinding(EventModifiers modifiers, KeyCode key, Func<T, bool> action, Func<T, bool> verify)
            : base(modifiers, key)
        {
            this.action = action;
            this.verify = verify;
        }

        protected override bool OnVerify(T obj)
        {
            return this.verify(obj);
        }

        protected override bool OnAction(T obj)
        {
            return this.action(obj);
        }
    }
}
