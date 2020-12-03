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

        public bool IsEnabled { get; set; } = true;

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

        public bool IsEnabled { get; set; } = true;

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
