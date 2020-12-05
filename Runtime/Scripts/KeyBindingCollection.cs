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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public class KeyBindingCollection : IKeyBindingCollection
    {
        private readonly Dictionary<string, IKeyBinding> itemByKey = new Dictionary<string, IKeyBinding>();

        public KeyBindingCollection(string name, IKeyBindingCollection bindings)
            : this(name)
        {
            this.BaseBindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
        }

        public KeyBindingCollection(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            return this.Name ?? base.ToString();
        }

        public void Add(IKeyBinding item)
        {
            var key = $"{item.Modifiers}+{item.KeyCode}+{item.IsPreview}";
            this.itemByKey.Add(key, item);
        }

        public bool Process(object obj, EventModifiers modifiers, KeyCode keyCode, bool isPreview)
        {
            var key = $"{modifiers}+{keyCode}+{isPreview}";
            if (this.itemByKey.ContainsKey(key) == true)
            {
                var binding = this.itemByKey[key];
                if (binding.Verify(obj) == true && binding.Action(obj) == true)
                    return true;
            }
            if (this.BaseBindings != null && this.BaseBindings.Process(obj, modifiers, keyCode, isPreview) == true)
            {
                return true;
            }
            return false;
        }

        public int Count => this.itemByKey.Count;

        public IKeyBindingCollection BaseBindings { get; }

        public string Name { get; }

        #region IEnumerable

        IEnumerator<IKeyBinding> IEnumerable<IKeyBinding>.GetEnumerator()
        {
            foreach (var item in this.itemByKey)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.itemByKey)
            {
                yield return item.Value;
            }
        }

        #endregion
    }
}
