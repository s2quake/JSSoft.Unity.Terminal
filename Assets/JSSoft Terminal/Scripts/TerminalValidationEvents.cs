// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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
using System.ComponentModel;

namespace JSSoft.UI
{
    public static class TerminalValidationEvents
    {
        private static readonly HashSet<INotifyValidated> objs = new HashSet<INotifyValidated>();

        public static void Register(INotifyValidated obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (objs.Contains(obj) == true)
                throw new ArgumentException($"{nameof(obj)} is already exists.");
            objs.Add(obj);
            obj.Validated += Object_Validated;
            obj.PropertyChanged += Object_PropertyChanged;
        }

        public static void Unregister(INotifyValidated obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (objs.Contains(obj) == false)
                throw new ArgumentException($"{nameof(obj)} does not exists.");
            obj.Validated -= Object_Validated;
            obj.PropertyChanged -= Object_PropertyChanged;
            objs.Remove(obj);
        }

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Object_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

#if UNITY_EDITOR
        internal static void InvokeValidatedEvent(INotifyValidated obj, EventArgs e)
        {
            if (objs.Contains(obj) == false)
                throw new ArgumentException($"{nameof(obj)} does not exists.");
            Validated?.Invoke(obj, e);
        }
#endif
    }
}
