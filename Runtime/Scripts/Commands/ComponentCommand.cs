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
using JSSoft.Library.Commands;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class ComponentCommand : TerminalCommandMethodBase
    {
        public ComponentCommand(ITerminal terminal)
            : base(terminal, new string[] { "comp" })
        {
        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            if (memberDescriptor.DescriptorName == "path")
            {
                try
                {
                    return GameObjectUtility.GetPathCompletions(find);
                }
                catch
                {
                    return null;
                }
            }
            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod]
        public void Add(string path, string componentType)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            var type = Type.GetType(componentType);
            if (type == null)
                throw new InvalidOperationException($"cannot found type: '{componentType}'");
            gameObject.AddComponent(type);
        }

        [CommandMethod(Aliases = new string[] { "rm" })]
        public void Remove(string path, int index)
        {
            var component = GameObjectUtility.GetComponent(path, index);
            Component.DestroyImmediate(component);
        }

        [CommandMethod(Aliases = new string[] { "on" })]
        public void Activate(string path, int index)
        {
            var component = GameObjectUtility.GetComponent(path, index);
            if (component is Behaviour behaviour)
            {
                behaviour.enabled = true;
            }
            else
            {
                throw new InvalidOperationException("component cannot be enabled.");
            }
        }

        [CommandMethod(Aliases = new string[] { "off" })]
        public void Deactivate(string path, int index)
        {
            var component = GameObjectUtility.GetComponent(path, index);
            if (component is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
            else
            {
                throw new InvalidOperationException("component cannot be enabled.");
            }
        }

        [CommandMethod("list", Aliases = new string[] { "ls" })]
        public void ShowList(string path = "")
        {
            var sb = new StringBuilder();
            var components = GameObjectUtility.GetComponents(path);
            for (var i = 0; i < components.Length; i++)
            {
                var item = components[i];
                sb.AppendLine($"{i,2}: {item.GetType()}");
            }
            this.WriteLine(sb.ToString());
        }
    }
}
