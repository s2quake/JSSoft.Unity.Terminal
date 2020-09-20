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
using System.Text;
using System.Threading.Tasks;
using JSSoft.Terminal;
using JSSoft.Terminal.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Library.Threading;
using UnityEngine;
using System.Collections.Generic;
using JSSoft.Library;

namespace JSSoft.Terminal.Commands
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly List<ConfigurationPropertyDescriptor> configList = new List<ConfigurationPropertyDescriptor>();

        public void Add(ConfigurationPropertyDescriptor descriptor)
        {
            foreach (var item in this.configList)
            {
                if (item.PropertyName == descriptor.PropertyName)
                    throw new ArgumentException();
            }
            this.configList.Add(descriptor);
        }

        public void Add(ConfigurationProperty property)
        {
            foreach (var item in this.configList)
            {
                if (item.PropertyName == property.PropertyName)
                    throw new ArgumentException();
            }
            this.configList.Add(property.Descriptor);
        }

        public void Remove(ConfigurationPropertyDescriptor descriptor)
        {
            this.configList.Remove(descriptor);
        }

        public void Remove(ConfigurationProperty property)
        {
            foreach (var item in this.configList)
            {
                if (item.PropertyName == property.PropertyName)
                {
                    this.configList.Remove(item);
                    break;
                }
            }
        }

        public void Remove(string propertyName)
        {
            foreach (var item in this.configList)
            {
                if (item.PropertyName == propertyName)
                {
                    this.configList.Remove(item);
                    break;
                }
            }
        }

        public IEnumerable<ConfigurationPropertyDescriptor> Configs => this.configList;

        public static ConfigurationProvider Current { get; } = new ConfigurationProvider();
    }
}
