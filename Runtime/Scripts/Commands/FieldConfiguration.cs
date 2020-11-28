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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    public class FieldConfiguration : CommandConfigurationBase
    {
        private readonly object instance;
        private readonly FieldInfo fieldInfo;
        private RangeAttribute range;

        public FieldConfiguration(string name, object instance, FieldInfo fieldInfo)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            this.Initialize();
        }

        public FieldConfiguration(string name, object instance, string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            this.Initialize();
        }

        public FieldConfiguration(string name, Type type, string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.instance = null;
            this.fieldInfo = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.fieldInfo == null)
                throw new InvalidOperationException($"property cannot found: '{fieldName}'");
            this.Initialize();
        }

        public override Type Type => this.fieldInfo.FieldType;

        public override string Name { get; }

        protected override object GetValue()
        {
            return this.fieldInfo.GetValue(this.instance);
        }

        protected override void SetValue(object value)
        {
            this.Validate(value);
            this.fieldInfo.SetValue(this.instance, value);
        }

        private void Validate(object value)
        {
            if (this.range != null && typeof(IConvertible).IsAssignableFrom(this.Type) == true)
            {
                if (value is IConvertible convertible)
                {
                    var v = convertible.ToSingle(CultureInfo.CurrentUICulture);
                    var min = this.range.min;
                    var max = this.range.max;
                    if (v < min || v > max)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"The value is out of range. {min} <= value <= {max}");
                    }
                }
            }
        }

        private void Initialize()
        {
            this.DefaultValue = this.fieldInfo.GetValue(instance);
            if (this.fieldInfo.GetCustomAttribute(typeof(TooltipAttribute)) is TooltipAttribute tooltip)
            {
                this.Comment = tooltip.tooltip;
            }
            else if (this.fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description)
            {
                this.Comment = description.Description;
            }
            if (this.fieldInfo.GetCustomAttribute(typeof(RangeAttribute)) is RangeAttribute range)
            {
                this.range = range;
            }
        }
    }
}
