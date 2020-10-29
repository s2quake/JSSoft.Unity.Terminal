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
using System.Text;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [CommandSummary(CommandStrings.VersionCommand.Summary)]
    [CommandSummary(CommandStrings.VersionCommand.Summary_ko_KR, Locale = "ko-KR")]
    [VersionCommand]
    public class VersionCommand : CommandBase
    {
        protected override void OnExecute()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Identifier: {Application.identifier}");
            sb.AppendLine($"Version: {Application.version}");
            sb.AppendLine($"ProductName: {Application.productName}");
            sb.AppendLine($"Unity Version: {Application.unityVersion}");
            this.Out.WriteLine(sb.ToString());
        }
    }
}
