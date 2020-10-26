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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JSSoft.Library;
using JSSoft.Library.Commands;
using JSSoft.Library.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JSSoft.Unity.Terminal.Commands
{
    // [CommandSummary(CommandStrings.DateCommand.Summary)]
    // [CommandSummary(CommandStrings.DateCommand.Summary_ko_KR, Locale = "ko-KR")]
    public class ComponentCommand : TerminalCommandMethodBase
    {
        public ComponentCommand(ITerminal terminal)
            : base(terminal, new string[] { "comp" })
        {
        }

        [CommandMethod(Aliases = new string[] { "new" })]
        public void Create()
        {

        }

        [CommandMethod(Aliases = new string[] { "ren" })]
        public void Rename()
        {

        }

        [CommandMethod(Aliases = new string[] { "mv" })]
        public void Move()
        {

        }

        [CommandMethod(Aliases = new string[] { "rm" })]
        public void Delete()
        {

        }

        [CommandMethod(Aliases = new string[] { "on" })]
        public void Activate()
        {

        }

        [CommandMethod(Aliases = new string[] { "off" })]
        public void Deactivate(string path = "/")
        {
           
        }

        [CommandMethod("list", Aliases = new string[] { "ls" })]
        public void ShowList(string path = "/")
        {
            
        }
    }
}
