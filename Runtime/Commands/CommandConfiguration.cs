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

using JSSoft.Library;
using System;
using System.ComponentModel;
using System.Reflection;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandConfiguration<T> : CommandConfigurationBase
    {
        private readonly Func<object, T> getter;
        private readonly Action<object, T> setter;

        public CommandConfiguration(string name, Func<object, T> getter, Action<object, T> setter)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.getter = getter ?? throw new ArgumentNullException(nameof(getter));
            this.setter = setter ?? throw new ArgumentNullException(nameof(setter));
        }

        public override Type Type => typeof(T);

        public override string Name { get; }

        public object UserData { get; set; }

        protected override object GetValue()
        {
            return this.getter(this.UserData);
        }

        protected override void SetValue(object value)
        {
            if (value is T v)
            {
                this.setter(this.UserData, v);
            }
            else if (object.Equals(value, default(T)))
            {
                this.setter(this.UserData, default(T));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
