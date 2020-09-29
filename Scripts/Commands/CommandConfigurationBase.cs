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

namespace JSSoft.Unity.Terminal.Commands
{
    public abstract class CommandConfigurationBase : ICommandConfiguration
    {
        public abstract Type Type { get; }

        public abstract string Name { get; }

        public string Comment { get; set; } = string.Empty;

        public object DefaultValue { get; set; } = DBNull.Value;

        public object Value
        {
            get => this.GetValue();
            set
            {
                this.SetValue(value);
                this.OnChanged(EventArgs.Empty);
            }
        }

        public event EventHandler Changed;

        protected abstract void SetValue(object value);

        protected abstract object GetValue();

        protected virtual void OnChanged(EventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }
    }

    public abstract class CommandConfigurationBase<T> : ICommandConfiguration
    {
        public abstract Type Type { get; }

        public abstract string Name { get; }

        public string Comment { get; set; } = string.Empty;

        public object DefaultValue { get; set; } = DBNull.Value;

        public T Value
        {
            get => this.GetValue();
            set
            {
                this.SetValue(value);
                this.OnChanged(EventArgs.Empty);
            }
        }

        public event EventHandler Changed;

        protected abstract void SetValue(T value);

        protected abstract T GetValue();

        protected virtual void OnChanged(EventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }

        #region ICommandConfiguration

        object ICommandConfiguration.Value
        {
            get => this.Value;
            set
            {
                if (value is T v)
                {
                    this.Value = v;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        #endregion
    }
}
