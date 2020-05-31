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
using System.Threading.Tasks;

namespace JSSoft.Terminal.Tasks
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
