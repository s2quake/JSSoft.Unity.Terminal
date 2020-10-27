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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JSSoft.Library;
using JSSoft.Library.Commands;
using JSSoft.Library.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal.Commands
{
    // [CommandSummary(CommandStrings.DateCommand.Summary)]
    // [CommandSummary(CommandStrings.DateCommand.Summary_ko_KR, Locale = "ko-KR")]
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
                return GameObjectUtility.GetCompletions(find);
            }
            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod(Aliases = new string[] { "new" })]
        public void Create(string path, string className)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            var type = Type.GetType(className);
            if (type == null)
                throw new InvalidOperationException($"cannot found type: '{className}'");
            gameObject.AddComponent(type);
        }

        [CommandMethod(Aliases = new string[] { "rm" })]
        public void Delete(string path, int index)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            var components = gameObject.GetComponents(typeof(Component));
            var component = components[index];
            Component.DestroyImmediate(component);
        }

        [CommandMethod(Aliases = new string[] { "on" })]
        public void Activate(string path, int index)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            var components = gameObject.GetComponents(typeof(Component));
            var component = components[index];
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
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            var components = gameObject.GetComponents(typeof(Component));
            var component = components[index];
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
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            var sb = new StringBuilder();
            var components = gameObject.GetComponents(typeof(Component));
            for (var i = 0; i < components.Length; i++)
            {
                var item = components[i];
                sb.AppendLine($"{i,2}: {item.GetType()}");
            }
            this.WriteLine(sb.ToString());
        }
    }
}
