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

using UnityEngine;
// using JSSoft.Library.Commands;
using JSSoft.Terminal.Commands;
using System.Linq;
using JSSoft.Terminal;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using JSSoft.Terminal.Tasks;
using Jint.Native;
using Jint.Runtime;
using Jint;
using Types = Jint.Runtime.Types;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace JSSoft.Javascript
{
    [RequireComponent(typeof(TerminalBase))]
    [DisallowMultipleComponent]
    public class JavascriptContextHost : MonoBehaviour, IServiceProvider
    {
        private Engine engine;
        private TerminalBase terminal;
        private TerminalGridBase grid;
        public TerminalBase Terminal => this.terminal;
        public TerminalGridBase Grid => this.grid;

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            this.engine = new Engine(cfg => cfg.AllowClr());
            this.engine.SetValue("print", new Action<object>((obj) => this.terminal.AppendLine($"{obj}")));
            this.engine.SetValue("load", new Func<string, object>(
                    path => engine.Execute(File.ReadAllText(path))
                                  .GetCompletionValue()));

            this.grid = this.GetComponent<TerminalGridBase>();
            this.terminal = this.GetComponent<TerminalBase>();
            this.terminal.ResetOutput();

            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;

            this.terminal.AppendLine($"Welcome to Jint ({version})");
            this.terminal.AppendLine("Type 'exit' to leave, " +
                              "'print()' to write on the console, " +
                              "'load()' to load scripts.");
            this.terminal.AppendLine();
            this.terminal.CursorPosition = 0;
            this.terminal.Prompt = "jint> ";
            this.terminal.Executing += Terminal_Executing;
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITerminal))
                return this.terminal;
            else if (serviceType == typeof(ITerminalGrid))
                return this.grid;
            else if (serviceType == typeof(TerminalDispatcher))
                return this.terminal.Dispatcher;
            return null;
        }

        private void Terminal_Executing(object sender, TerminalExecuteEventArgs e)
        {
            if (this.terminal.Dispatcher != null)
            {
                this.Run(e);
            }
            else
            {
                this.terminal.AppendLine("dispatcher is null.");
            }
        }

        private void Run(TerminalExecuteEventArgs e)
        {
            try
            {
                var input = e.Command;
                if (input == "exit")
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    UnityEngine.Application.Quit();
#endif
                }
                else
                {
                    var result = engine.GetValue(engine.Execute(input).GetCompletionValue());
                    if (result.Type != Types.None && result.Type != Types.Null && result.Type != Types.Undefined)
                    {
                        var str = TypeConverter.ToString(engine.Json.Stringify(engine.Json, Arguments.From(result, Undefined.Instance, "  ")));
                        this.terminal.AppendLine($"=> {str}");
                    }
                    e.Success();
                }
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                this.AppendException(ex);
                e.Fail(ex);
            }
            catch (JavaScriptException ex)
            {
                this.terminal.ForegroundColor = TerminalColor.Red;
                this.AppendException(ex);
                this.terminal.ResetColor();
                e.Fail(ex);
            }
            catch (Exception ex)
            {
                this.AppendException(ex);
                e.Fail(ex);
            }
        }

        private void AppendException(Exception e)
        {
            var message = GetMessage();
            this.terminal.AppendLine(message);
            UnityEngine.Debug.LogError(message);

            string GetMessage()
            {
                if (this.terminal.IsVerbose == true)
                {
                    return $"{e}";
                }
                else
                {
                    if (e.InnerException != null)
                        return e.InnerException.Message;
                    else
                        return e.Message;
                }
            }
        }

        #region IServiceProvider

        object IServiceProvider.GetService(Type serviceType)
        {
            return this.GetService(serviceType);
        }

        #endregion
    }
}
