﻿// MIT License
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
using System.Linq;
using System.Text;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class InfoCommand : TerminalCommandBase
    {
        private static readonly Dictionary<string, Func<string, string>> actions;

        static InfoCommand()
        {
            actions = new Dictionary<string, Func<string, string>>()
            {
                { nameof(Application.absoluteURL), (item) => $"{Application.absoluteURL}" },
                { nameof(Application.backgroundLoadingPriority), (item) => $"{Application.backgroundLoadingPriority}" },
                { nameof(Application.buildGUID), (item) => $"{Application.buildGUID}" },
                { nameof(Application.cloudProjectId), (item) => $"{Application.cloudProjectId}" },
                { nameof(Application.companyName), (item) => $"{Application.companyName}" },
                { nameof(Application.consoleLogPath), (item) => $"{Application.consoleLogPath}" },
                { nameof(Application.dataPath), (item) => $"{Application.dataPath}" },
                { nameof(Application.genuine), (item) => $"{Application.genuine}" },
                { nameof(Application.genuineCheckAvailable), (item) => $"{Application.genuineCheckAvailable}" },
                { nameof(Application.identifier), (item) => $"{Application.identifier}" },
                { nameof(Application.installerName), (item) => $"{Application.installerName}" },
                { nameof(Application.installMode), (item) => $"{Application.installMode}" },
                { nameof(Application.internetReachability), (item) => $"{Application.internetReachability}" },
                { nameof(Application.isBatchMode), (item) => $"{Application.isBatchMode}" },
                { nameof(Application.isConsolePlatform), (item) => $"{Application.isConsolePlatform}" },
                { nameof(Application.isEditor), (item) => $"{Application.isEditor}" },
                { nameof(Application.isFocused), (item) => $"{Application.isFocused}" },
                { nameof(Application.isMobilePlatform), (item) => $"{Application.isMobilePlatform}" },
                { nameof(Application.isPlaying), (item) => $"{Application.isPlaying}" },
                { nameof(Application.persistentDataPath), (item) => $"{Application.persistentDataPath}" },
                { nameof(Application.platform), (item) => $"{Application.platform}" },
                { nameof(Application.productName), (item) => $"{Application.productName}" },
                { nameof(Application.runInBackground), (item) => $"{Application.runInBackground}" },
                { nameof(Application.sandboxType), (item) => $"{Application.sandboxType}" },
                { nameof(Application.streamingAssetsPath), (item) => $"{Application.streamingAssetsPath}" },
                { nameof(Application.systemLanguage), (item) => $"{Application.systemLanguage}" },
                { nameof(Application.targetFrameRate), (item) => $"{Application.targetFrameRate}" },
                { nameof(Application.temporaryCachePath), (item) => $"{Application.temporaryCachePath}" },
                { nameof(Application.unityVersion), (item) => $"{Application.unityVersion}" },
                { nameof(Application.version), (item) => $"{Application.version}" },
            };
        }

        public InfoCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyArray]
        public string[] PropertyNames
        {
            get; set;
        }

        [CommandPropertySwitch("quiet")]
        public bool IsQuiet
        {
            get; set;
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor.DescriptorName == nameof(PropertyNames))
            {
                var query = from item in actions
                            where item.Key.StartsWith(completionContext.Find)
                            select item.Key;
                return query.Except(completionContext.Arguments).ToArray();
            }
            return actions.Keys.ToArray();
        }

        protected override void OnExecute()
        {
            var sb = new StringBuilder();
            var propertyNames = this.PropertyNames;
            var isQuiet = this.IsQuiet;
            if (propertyNames.Any() == true)
            {
                foreach (var item in propertyNames)
                {
                    if (actions.ContainsKey(item) == false)
                        throw new InvalidOperationException($"property '{item}' does not exists.");
                    var action = actions[item];
                    WriteInfo(sb, item, action(item), isQuiet);
                }
            }
            else
            {
                foreach (var item in actions)
                {
                    WriteInfo(sb, item.Key, item.Value(item.Key), isQuiet);
                }
            }
            this.WriteLine(sb.ToString());
        }

        private static void WriteInfo(StringBuilder sb, string key, string value, bool isQuiet)
        {
            if (isQuiet == true)
                sb.AppendLine(value);
            else
                sb.AppendLine($"{key}: {value}");
        }
    }
}
