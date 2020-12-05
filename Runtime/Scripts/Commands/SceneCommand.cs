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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class SceneCommand : TerminalCommandBase
    {
        public SceneCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor.DescriptorName == nameof(SceneName))
            {
                var query = from item in GetScenes()
                            where item.StartsWith(completionContext.Find)
                            select item;
                return query.ToArray();
            }
            return null;
        }

        [CommandPropertySwitch("list")]
        [CommandPropertyTrigger(nameof(SceneName), "")]
        public bool IsList { get; set; }

        [CommandPropertyRequired(DefaultValue = "")]
        public string SceneName { get; set; }

        protected override void OnExecute()
        {
            if (this.IsList == true)
            {
                this.ShowSceneList();
            }
            else if (this.SceneName == string.Empty)
            {
                this.ShowActiveScene();
            }
            else
            {
                this.LoadScene(this.SceneName);
            }
        }

        private void ShowSceneList()
        {
            var sb = new StringBuilder();
            var activeScene = SceneManager.GetActiveScene();
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            for (var i = 0; i < sceneCount; i++)
            {
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                var isCurrent = activeScene.buildIndex == i ? "*" : " ";
                sb.AppendLine($"{isCurrent}{i,2}: {sceneName}");
            }
            this.WriteLine(sb.ToString());
        }

        private void RemoveStyle(ITerminalGrid grid)
        {
            if (grid.Style == null)
                throw new InvalidOperationException("style is not applied.");
            grid.Style = null;
        }

        private void ShowActiveScene()
        {
            this.WriteLine(SceneManager.GetActiveScene().name);
        }

        private void LoadScene(string sceneName)
        {
            if (int.TryParse(sceneName, out var index) == true)
            {
                var scene = SceneManager.GetSceneByBuildIndex(index);
                if (scene == null)
                    throw new ArgumentException($"invalid scene index: '{index}'");
                SceneManager.LoadScene(index);
            }
            else
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                if (scene == null)
                    throw new ArgumentException($"invalid scene name: '{sceneName}'");
                SceneManager.LoadScene(sceneName);
            }
        }

        private static string[] GetScenes()
        {
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            var sceneList = new List<string>(sceneCount);
            for (var i = 0; i < sceneCount; i++)
            {
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                sceneList.Add(sceneName);
            }
            return sceneList.ToArray();
        }
    }
}