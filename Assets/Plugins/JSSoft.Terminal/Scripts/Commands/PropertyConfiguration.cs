using System;
using System.Reflection;
using JSSoft.Library;

namespace JSSoft.Terminal.Commands
{
    public class PropertyConfiguration : CommandConfigurationBase
    {
        private readonly object instance;
        private readonly PropertyInfo propertyInfo;

        public PropertyConfiguration(string name, object instance, PropertyInfo propertyInfo)
        {
            this.Name = name;
            this.instance = instance;
            this.propertyInfo = propertyInfo;
        }

        public PropertyConfiguration(string name, object instance, string propertyName)
        {
            this.Name = name;
            this.instance = instance;
            this.propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public PropertyConfiguration(string name, Type type, string propertyName)
        {
            this.Name = name;
            this.instance = null;
            this.propertyInfo = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
    }
}
