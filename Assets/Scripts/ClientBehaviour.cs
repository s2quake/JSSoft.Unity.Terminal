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

namespace JSSoft.Communication.Shells
{
    public class ClientBehaviour : MonoBehaviour
    {
        private IShell shell;
        private CommandContext commandContext;
        private TerminalPro terminal;
        private CancellationTokenSource cancellation;
        private CommandWriter writer;

        public void Awake()
        {
            this.shell = Container.GetService<IShell>();
            this.commandContext = Container.GetService<CommandContext>();
        }

        public void Start()
        {
            this.terminal = GameObject.FindObjectOfType<TerminalPro>();
            this.terminal.onExecuted.AddListener(this.terminal_onExecuted);
            this.cancellation = new CancellationTokenSource();
            this.writer = new CommandWriter(this.terminal);
            this.commandContext.Out = this.writer;
        }

        public void Update()
        {
            var text = this.writer.GetString();
            if (text != null)
            {
                this.terminal.Append(text);
            }
        }

        public void OnDestroy()
        {
            this.cancellation.Cancel();
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
                //this.SetPrompt();
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