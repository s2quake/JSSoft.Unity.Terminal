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
using System.Linq;
using JSSoft.Library.Commands;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

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
        [CommandProperty("list")]
        [CommandPropertyTrigger(nameof(SceneName), "")]
        public bool IsList { get; set; }

        [CommandSummary(CommandStrings.SceneCommand.SceneName.Summary)]
        [CommandSummary(CommandStrings.SceneCommand.SceneName.Summary_ko_KR, Locale = "ko-KR")]
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
            var activeScene = SceneManager.GetActiveScene();
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                var isCurrent = activeScene.buildIndex == i ? "*" : " ";
                this.WriteLine($"{isCurrent}{i,2}: {sceneName}");
            }
            this.WriteLine();
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
                SceneManager.LoadScene(index);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        private static string[] GetScenes()
        {
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            var sceneList = new List<string>(sceneCount);
            for (int i = 0; i < sceneCount; i++)
            {
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                sceneList.Add(sceneName);
            }
            return sceneList.ToArray();
        }
    }
}