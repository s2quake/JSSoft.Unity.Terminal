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
using UnityEngine;
using JSSoft.Library.Threading;

namespace JSSoft.Terminal
{
    public class TerminalDispatcher : MonoBehaviour
    {
        private DispatcherScheduler scheduler;
        private Dispatcher dispatcher;

        public bool CheckAccess()
        {
            this.Validate();
            return this.dispatcher.CheckAccess();
        }

        public void VerifyAccess()
        {
            this.Validate();
            this.dispatcher.VerifyAccess();
        }

        public TResult Invoke<TResult>(Func<TResult> callback)
        {
            this.Validate();
            return this.dispatcher.Invoke<TResult>(callback);
        }

        public void Invoke(Action action)
        {
            this.Validate();
            this.dispatcher.Invoke(action);
        }

        public Task InvokeAsync(Action action)
        {
            this.Validate();
            return this.dispatcher.InvokeAsync(action);
        }

        public Task InvokeAsync(Task task)
        {
            this.Validate();
            return this.dispatcher.InvokeAsync(task);
        }

        public Task<TResult> InvokeAsync<TResult>(Task<TResult> task)
        {
            this.Validate();
            return this.dispatcher.InvokeAsync(task);
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            this.Validate();
            return this.dispatcher.InvokeAsync(callback);
        }

        protected virtual void Awake()
        {
            this.scheduler = DispatcherScheduler.Current;
            this.dispatcher = Dispatcher.Current;
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying && this.scheduler != null)
#endif
            {
                var time = DateTime.Now;
                var count = this.scheduler.Process(1000 / 60);
                // var count = this.scheduler.ProcessAll();
                if (count > 0)
                {
                    // Debug.Log(DateTime.Now - time);
                }
            }
        }

        private void Validate()
        {
            if (this.dispatcher == null)
                throw new InvalidOperationException();
        }
    }
}
