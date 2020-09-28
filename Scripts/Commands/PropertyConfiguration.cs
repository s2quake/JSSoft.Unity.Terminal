using System;
using System.Reflection;
using JSSoft.Library;

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
            this.DefaultValue = propertyInfo.GetValue(instance);
        }

        public PropertyConfiguration(string name, object instance, string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.propertyInfo == null)
                throw new InvalidOperationException($"property cannot found: '{propertyName}'");
            this.DefaultValue = this.propertyInfo.GetValue(instance);
        }

        public PropertyConfiguration(string name, Type type, string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            this.Name = name ?? throw new ArgumentNullException(nameof(name)); ;
            this.instance = null;
            this.propertyInfo = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.propertyInfo == null)
                throw new InvalidOperationException($"property cannot found: '{propertyName}'");
            this.DefaultValue = this.propertyInfo.GetValue(instance);
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
