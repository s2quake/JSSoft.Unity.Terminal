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
using System.ComponentModel;
using System.Threading.Tasks;
using JSSoft.Library.Threading;
using JSSoft.Library.Commands;
using UnityEngine;
using System.Threading;
using JSSoft.Terminal.Tasks;

namespace JSSoft.Terminal.Commands
{
    [CommandSummary(CommandStrings.ExitCommand.Summary)]
    [CommandSummary(CommandStrings.ExitCommand.Summary_ko_KR)]
    public class ExitCommand : TerminalCommandBase
    {
        public ExitCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandSummary(CommandStrings.ExitCommand.ExitCode.Summary)]
        [CommandSummary(CommandStrings.ExitCommand.ExitCode.Summary_ko_KR, Locale = "ko-KR")]
        [CommandPropertyRequired(DefaultValue = 0)]
        public int ExitCode { get; set; }

        protected override void OnExecute()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}