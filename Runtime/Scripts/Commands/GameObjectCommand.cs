////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

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
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
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
        public void Create(string path, string name)
        {
            var parentObject = GameObjectUtility.Get(path);
            var gameObject = new GameObject(name);
            GameObjectUtility.SetParent(gameObject, parentObject);
        }

        [CommandMethod(Aliases = new string[] { "ren" })]
        public void Rename(string path, string newName)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            gameObject.name = newName;
        }

        [CommandMethod(Aliases = new string[] { "mv" })]
        public void Move(string path, string parentPath)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            var parentObject = GameObjectUtility.Get(parentPath);
            GameObjectUtility.SetParent(gameObject, parentObject);
        }

        [CommandMethod(Aliases = new string[] { "rm" })]
        public void Delete(string path)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            GameObject.DestroyImmediate(gameObject);
        }

        [CommandMethod(Aliases = new string[] { "on" })]
        public void Activate(string path)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            gameObject.SetActive(true);
        }

        [CommandMethod(Aliases = new string[] { "off" })]
        public void Deactivate(string path)
        {
            var gameObject = GameObjectUtility.GetGameObject(path);
            gameObject.SetActive(false);
        }

        [CommandMethod("list", Aliases = new string[] { "ls" })]
        [CommandMethodProperty(nameof(IsRecursive))]
        public void ShowList(string path = "/")
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
