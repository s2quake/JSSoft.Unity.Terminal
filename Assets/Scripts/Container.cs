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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ntreev.Library.Commands;
using JSSoft.Communication.Commands;
using JSSoft.Communication.Services;
using JSSoft.UI;
using Terminal = JSSoft.UI.Terminal;

namespace JSSoft.Communication.Shells
{
    static class Container
    {
        private static readonly List<ICommand> commandList = new List<ICommand>();
        private static Shell shell;
        private static CommandContext commandContext;
        private static ClientContext serviceContext;
        private static IServiceHost[] serviceHosts;
        private static Terminal terminal;

        static Container()
        {
            var lazyTerminal = new Lazy<ITerminal>(GetTerminal);
            var lazyIShell = new Lazy<IShell>(GetShell);
            var lazyShell = new Lazy<Shell>(GetShell);
            var userService = new UserService();
            var userServiceHost = new UserServiceHost(userService);
            var lazyUserService = new Lazy<IUserService>(() => userService);
            var dataService = new DataService();
            var dataServiceHost = new DataServiceHost(dataService);
            var lazyDataService = new Lazy<IDataService>(() => dataService);
            serviceHosts = new IServiceHost[] { userServiceHost, dataServiceHost };
            serviceContext = new ClientContext(serviceHosts);

            commandList.Add(new ResetCommand(lazyTerminal));
            commandList.Add(new CloseCommand(serviceContext, lazyShell));
            commandList.Add(new ExitCommand(lazyIShell));
            commandList.Add(new LoginCommand(lazyShell, lazyUserService));
            commandList.Add(new LogoutCommand(lazyShell, lazyUserService));
            commandList.Add(new OpenCommand(serviceContext, lazyShell));
            commandList.Add(new UserCommand(lazyShell, lazyUserService));
            commandList.Add(new DataCommand(lazyShell, lazyDataService));
            commandContext = new CommandContext(commandList, Enumerable.Empty<ICommandProvider>());
            commandContext.Name = "UnityCommand";
            commandContext.VerifyName = false;
            terminal = UnityEngine.GameObject.FindObjectOfType<Terminal>();
            shell = new Shell(commandContext, serviceContext, userService, terminal);
        }

        public static T GetService<T>() where T : class
        {
            if (typeof(T) == typeof(IShell))
            {
                return shell as T;
            }
            else if (typeof(T) == typeof(CommandContext))
            {
                return commandContext as T;
            }
            else if (typeof(T) == typeof(ITerminal) || typeof(T) == typeof(Terminal))
            {
                return terminal as T;
            }
            throw new NotImplementedException();
        }

        public static object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public static void Release()
        {

        }

        public static ITerminal GetTerminal() => terminal;

        private static Shell GetShell() => shell;
    }
}
