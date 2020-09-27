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
            this.Name = name;
            this.instance = instance;
            this.fieldInfo = fieldInfo;
        }

        public FieldConfiguration(string propertyName, object instance, string fieldName)
        {
            this.Name = propertyName;
            this.instance = instance;
            this.fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
