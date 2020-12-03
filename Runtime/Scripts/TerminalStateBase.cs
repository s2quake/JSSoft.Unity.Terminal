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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    public abstract class TerminalStateBase : UIBehaviour
    {
        private readonly static Dictionary<string, object> dataByName = new Dictionary<string, object>();

        [SerializeField]
        private string stateName = string.Empty;

        protected TerminalStateBase()
        {

        }

        protected TerminalStateBase(string stateName)
        {
            if (stateName == null)
                new ArgumentNullException(nameof(stateName));
            this.stateName = stateName;
        }

        public string StateName
        {
            get => this.stateName;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.stateName != value)
                {
                    this.stateName = value;
                }
            }
        }

        public void SetState(object obj)
        {
            if (this.stateName != string.Empty)
                dataByName[this.stateName] = obj;
        }

        public T GetState<T>()
        {
            if (this.stateName != string.Empty)
            {
                var state = dataByName[this.stateName];
                return (T)state;
            }
            return default;
        }

        public bool TryGetState<T>(out T state)
        {
            if (this.ContainsState() == false)
            {
                state = default;
                return false;
            }
            else
            {
                state = this.GetState<T>();
                return true;
            }
        }

        public bool ContainsState()
        {
            if (this.stateName != string.Empty)
                return dataByName.ContainsKey(this.stateName);
            return false;
        }

        public void ResetState()
        {
            if (this.stateName != string.Empty && dataByName.ContainsKey(this.stateName))
            {
                dataByName.Remove(this.stateName);
            }
        }

    }
}
