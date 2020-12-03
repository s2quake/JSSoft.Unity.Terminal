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
