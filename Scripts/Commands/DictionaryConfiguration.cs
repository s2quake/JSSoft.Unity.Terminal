using System;
using System.Collections.Generic;
using System.Reflection;
using JSSoft.Library;

namespace JSSoft.Unity.Terminal.Commands
{
    public class DictionaryConfiguration<TKey, TValue> : CommandConfigurationBase<TValue>
    {
        private readonly IDictionary<TKey, TValue> dictionary;
        private readonly TKey key;

        public DictionaryConfiguration(string name, IDictionary<TKey, TValue> dictionary, TKey key)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.key = key;
            this.DefaultValue = dictionary[key];
        }

        public override Type Type => typeof(TValue);

        public override string Name { get; }

        public ConfigurationPropertyDescriptor Descriptor { get; }

        protected override TValue GetValue()
        {
            return this.dictionary[this.key];
        }

        protected override void SetValue(TValue value)
        {
            this.dictionary[this.key] = value;
        }
    }
}
