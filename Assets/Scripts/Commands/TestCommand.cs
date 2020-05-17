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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JSSoft.Communication.Commands
{
    class TestCommand : CommandAsyncBase
    {
        private readonly ITerminalGrid grid;
        private Dispatcher dispatcher;

        public TestCommand(ITerminalGrid grid)
        {
            this.grid = grid;
            this.dispatcher = Dispatcher.Current;
        }

        // [CommandProperty(IsRequired = true)]
        // [DefaultValue("")]
        // public int? Value
        // {
        //     get; set;
        // }

        protected override async Task OnExecuteAsync()
        {
            await this.dispatcher.InvokeAsync(() =>
            {
                var assemblyDirectory = System.IO.Path.GetDirectoryName(GetAssemblyPath());
                // var name = Type.GetType("UnityEngine.Application, UnityEngine").GetProperty("platform").GetValue(null).ToString();
                // this.Out.WriteLine(name);
                this.Out.WriteLine(assemblyDirectory);
                this.Out.WriteLine($"Linux: {RuntimeInformation.IsOSPlatform(OSPlatform.Linux)}");
                this.Out.WriteLine($"OSX: {RuntimeInformation.IsOSPlatform(OSPlatform.OSX)}");
                this.Out.WriteLine($"64bit: {IntPtr.Size == 8}");
            });
        }

        private static string GetAssemblyPath()
        {
            var assembly = typeof(JSSoft.Communication.ClientContextBase).GetTypeInfo().Assembly;
#if NETSTANDARD1_5 || NETSTANDARD2_0
            // Assembly.EscapedCodeBase does not exist under CoreCLR, but assemblies imported from a nuget package
            // don't seem to be shadowed by DNX-based projects at all.
            return assembly.Location;
#else
            // If assembly is shadowed (e.g. in a webapp), EscapedCodeBase is pointing
            // to the original location of the assembly, and Location is pointing
            // to the shadow copy. We care about the original location because
            // the native dlls don't get shadowed.

            var escapedCodeBase = assembly.EscapedCodeBase;
            if (IsFileUri(escapedCodeBase))
            {
                return new Uri(escapedCodeBase).LocalPath;
            }
            return assembly.Location;
#endif
        }

        private static bool IsFileUri(string uri)
        {
            return uri.ToLowerInvariant().StartsWith(Uri.UriSchemeFile);
        }
    }
}