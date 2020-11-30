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
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [VersionCommand]
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class VersionCommand : CommandBase
    {
        protected override void OnExecute()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Identifier: {Application.identifier}");
            sb.AppendLine($"Application Version: {Application.version}");
            sb.AppendLine($"ProductName: {Application.productName}");
            sb.AppendLine($"Unity Version: {Application.unityVersion}");
            sb.AppendLine($"u-terminal(file): {GetTerminalFileVersion()}");
            sb.AppendLine($"u-terminal(info): {GetTerminalInfoVersion()}");
            this.Out.WriteLine(sb.ToString());
        }

        private static string GetTerminalFileVersion()
        {
            var attr = Attribute.GetCustomAttribute(typeof(Terminal).Assembly, typeof(AssemblyFileVersionAttribute));
            if (attr is AssemblyFileVersionAttribute versionAttribute)
                return versionAttribute.Version;
            return "none version";
        }

        private static string GetTerminalInfoVersion()
        {
            var attr = Attribute.GetCustomAttribute(typeof(Terminal).Assembly, typeof(AssemblyInformationalVersionAttribute));
            if (attr is AssemblyInformationalVersionAttribute versionAttribute)
                return versionAttribute.InformationalVersion;
            return "none version";
        }
    }
}
