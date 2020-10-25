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

using JSSoft.Library;
using JSSoft.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace JSSoft.Unity.Terminal.Commands
{
    [CommandSummary(CommandStrings.ConfigCommand.Summary)]
    [CommandSummary(CommandStrings.ConfigCommand.Summary_ko_KR, Locale = "ko-KR")]
    public class ConfigCommand : TerminalCommandBase
    {
        private readonly ICommandConfigurationProvider provider;

        public ConfigCommand(ITerminal terminal, ICommandConfigurationProvider provider)
            : base(terminal)
        {
            this.provider = provider;
        }

        public ConfigCommand(string name, ITerminal terminal, ICommandConfigurationProvider provider)
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
                                let configName = item.Name
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

        [CommandProperty("list", DefaultValue = "")]
        [CommandPropertyTrigger(nameof(ConfigName), "")]
        [CommandSummary(CommandStrings.ConfigCommand.ListSwitch.Summary)]
        [CommandSummary(CommandStrings.ConfigCommand.ListSwitch.Summary_ko_KR, Locale = "ko-KR")]
        public string ListPattern { get; set; }

        [CommandPropertySwitch("reset")]
        [CommandPropertyTrigger(nameof(ListPattern), false)]
        [CommandSummary(CommandStrings.ConfigCommand.ResetSwitch.Summary)]
        [CommandSummary(CommandStrings.ConfigCommand.ResetSwitch.Summary_ko_KR, Locale = "ko-KR")]
        public bool ResetSwitch { get; set; }

        protected override void OnExecute()
        {
            if (this.ListPattern != null)
            {
                this.ShowList(this.ListPattern);
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

        protected IEnumerable<ICommandConfiguration> Configs => this.provider.Configs;

        private void ShowList(string filter)
        {
            var sb = new StringBuilder();
            var query = from item in this.Configs
                        where item.IsEnabled == true
                        where filter == string.Empty || StringUtility.Glob(item.Name, filter, false)
                        select item;
            foreach (var item in query)
            {
                sb.AppendLine($"{item.Name}={ConfigValueToString(item)}");
            }
            this.Write(sb.ToString());
        }

        private void Show(string configName)
        {
            var config = this.GetConfiguration(configName);
            this.WriteLine(ConfigValueToString(config));
        }

        private void Set(string configName, string value)
        {
            var config = this.GetConfiguration(configName);
            if (config.Type != typeof(string))
            {
                var converter = TypeDescriptor.GetConverter(config.Type);
                config.Value = converter.ConvertFromString(value);
            }
            else
            {
                config.Value = value;
            }
        }

        private void Reset(string configName)
        {
            var config = this.GetConfiguration(configName);
            if (config.DefaultValue != DBNull.Value)
            {
                config.Value = config.DefaultValue;
            }
        }

        private void ShowUsage()
        {
            this.WriteLine("type 'help config'");
        }

        private ICommandConfiguration GetConfiguration(string configName)
        {
            var config = this.Configs.FirstOrDefault(item => item.IsEnabled == true && item.Name == configName);
            if (config == null)
                throw new ArgumentException($"{configName} does not existed property.");
            return config;
        }

        private static string ConfigValueToString(ICommandConfiguration config)
        {
            if (config.Type == typeof(bool))
                return $"{config.Value}".ToLower();
            if (config.Type == typeof(string) && config.Value != null)
                return $"\"{config.Value}\"";
            return $"{config.Value}";
        }
    }
}
