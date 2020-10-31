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
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal.Commands
{
    [RequireComponent(typeof(CommandContextHost))]
    [RequireComponent(typeof(TerminalGridState))]
    [DefaultExecutionOrder(int.MaxValue)]
    public abstract class CommandSystemBase : MonoBehaviour, ICommandProvider, ICommandConfigurationProvider
    {
        private readonly CommandConfigurationProvider configs = new CommandConfigurationProvider();
        private readonly CommandProvider commands = new CommandProvider();

        public void AddConfig(ICommandConfiguration provider)
        {
            this.configs.Add(provider);
        }

        public void RemoveConfig(ICommandConfiguration provider)
        {
            this.configs.Remove(provider);
        }

        public void RemoveConfig(string configName)
        {
            this.configs.Remove(configName);
        }

        public void AddConfigs(object instance)
        {
            this.configs.AddInstance(instance);
        }

        public void AddConfigs(object instance, string instanceName)
        {
            this.configs.AddInstance(instance, instanceName);
        }

        public void RemoveConfigs(object instance)
        {
            if (instance != null)
                this.configs.RemoveInstance(instance);
        }

        public void RemoveConfigs(object instance, string instanceName)
        {
            if (instance != null)
                this.configs.RemoveInstance(instance, instanceName);
        }

        protected virtual void Awake()
        {
            var commandContext = GetComponent<CommandContextHost>();
            var terminalGrid = commandContext.Grid;
            commandContext.CommandProvider = this;
            commandContext.ConfigurationProvider = this;
        }

        protected virtual void OnEnable()
        {
            var commandContext = GetComponent<CommandContextHost>();
            var terminal = commandContext.Terminal;
            terminal.Prompt = $"{SceneManager.GetActiveScene().name}:$ ";
        }

        protected virtual void OnDestroy()
        {
            var commandContext = GetComponent<CommandContextHost>();
            var terminalGrid = commandContext.Grid;
        }

        protected abstract IEnumerable<ICommand> OnCommandProvide(ITerminal terminal);

        #region ICommandProvider

        IEnumerable<ICommand> ICommandProvider.Provide(ITerminal terminal, ICommandConfigurationProvider configurationProvider)
        {
            foreach (var item in commands.Provide(terminal, configurationProvider))
            {
                yield return item;
            }
            foreach (var item in this.OnCommandProvide(terminal))
            {
                yield return item;
            }
        }

        #endregion

        #region ICommandConfigurationProvider

        IEnumerable<ICommandConfiguration> ICommandConfigurationProvider.Configs => configs.Configs;

        #endregion
    }
}