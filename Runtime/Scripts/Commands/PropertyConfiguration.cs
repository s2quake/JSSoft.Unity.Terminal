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

using JSSoft.Library;
using System;
using System.ComponentModel;
using System.Reflection;

namespace JSSoft.Unity.Terminal.Commands
{
    public class PropertyConfiguration : CommandConfigurationBase
    {
        private readonly object instance;
        private readonly PropertyInfo propertyInfo;

        public PropertyConfiguration(string name, object instance, PropertyInfo propertyInfo)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            this.Initialize();
        }

        public PropertyConfiguration(string name, object instance, string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (this.propertyInfo == null)
                throw new InvalidOperationException($"Property not found or not public.: '{propertyName}'");
            if (this.propertyInfo.CanRead == false)
                throw new InvalidOperationException($"Property cannot read.: '{propertyName}'");
            if (this.propertyInfo.CanWrite == false)
                throw new InvalidOperationException($"Property cannot write.: '{propertyName}'");
            this.Initialize();
        }

        [Obsolete("Failed to get static property information from ios.")]
        public PropertyConfiguration(string name, Type type, string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = null;
            this.propertyInfo = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (this.propertyInfo == null)
                throw new InvalidOperationException($"property cannot found: '{propertyName}'");
            this.Initialize();
        }

        public override Type Type => this.propertyInfo.PropertyType;

        public override string Name { get; }

        protected override object GetValue()
        {
            return this.propertyInfo.GetValue(this.instance);
        }

        protected override void SetValue(object value)
        {
            this.propertyInfo.SetValue(this.instance, value);
        }
        private void Initialize()
        {
            this.DefaultValue = this.propertyInfo.GetValue(instance);
            if (this.propertyInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description)
            {
                this.Comment = description.Description;
            }
        }
    }
}
