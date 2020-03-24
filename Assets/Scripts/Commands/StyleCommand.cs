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
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using JSSoft.Communication.Shells;
using JSSoft.UI;
using Ntreev.Library.Commands;
using Ntreev.Library.Threading;
using UnityEngine;
#if MEF
using System.ComponentModel.Composition;
#endif

namespace JSSoft.Communication.Commands
{
#if MEF
    [Export(typeof(ICommand))]
#endif
    class StyleCommand : CommandAsyncBase
    {
        private readonly Lazy<ITerminalGrid> grid;
        private Dispatcher dispatcher;

#if MEF
        [ImportingConstructor]
#endif
        public StyleCommand(Lazy<ITerminalGrid> grid)
        {
            this.grid = grid;
            this.dispatcher = Dispatcher.Current;
        }

        public override string[] GetCompletions(CommandCompletionContext completionContext)
        {
            var resources = UnityEngine.GameObject.FindObjectOfType<StyleResources>();
            return resources.Styles.Select(item => item.name).ToArray();
        }

        [CommandProperty("list")]
        public bool IsList
        {
            get; set;
        }

        [CommandProperty(IsRequired = true)]
        [DefaultValue("")]
        public string StyleName
        {
            get; set;
        }

        protected override async Task OnExecuteAsync()
        {
            if (this.IsList == true)
            {
                await this.ShowStyleListAsync();
            }
            else if (this.StyleName == string.Empty)
            {
                await this.ShowCurrentStyleAsync();
            }
            else
            {
                await this.ChangeStyleAsync();
            }
        }

        private Task ShowStyleListAsync()
        {
            return this.dispatcher.InvokeAsync(() =>
            {
                var resources = UnityEngine.GameObject.FindObjectOfType<StyleResources>();
                var names = resources.Styles.Select(item => item.name).ToArray();
                foreach (var item in names)
                {
                    var isCurrent = this.Grid.Style.name == item ? "*" : " ";
                    this.Out.WriteLine($"{isCurrent} {item}");
                }
            });
        }

        private Task ShowCurrentStyleAsync()
        {
            return this.dispatcher.InvokeAsync(() =>
            {
                var style = this.Grid.Style;
                this.Out.WriteLine(style.name);
            });
        }

        private async Task ChangeStyleAsync()
        {
            var rectangle = await this.dispatcher.InvokeAsync(() =>
            {
                var resources = UnityEngine.GameObject.FindObjectOfType<StyleResources>();
                var style = resources.Styles.FirstOrDefault(item => string.Compare(item.name, this.StyleName, true) == 0);
                if (style != null)
                {
                    this.Grid.Style = style;
                    this.Out.WriteLine($"{this.StyleName} applied.");
                    return this.Grid.Rectangle;
                }
                else
                {
                    this.Out.WriteLine($"{this.StyleName} style does not exits.");
                    return Rect.zero;
                }
            });
            await this.dispatcher.InvokeAsync(() =>
            {
                if (Application.isEditor == false && rectangle != Rect.zero)
                {
                    Screen.SetResolution((int)rectangle.width, (int)rectangle.height, false, 0);
                }
            });
        }

        private ITerminalGrid Grid => this.grid.Value;
    }
}