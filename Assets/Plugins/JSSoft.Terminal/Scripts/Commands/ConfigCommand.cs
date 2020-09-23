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

using System;
using System.Text;
using System.Threading.Tasks;
using JSSoft.Terminal;
using JSSoft.Terminal.Tasks;
using JSSoft.Library.Commands;
using JSSoft.Library.Threading;
using UnityEngine;
using System.Collections.Generic;
using JSSoft.Library;
using System.Linq;
using System.ComponentModel;

namespace JSSoft.Terminal.Commands
{
    [CommandSummary(CommandStrings.ConfigCommand.Summary)]
    [CommandSummary(CommandStrings.ConfigCommand.Summary_ko_KR, Locale = "ko-KR")]
    public class ConfigCommand : TerminalCommandBase
    {
        private readonly IConfigurationProvider provider;

        public ConfigCommand(ITerminal terminal, IConfigurationProvider provider)
            : base(terminal)
        {
            this.provider = provider;
        }

        public ConfigCommand(string name, ITerminal terminal, IConfigurationProvider provider)
            : base(terminal, name)
        {
            this.provider = provider;
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            if (completionContext.MemberDescriptor != null)
            {
                if (completionContext.MemberDescriptor.DescriptorName == nameof(ConfigName))
                {
                    var query = from item in this.Configs
                                let configName = item.PropertyName
                                where configName.StartsWith(completionContext.Find)
                                select configName;
                    return query.ToArray();
                }
            }
            return null;
        }

        [CommandPropertyRequired(DefaultValue = "")]
        [CommandSummary(CommandStrings.ConfigCommand.ConfigName.Summary)]
        [CommandSummary(CommandStrings.ConfigCommand.ConfigName.Summary_ko_KR, Locale = "ko-KR")]
        public string ConfigName { get; set; }

        [CommandPropertyRequired(DefaultValue = "")]
        [CommandSummary(CommandStrings.ConfigCommand.Value.Summary)]
        [CommandSummary(CommandStrings.ConfigCommand.Value.Summary_ko_KR, Locale = "ko-KR")]
        public string Value { get; set; }

        [CommandProperty("list")]
        [CommandPropertyTrigger(nameof(ConfigName), "")]
        [CommandSummary(CommandStrings.ConfigCommand.ListSwitch.Summary)]
        [CommandSummary(CommandStrings.ConfigCommand.ListSwitch.Summary_ko_KR, Locale = "ko-KR")]
        public bool ListSwitch { get; set; }

        [CommandProperty("reset")]
        [CommandPropertyTrigger(nameof(ListSwitch), false)]
        [CommandSummary(CommandStrings.ConfigCommand.ResetSwitch.Summary)]
        [CommandSummary(CommandStrings.ConfigCommand.ResetSwitch.Summary_ko_KR, Locale = "ko-KR")]
        public bool ResetSwitch { get; set; }

        protected override void OnExecute()
        {
            if (this.ListSwitch == true)
            {
                this.ShowList();
            }
            else if (this.ResetSwitch == true)
            {
                this.Reset(this.ConfigName);
            }
            else if (this.ConfigName == string.Empty)
            {
                this.ShowUsage();
            }
            else if (this.Value == string.Empty)
            {
                this.Show(this.ConfigName);
            }
            else
            {
                this.Set(this.ConfigName, this.Value);
            }
        }

        protected IEnumerable<ConfigurationPropertyDescriptor> Configs => this.provider.Configs;

        private void ShowList()
        {
            var sb = new StringBuilder();
            foreach (var item in this.Configs)
            {
                sb.AppendLine($"{item.PropertyName}={item.Value}");
            }
            this.Terminal.Append(sb.ToString());
        }

        private void Show(string configName)
        {
            var config = this.GetProperty(configName);
            this.Terminal.AppendLine($"{config.Value}");
        }

        private void Set(string configName, string value)
        {
            var property = this.GetProperty(configName);
            if (property.PropertyType != typeof(string))
            {
                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                property.Value = converter.ConvertFromString(value);
            }
            else
            {
                property.Value = value;
            }
        }

        private void Reset(string configName)
        {
            var property = this.GetProperty(configName);
            property.Reset();
        }

        private void ShowUsage()
        {
            this.Terminal.AppendLine("type 'help config'");
        }

        private ConfigurationPropertyDescriptor GetProperty(string configName)
        {
            var config = this.Configs.FirstOrDefault(item => item.PropertyName == configName);
            if (config == null)
                throw new ArgumentException($"{configName} does not existed property.");
            return config;
        }
    }
}