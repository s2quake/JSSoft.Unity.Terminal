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
            this.propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.propertyInfo == null)
                throw new InvalidOperationException($"property cannot found: '{propertyName}'");
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
