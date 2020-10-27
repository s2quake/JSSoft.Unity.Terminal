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
    public class GameObjectCommand : TerminalCommandMethodBase
    {
        public GameObjectCommand(ITerminal terminal)
            : base(terminal, new string[] { "obj" })
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
        public void Create(string path, string name)
        {
            var parentObject = GameObjectUtility.Get(path);
            var gameObject = new GameObject(name);
            GameObjectUtility.SetParent(gameObject, parentObject);
        }

        [CommandMethod(Aliases = new string[] { "ren" })]
        public void Rename(string path, string newName)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            gameObject.name = newName;
        }

        [CommandMethod(Aliases = new string[] { "mv" })]
        public void Move(string path, string parentPath)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            var parentObject = GameObjectUtility.Get(parentPath);
            GameObjectUtility.SetParent(gameObject, parentObject);
        }

        [CommandMethod(Aliases = new string[] { "rm" })]
        public void Delete(string path)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            GameObject.DestroyImmediate(gameObject);
        }

        [CommandMethod(Aliases = new string[] { "on" })]
        public void Activate(string path)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            gameObject.SetActive(true);
        }

        [CommandMethod(Aliases = new string[] { "off" })]
        public void Deactivate(string path)
        {
            var gameObject = GameObjectUtility.Get(path, typeof(GameObject)) as GameObject;
            gameObject.SetActive(false);
        }

        [CommandMethod("list", Aliases = new string[] { "ls" })]
        [CommandMethodProperty(nameof(IsRecursive))]
        public void ShowList(string path = "")
        {
            var sb = new StringBuilder();
            var obj = GameObjectUtility.Find(path);
            var isRecursive = this.IsRecursive;
            ShowRecursive(sb, obj, path, isRecursive);
            this.Write(sb.ToString());
        }

        [CommandPropertySwitch("recursive", 'r')]
        public bool IsRecursive
        {
            get; set;
        }

        private static void ShowRecursive(StringBuilder sb, object obj, string path, bool isRecursive)
        {
            if (isRecursive == true)
            {
                sb.AppendLine($"path: {path}:");
            }
            foreach (var item in GameObjectUtility.GetChilds(obj))
            {
                sb.AppendLine(item.name);
            }
            sb.AppendLine();

            if (isRecursive)
            {
                var parentPath = path == "/" ? string.Empty : path;
                foreach (var item in GameObjectUtility.GetChilds(obj))
                {
                    ShowRecursive(sb, item, $"{parentPath}/{item.name}", isRecursive);
                }
            }
        }
    }
}
