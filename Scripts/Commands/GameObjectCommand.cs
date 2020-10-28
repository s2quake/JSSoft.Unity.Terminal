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
    [CommandSummary(CommandStrings.GameObjectCommand.Summary)]
    [CommandSummary(CommandStrings.GameObjectCommand.Summary_ko_KR, Locale = "ko-KR")]
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

        [CommandMethod(Aliases = new string[] { "new" })]
        [CommandSummary(CommandStrings.GameObjectCommand.Create.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.Create.Summary_ko_KR, Locale = "ko-KR")]
        public void Create(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path,
            [CommandSummary(CommandStrings.GameObjectCommand.Name.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Name.Summary_ko_KR, Locale = "ko-KR")]
            string name)
        {
            var parentObject = GameObjectUtility.Get(path);
            var gameObject = new GameObject(name);
            GameObjectUtility.SetParent(gameObject, parentObject);
        }

        [CommandMethod(Aliases = new string[] { "ren" })]
        [CommandSummary(CommandStrings.GameObjectCommand.Rename.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.Rename.Summary_ko_KR, Locale = "ko-KR")]
        public void Rename(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path,
            [CommandSummary(CommandStrings.GameObjectCommand.NewName.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.NewName.Summary_ko_KR, Locale = "ko-KR")]
            string newName)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            gameObject.name = newName;
        }

        [CommandMethod(Aliases = new string[] { "mv" })]
        [CommandSummary(CommandStrings.GameObjectCommand.Move.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.Move.Summary_ko_KR, Locale = "ko-KR")]
        public void Move(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path,
            [CommandSummary(CommandStrings.GameObjectCommand.ParentPath.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.ParentPath.Summary_ko_KR, Locale = "ko-KR")]
            string parentPath)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            var parentObject = GameObjectUtility.Get(parentPath);
            GameObjectUtility.SetParent(gameObject, parentObject);
        }

        [CommandMethod(Aliases = new string[] { "rm" })]
        [CommandSummary(CommandStrings.GameObjectCommand.Delete.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.Delete.Summary_ko_KR, Locale = "ko-KR")]
        public void Delete(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            GameObject.DestroyImmediate(gameObject);
        }

        [CommandMethod(Aliases = new string[] { "on" })]
        [CommandSummary(CommandStrings.GameObjectCommand.Activate.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.Activate.Summary_ko_KR, Locale = "ko-KR")]
        public void Activate(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            gameObject.SetActive(true);
        }

        [CommandMethod(Aliases = new string[] { "off" })]
        [CommandSummary(CommandStrings.GameObjectCommand.Deactivate.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.Deactivate.Summary_ko_KR, Locale = "ko-KR")]
        public void Deactivate(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            gameObject.SetActive(false);
        }

        [CommandMethod("list", Aliases = new string[] { "ls" })]
        [CommandMethodProperty(nameof(IsRecursive))]
        [CommandSummary(CommandStrings.GameObjectCommand.ShowList.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.ShowList.Summary_ko_KR, Locale = "ko-KR")]
        public void ShowList(
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary)]
            [CommandSummary(CommandStrings.GameObjectCommand.Path.Summary_ko_KR, Locale = "ko-KR")]
            string path = "/")
        {
            var sb = new StringBuilder();
            var obj = GameObjectUtility.Find(path);
            var isRecursive = this.IsRecursive;
            ShowRecursive(sb, obj, path, isRecursive);
            this.Write(sb.ToString());
        }

        [CommandPropertySwitch("recursive", 'r')]
        [CommandSummary(CommandStrings.GameObjectCommand.IsRecursive.Summary)]
        [CommandSummary(CommandStrings.GameObjectCommand.IsRecursive.Summary_ko_KR, Locale = "ko-KR")]
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
