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
    public class JavascriptContextHost : TerminalHostBase
    {
        private Engine engine;
        private TextWriter consoleWriter;

        protected override void Awake()
        {
            base.Awake();
            this.engine = new Engine(cfg => cfg.AllowClr());
            this.engine.SetValue("print", new Action<object>((obj) => this.Terminal.AppendLine($"{obj}")));
            this.engine.SetValue("load", new Func<string, object>(path => engine.Execute(File.ReadAllText(path)).GetCompletionValue()));
            this.IsAsync = false;
            this.consoleWriter = Console.Out;
        }

        protected override void Start()
        {
            base.Start();
            this.IsAsync = false;
            this.Terminal.AppendLine($"Welcome to Jint ({this.Version})");
            this.Terminal.AppendLine("Type 'exit' to leave, 'print()' to write on the console, 'load()' to load scripts.");
            this.Terminal.AppendLine();
            this.Terminal.Prompt = "jint> ";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Console.SetOut(new TerminalTextWriter(this.Terminal));
        }

        protected override void OnDisable()
        {
            Console.SetOut(this.consoleWriter);
            base.OnDisable();
        }

        protected override void OnException(Exception e, string message)
        {
            this.Terminal.ForegroundColor = TerminalColor.Red;
            base.OnException(e, message);
            this.Terminal.ResetColor();
            UnityEngine.Debug.LogError(message);
        }

        protected override void OnRun(string command)
        {
            if (command == "exit")
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                UnityEngine.Application.Quit();
#endif
            }
            else
            {
                var result = engine.GetValue(engine.Execute(command).GetCompletionValue());
                if (result.Type != Types.None && result.Type != Types.Null && result.Type != Types.Undefined)
                {
                    var str = TypeConverter.ToString(engine.Json.Stringify(engine.Json, Arguments.From(result, Undefined.Instance, "  ")));
                    this.Terminal.AppendLine($"=> {str}");
                }
            }
        }

        private string Version
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }
    }
}
