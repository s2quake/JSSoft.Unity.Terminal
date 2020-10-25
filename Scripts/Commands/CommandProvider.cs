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
using System.Collections.Generic;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandProvider : ICommandProvider
    {
        public virtual IEnumerable<ICommand> Provide(ITerminal terminal, ICommandConfigurationProvider configurationProvider)
        {
            yield return new ResetCommand(terminal);
            yield return new InfoCommand(terminal);
            yield return new ExitCommand(terminal);
            yield return new VerboseCommand(terminal);
            yield return new ResolutionCommand(terminal);
            yield return new PingCommand(terminal);
            yield return new StyleCommand(terminal);
            yield return new InternetProtocolCommand(terminal);
            yield return new SceneCommand(terminal);
            yield return new DateCommand(terminal);

            yield return new ConfigCommand(terminal, configurationProvider);
        }
    }
}
