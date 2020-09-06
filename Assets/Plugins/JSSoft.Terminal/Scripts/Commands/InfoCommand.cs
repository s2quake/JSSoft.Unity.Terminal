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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSSoft.Terminal;
using JSSoft.Terminal.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Library.Threading;
using UnityEngine;

namespace JSSoft.Terminal.Commands
{
    [CommandSummary("Displays information.")]
    [CommandSummary("정보를 표시합니다.", Locale = "ko-KR")]
    public class InfoCommand : TerminalCommandBase
    {
        private static readonly Dictionary<string, Func<string, string>> actions;

        static InfoCommand()
        {
            actions = new Dictionary<string, Func<string, string>>()
            {
                { nameof(Application.absoluteURL), (item) => $"{item}: {Application.absoluteURL}" },
                { nameof(Application.backgroundLoadingPriority), (item) => $"{item}: {Application.backgroundLoadingPriority}" },
                { nameof(Application.buildGUID), (item) => $"{item}: {Application.buildGUID}" },
                { nameof(Application.cloudProjectId), (item) => $"{item}: {Application.cloudProjectId}" },
                { nameof(Application.companyName), (item) => $"{item}: {Application.companyName}" },
                { nameof(Application.consoleLogPath), (item) => $"{item}: {Application.consoleLogPath}" },
                { nameof(Application.dataPath), (item) => $"{item}: {Application.dataPath}" },
                { nameof(Application.genuine), (item) => $"{item}: {Application.genuine}" },
                { nameof(Application.genuineCheckAvailable), (item) => $"{item}: {Application.genuineCheckAvailable}" },
                { nameof(Application.identifier), (item) => $"{item}: {Application.identifier}" },
                { nameof(Application.installerName), (item) => $"{item}: {Application.installerName}" },
                { nameof(Application.installMode), (item) => $"{item}: {Application.installMode}" },
                { nameof(Application.internetReachability), (item) => $"{item}: {Application.internetReachability}" },
                { nameof(Application.isBatchMode), (item) => $"{item}: {Application.isBatchMode}" },
                { nameof(Application.isConsolePlatform), (item) => $"{item}: {Application.isConsolePlatform}" },
                { nameof(Application.isEditor), (item) => $"{item}: {Application.isEditor}" },
                { nameof(Application.isFocused), (item) => $"{item}: {Application.isFocused}" },
                { nameof(Application.isMobilePlatform), (item) => $"{item}: {Application.isMobilePlatform}" },
                { nameof(Application.isPlaying), (item) => $"{item}: {Application.isPlaying}" },
                { nameof(Application.persistentDataPath), (item) => $"{item}: {Application.persistentDataPath}" },
                { nameof(Application.platform), (item) => $"{item}: {Application.platform}" },
                { nameof(Application.productName), (item) => $"{item}: {Application.productName}" },
                { nameof(Application.runInBackground), (item) => $"{item}: {Application.runInBackground}" },
                { nameof(Application.sandboxType), (item) => $"{item}: {Application.sandboxType}" },
                { nameof(Application.streamingAssetsPath), (item) => $"{item}: {Application.streamingAssetsPath}" },
                { nameof(Application.systemLanguage), (item) => $"{item}: {Application.systemLanguage}" },
                { nameof(Application.targetFrameRate), (item) => $"{item}: {Application.targetFrameRate}" },
                { nameof(Application.temporaryCachePath), (item) => $"{item}: {Application.temporaryCachePath}" },
                { nameof(Application.unityVersion), (item) => $"{item}: {Application.unityVersion}" },
                { nameof(Application.version), (item) => $"{item}: {Application.version}" },
            };
        }

        public InfoCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyRequired(DefaultValue = "")]
        public string PropertyName
        {
            get; set;
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            return actions.Keys.ToArray();
        }

        protected override void OnExecute()
        {
            var sb = new StringBuilder();
            var propertyName = this.PropertyName;
            if (propertyName == string.Empty)
            {
                foreach (var item in actions)
                {
                    sb.AppendLine(item.Value(item.Key));
                }
            }
            else if (actions.ContainsKey(propertyName) == true)
            {
                sb.AppendLine(actions[propertyName](propertyName));
            }
            else
            {
                sb.AppendLine($"property '{propertyName}' does not exists.");
            }
            this.Terminal.AppendLine(sb.ToString());
        }
    }
}
