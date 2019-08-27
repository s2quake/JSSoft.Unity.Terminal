using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using JSSoft.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

namespace JSSoft.Communication.Shells
{
    public class ClientBehaviour : MonoBehaviour
    {
        private IShell shell;
        private CommandContext commandContext;
        private CommandWriter writer;
        public Terminal terminal;

        public void Awake()
        {
            this.shell = Container.GetService<IShell>();
            this.commandContext = Container.GetService<CommandContext>();
        }

        public async Task Start()
        {
            if (this.terminal != null)
            {
                this.terminal.onExecuted.AddListener(this.terminal_onExecuted);
                this.writer = new CommandWriter(this.terminal);
                this.terminal.onCompletion = this.commandContext.GetCompletion;
                this.commandContext.Out = this.writer;
                this.terminal.ActivateInputField();
                // Debug.Log("Start 1");
                await this.shell.StartAsync();
                // Debug.Log("Start 2");
                //await TestAsync();
            }
        }

        private async Task TestAsync()
        {
            Debug.Log("TestAsync 1");
            await Task.Delay(1000);
            Debug.Log("TestAsync 2");
            await Task.Delay(1000);
            Debug.Log("TestAsync 3");
        }

        public void Update()
        {
            if (this.terminal != null)
            {
                if (this.shell.Prompt != this.terminal.Prompt)
                {
                    this.terminal.Prompt = this.shell.Prompt;
                }
                var text = this.writer.GetString();
                if (text != null)
                {
                    this.terminal.Append(text);
                }
            }
        }

        public async void OnDestroy()
        {
            Debug.Log("OnDestroy 1");
            if (this.terminal != null)
            {
                // Debug.Log("OnDestroy 1");
                await this.shell.StopAsync();
                this.shell.Dispose();
                // Debug.Log("OnDestroy 2");
                await TestAsync();
            }
        }

        private void terminal_onExecuted(string command)
        {
            this.Run(command);
        }

        public async void Run(string commandLine)
        {
            try
            {
                await Task.Run(() => this.commandContext.Execute(this.commandContext.Name + " " + commandLine));
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                if (e.InnerException != null)
                    this.terminal.AppendLine(e.InnerException.Message);
                else
                    this.terminal.AppendLine(e.Message);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    this.terminal.AppendLine(e.InnerException.Message);
                else
                    this.terminal.AppendLine(e.Message);
            }
            finally
            {
                this.terminal.InsertPrompt();
            }
        }
    }
}