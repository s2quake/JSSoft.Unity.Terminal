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

using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal.Commands
{
    [CommandSummary(CommandStrings.SceneCommand.Summary)]
    [CommandSummary(CommandStrings.SceneCommand.Summary_ko_KR, Locale = "ko-KR")]
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

        [CommandSummary(CommandStrings.SceneCommand.IsList.Summary)]
        [CommandSummary(CommandStrings.SceneCommand.IsList.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertySwitch("list")]
        [CommandPropertyTrigger(nameof(SceneName), "")]
        public bool ListSwitch { get; set; }

        [CommandSummary(CommandStrings.SceneCommand.SceneName.Summary)]
        [CommandSummary(CommandStrings.SceneCommand.SceneName.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertyRequired(DefaultValue = "")]
        public string SceneName { get; set; }

        protected override void OnExecute()
        {
            if (this.ListSwitch == true)
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