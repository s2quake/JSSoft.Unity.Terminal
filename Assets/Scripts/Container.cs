using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ntreev.Library.Commands;
using JSSoft.Communication.Commands;
using JSSoft.Communication.Services;

namespace JSSoft.Communication.Shells
{
    static class Container
    {
        private static readonly List<ICommand> commandList = new List<ICommand>();
        private static Shell shell;
        private static CommandContext commandContext;
        private static ClientContext serviceContext;
        private static IServiceHost[] serviceHosts;

        static Container()
        {
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

            commandList.Add(new CloseCommand(serviceContext, lazyShell));
            commandList.Add(new ExitCommand(lazyIShell));
            commandList.Add(new LoginCommand(lazyShell, lazyUserService));
            commandList.Add(new LogoutCommand(lazyShell, lazyUserService));
            commandList.Add(new OpenCommand(serviceContext, lazyShell));
            commandList.Add(new UserCommand(lazyShell, lazyUserService));
            commandList.Add(new DataCommand(lazyShell, lazyDataService));
            commandContext = new CommandContext(commandList, Enumerable.Empty<ICommandProvider>());
            shell = new Shell(commandContext, serviceContext, userService);
        }

        public static T GetService<T>() where T : class
        {
            if (typeof(T) == typeof(IShell))
            {
                return shell as T;
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

        private static Shell GetShell() => shell;
    }
}
