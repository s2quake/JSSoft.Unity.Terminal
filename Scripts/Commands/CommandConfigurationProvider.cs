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
using JSSoft.Unity.Terminal;
using JSSoft.Unity.Terminal.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Library.Threading;
using UnityEngine;
using System.Collections.Generic;
using JSSoft.Library;

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
    }
}
