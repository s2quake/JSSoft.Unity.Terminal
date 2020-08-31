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

using System;
using System.Threading.Tasks;

namespace JSSoft.Terminal.Tasks
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
