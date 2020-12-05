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
// Site  : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

using JSSoft.Library.Threading;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace JSSoft.Unity.Terminal
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
            if (Application.isEditor == true)
            {
                if (Application.isPlaying && this.scheduler != null)
                    this.Process();
            }
            else
            {
                this.Process();
            }
        }
        
        private void Process()
        {
            var time = DateTime.Now;
            var count = this.scheduler.Process(1000 / 60);
            // var count = this.scheduler.ProcessAll();
            if (count > 0)
            {
                // Debug.Log(DateTime.Now - time);
            }
        }

        private void Validate()
        {
            if (this.dispatcher == null)
                throw new InvalidOperationException();
        }
    }
}
