// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
