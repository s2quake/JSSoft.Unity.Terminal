////////////////////////////////////////////////////////////////////////////////
//                                                                              
// ██╗   ██╗   ████████╗███████╗██████╗ ███╗   ███╗██╗███╗   ██╗ █████╗ ██╗     
// ██║   ██║   ╚══██╔══╝██╔════╝██╔══██╗████╗ ████║██║████╗  ██║██╔══██╗██║     
// ██║   ██║█████╗██║   █████╗  ██████╔╝██╔████╔██║██║██╔██╗ ██║███████║██║     
// ██║   ██║╚════╝██║   ██╔══╝  ██╔══██╗██║╚██╔╝██║██║██║╚██╗██║██╔══██║██║     
// ╚██████╔╝      ██║   ███████╗██║  ██║██║ ╚═╝ ██║██║██║ ╚████║██║  ██║███████╗
//  ╚═════╝       ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝
//
// Copyright (c) 2020 Jeesu Choi
// E-mail: han0210@netsgo.com
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading.Tasks;

namespace JSSoft.Unity.Terminal.Tasks
{
    public static class ITerminalAsync
    {
        public static bool CheckAccess(this ITerminal terminal)
        {
            return terminal.Dispatcher.CheckAccess();
        }

        public static void VerifyAccess(this ITerminal terminal)
        {
            terminal.Dispatcher.VerifyAccess();
        }

        public static TResult Invoke<TResult>(this ITerminal terminal, Func<TResult> callback)
        {
            return terminal.Dispatcher.Invoke<TResult>(callback);
        }

        public static void Invoke(this ITerminal terminal, Action action)
        {
            terminal.Dispatcher.Invoke(action);
        }

        public static Task InvokeAsync(this ITerminal terminal, Action action)
        {
            return terminal.Dispatcher.InvokeAsync(action);
        }

        public static Task InvokeAsync(this ITerminal terminal, Task task)
        {
            return terminal.Dispatcher.InvokeAsync(task);
        }

        public static Task<TResult> InvokeAsync<TResult>(this ITerminal terminal, Task<TResult> task)
        {
            return terminal.Dispatcher.InvokeAsync(task);
        }

        public static Task<TResult> InvokeAsync<TResult>(this ITerminal terminal, Func<TResult> callback)
        {
            return terminal.Dispatcher.InvokeAsync(callback);
        }
    }
}
