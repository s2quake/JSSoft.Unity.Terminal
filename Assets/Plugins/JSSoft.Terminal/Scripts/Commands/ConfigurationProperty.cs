using System;
using System.Reflection;
using JSSoft.Library;

namespace JSSoft.Terminal.Commands
{
    public class ConfigurationProperty
    {
        private readonly object instance;
        private readonly FieldInfo fieldInfo;

        public ConfigurationProperty(string propertyName, object instance, FieldInfo fieldInfo)
        {
            this.PropertyName = propertyName;
            this.instance = instance;
            this.fieldInfo = fieldInfo;
            this.Descriptor = new InternalDescriptor(this);
        }

        public ConfigurationProperty(string propertyName, object instance, string fieldName)
        {
            this.PropertyName = propertyName;
            this.instance = instance;
            this.fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            this.Descriptor = new InternalDescriptor(this);
        }

        public Type PropertyType => this.fieldInfo.FieldType;

        public string PropertyName { get; }

        public string Comment { get; set; } = string.Empty;

        public object DefaultValue { get; set; } = DBNull.Value;

        public object Value
        {
            get => this.fieldInfo.GetValue(this.instance);
            set => this.fieldInfo.SetValue(this.instance, value);
        }

        public ConfigurationPropertyDescriptor Descriptor { get; }

        #region InternalDescriptor 

        class InternalDescriptor : ConfigurationPropertyDescriptor
        {
            private readonly ConfigurationProperty owner;

            public InternalDescriptor(ConfigurationProperty owner)
            {
                this.owner = owner;
            }

            public override Type PropertyType => this.owner.PropertyType;

            public override string PropertyName => this.owner.PropertyName;

            public override string Comment => this.owner.Comment;

            public override object DefaultValue => this.owner.DefaultValue;

            public override Type ScopeType => null;

            public override object Value { get => this.owner.Value; set => this.owner.Value = value; }

            protected override void OnReset()
            {
                if (this.DefaultValue != DBNull.Value)
                {
                    this.Value = this.DefaultValue;
                }
            }
        }

        #endregion
    }
}
