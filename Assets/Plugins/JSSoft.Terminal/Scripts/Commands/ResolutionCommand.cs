// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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
using System.Text;

namespace JSSoft.Terminal.Commands
{
    public class ResolutionCommand : TerminalCommandBase
    {
        public ResolutionCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandProperty]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public int Width { get; set; }

        [CommandProperty]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public int Height { get; set; }

        [CommandProperty("window")]
        [CommandPropertyTrigger(nameof(IsFullScreen), false)]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public bool IsWindowMode { get; set; }

        [CommandProperty("full")]
        [CommandPropertyTrigger(nameof(IsWindowMode), false)]
        [CommandPropertyTrigger(nameof(IsList), false)]
        public bool IsFullScreen { get; set; }

        [CommandProperty("list")]
        public bool IsList { get; set; }

        protected override void OnExecute()
        {
            this.Execute();
        }

        private void Execute()
        {
            if (this.IsList == true)
            {
                this.ShowList();
            }
            else
            {
                this.SetResolution();
            }
        }

        private void ShowList()
        {
            var sb = new StringBuilder();
            sb.AppendLine("available resolutions: ");
            foreach (var item in Screen.resolutions)
            {
                sb.AppendLine($"   {item}");
            }
            sb.AppendLine("current resolution: ");
            sb.AppendLine($"   {Screen.currentResolution}");
            sb.AppendLine();
            this.Terminal.Append(sb.ToString());
        }

        private void SetResolution()
        {
            var width = this.PreferredWidth;
            var height = this.PreferredHeight;
            var fullScreen = this.PreferredFullScreen;
            if (width != Screen.width || height != Screen.height || fullScreen != Screen.fullScreen)
            {
                Screen.SetResolution(width, height, fullScreen);
            }
            this.Terminal.AppendLine($"{Screen.width} x {Screen.height}");
        }

        private int PreferredWidth => Width != 0 ? Width : Screen.width;

        private int PreferredHeight => Height != 0 ? Height : Screen.height;

        private bool PreferredFullScreen
        {
            get
            {
                if (IsWindowMode == true)
                    return false;
                else if (IsFullScreen == true)
                    return true;
                return Screen.fullScreen;
            }
        }
    }
}
