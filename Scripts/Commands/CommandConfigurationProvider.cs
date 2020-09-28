﻿// MIT License
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
using System.Collections.Generic;
using System.Reflection;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandConfigurationProvider : ICommandConfigurationProvider
    {
        private readonly List<ICommandConfiguration> configList = new List<ICommandConfiguration>();

        public void Add(ICommandConfiguration config)
        {
            foreach (var item in this.configList)
            {
                if (item.Name == config.Name)
                    throw new ArgumentException();
            }
            this.configList.Add(config);
        }

        public void AddInstance(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            var instanceType = instance.GetType();
            if (Attribute.GetCustomAttribute(instanceType, typeof(CommandConfigurationAttribute)) is CommandConfigurationAttribute attr)
            {
                if (attr.Name != string.Empty)
                {
                    this.AddFields(instance, attr.Name);
                    this.AddProperties(instance, attr.Name);
                }
                else
                {
                    this.AddFields(instance, instanceType.Name);
                    this.AddProperties(instance, instanceType.Name);
                }
            }
        }

        public void RemoveInstance(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            var instanceType = instance.GetType();
            if (Attribute.GetCustomAttribute(instanceType, typeof(CommandConfigurationAttribute)) is CommandConfigurationAttribute attr)
            {
                if (attr.Name != string.Empty)
                {
                    this.RemoveFields(instance, attr.Name);
                    this.RemoveProperties(instance, attr.Name);
                }
                else
                {
                    this.RemoveFields(instance, instanceType.Name);
                    this.RemoveProperties(instance, instanceType.Name);
                }
            }
        }

        public void Remove(ICommandConfiguration config)
        {
            this.configList.Remove(config);
        }

        public void Remove(string propertyName)
        {
            foreach (var item in this.configList)
            {
                if (item.Name == propertyName)
                {
                    this.configList.Remove(item);
                    break;
                }
            }
        }

        public IEnumerable<ICommandConfiguration> Configs => this.configList;

        private void AddProperties(object instance, string rootName)
        {
            var instanceType = instance.GetType();
            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in properties)
            {
                if (Attribute.GetCustomAttribute(item, typeof(CommandConfigurationAttribute)) is CommandConfigurationAttribute attr)
                {
                    if (attr.FullName != string.Empty)
                    {
                        this.Add(new PropertyConfiguration(attr.FullName, instance, item));
                    }
                    else if (attr.Name != string.Empty)
                    {
                        this.Add(new PropertyConfiguration($"{rootName}.{attr.Name}", instance, item));
                    }
                    else
                    {
                        this.Add(new PropertyConfiguration($"{rootName}.{item.Name}", instance, item));
                    }
                }
            }
        }

        private void RemoveProperties(object instance, string rootName)
        {
            var instanceType = instance.GetType();
            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in properties)
            {
                if (Attribute.GetCustomAttribute(item, typeof(CommandConfigurationAttribute)) is CommandConfigurationAttribute attr)
                {
                    if (attr.FullName != string.Empty)
                    {
                        this.Remove(attr.FullName);
                    }
                    else if (attr.Name != string.Empty)
                    {
                        this.Remove($"{rootName}.{attr.Name}");
                    }
                    else
                    {
                        this.Remove($"{rootName}.{item.Name}");
                    }
                }
            }
        }

        private void AddFields(object instance, string rootName)
        {
            var instanceType = instance.GetType();
            var fields = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in fields)
            {
                if (Attribute.GetCustomAttribute(item, typeof(CommandConfigurationAttribute)) is CommandConfigurationAttribute attr)
                {
                    if (attr.FullName != string.Empty)
                    {
                        this.Add(new FieldConfiguration(attr.FullName, instance, item));
                    }
                    else if (attr.Name != string.Empty)
                    {
                        this.Add(new FieldConfiguration($"{rootName}.{attr.Name}", instance, item));
                    }
                    else
                    {
                        this.Add(new FieldConfiguration($"{rootName}.{item.Name}", instance, item));
                    }
                }
            }
        }

        private void RemoveFields(object instance, string rootName)
        {
            var instanceType = instance.GetType();
            var fields = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var item in fields)
            {
                if (Attribute.GetCustomAttribute(item, typeof(CommandConfigurationAttribute)) is CommandConfigurationAttribute attr)
                {
                    if (attr.FullName != string.Empty)
                    {
                        this.Remove(attr.FullName);
                    }
                    else if (attr.Name != string.Empty)
                    {
                        this.Remove($"{rootName}.{attr.Name}");
                    }
                    else
                    {
                        this.Remove($"{rootName}.{item.Name}");
                    }
                }
            }
        }
    }
}
