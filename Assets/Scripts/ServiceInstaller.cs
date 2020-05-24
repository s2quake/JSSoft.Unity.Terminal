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
using Zenject;
using UnityEngine;

namespace JSSoft.Communication.Shells
{
    class ServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private TerminalGrid grid;

        public override void InstallBindings()
        {
            if (this.grid == null)
                throw new InvalidOperationException("TerminalGrid does not exists.");

            Container.Bind(typeof(IUserService), typeof(UserService), typeof(INotifyUserService)).To<UserService>().AsSingle();
            Container.Bind(typeof(IDataService), typeof(DataService)).To<DataService>().AsSingle();
            Container.Bind<IServiceHost>().To<UserServiceHost>().AsSingle();
            Container.Bind<IServiceHost>().To<DataServiceHost>().AsSingle();
            Container.Bind<IServiceContext>().To<ClientContext>().AsSingle();

            Container.Bind<ITerminalGrid>().FromInstance(this.grid);
            Container.Bind<ITerminal>().FromInstance(this.grid.Terminal);

            Container.Bind<ICommand>().To<ResetCommand>().AsSingle();
            Container.Bind<ICommand>().To<CloseCommand>().AsSingle();
            Container.Bind<ICommand>().To<ExitCommand>().AsSingle();
            Container.Bind<ICommand>().To<LoginCommand>().AsSingle();
            Container.Bind<ICommand>().To<LogoutCommand>().AsSingle();
            Container.Bind<ICommand>().To<OpenCommand>().AsSingle();
            Container.Bind<ICommand>().To<UserCommand>().AsSingle();
            Container.Bind<ICommand>().To<DataCommand>().AsSingle();
            Container.Bind<ICommand>().To<StyleCommand>().AsSingle();
            Container.Bind<ICommand>().To<VerboseCommand>().AsSingle();
            Container.Bind<ICommand>().To<WidthCommand>().AsSingle();
            Container.Bind<ICommand>().To<HeightCommand>().AsSingle();
            Container.Bind<ICommand>().To<TestCommand>().AsSingle();

            Container.Bind<CommandContext>().AsSingle();
            Container.Bind(typeof(IShell), typeof(Shell)).To<Shell>().AsSingle().Lazy();
        }
    }
}
