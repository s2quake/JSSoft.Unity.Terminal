using System;
using System.Reflection;

namespace JSSoft.Unity.Terminal.Commands
{
    public class FieldConfiguration : CommandConfigurationBase
    {
        private readonly object instance;
        private readonly FieldInfo fieldInfo;

        public FieldConfiguration(string name, object instance, FieldInfo fieldInfo)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            this.DefaultValue = fieldInfo.GetValue(instance);
        }

        public FieldConfiguration(string name, object instance, string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            this.DefaultValue = this.fieldInfo.GetValue(instance);
        }

        public FieldConfiguration(string name, Type type, string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            this.Name = name ?? throw new ArgumentNullException(nameof(name)); ;
            this.instance = null;
            this.fieldInfo = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.fieldInfo == null)
                throw new InvalidOperationException($"property cannot found: '{fieldName}'");
            this.DefaultValue = this.fieldInfo.GetValue(instance);
        }

        public override Type Type => this.fieldInfo.FieldType;

        public override string Name { get; }

        protected override object GetValue()
        {
            return this.fieldInfo.GetValue(this.instance);
        }

        protected override void SetValue(object value)
        {
            this.fieldInfo.SetValue(this.instance, value);
        }
    }
}
