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
    public static class ITerminalGridAsync
    {
        public static bool CheckAccess(this ITerminalGrid grid)
        {
            return grid.Dispatcher.CheckAccess();
        }

        public static void VerifyAccess(this ITerminalGrid grid)
        {
            grid.Dispatcher.VerifyAccess();
        }

        public static TResult Invoke<TResult>(this ITerminalGrid grid, Func<TResult> callback)
        {
            return grid.Dispatcher.Invoke<TResult>(callback);
        }

        public static void Invoke(this ITerminalGrid grid, Action action)
        {
            grid.Dispatcher.Invoke(action);
        }

        public static Task InvokeAsync(this ITerminalGrid grid, Action action)
        {
            return grid.Dispatcher.InvokeAsync(action);
        }

        public static Task InvokeAsync(this ITerminalGrid grid, Task task)
        {
            return grid.Dispatcher.InvokeAsync(task);
        }

        public static Task<TResult> InvokeAsync<TResult>(this ITerminalGrid grid, Task<TResult> task)
        {
            return grid.Dispatcher.InvokeAsync(task);
        }

        public static Task<TResult> InvokeAsync<TResult>(this ITerminalGrid grid, Func<TResult> callback)
        {
            return grid.Dispatcher.InvokeAsync(callback);
        }
    }
}
