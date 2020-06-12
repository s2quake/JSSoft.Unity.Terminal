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
using System.Collections.Generic;
using JSSoft.Communication;
using JSSoft.Communication.Services;
using JSSoft.Communication.Services.Server;

namespace JSSoft.Communication.Services
{
    static class ServiceContextHostEvents
    {
        private static readonly HashSet<ServiceContextHost> serviceContexts = new HashSet<ServiceContextHost>();

        public static void Register(ServiceContextHost serviceContext)
        {
            if (serviceContext == null)
                throw new ArgumentNullException(nameof(serviceContext));
            if (serviceContexts.Contains(serviceContext) == true)
                throw new ArgumentException($"{nameof(serviceContext)} is already exists.");
            serviceContexts.Add(serviceContext);
            serviceContext.Opened += ServiceContextHost_Opened;
            serviceContext.Closed += ServiceContextHost_Closed;
        }

        public static void Unregister(ServiceContextHost serviceContext)
        {
            if (serviceContext == null)
                throw new ArgumentNullException(nameof(serviceContext));
            if (serviceContexts.Contains(serviceContext) == false)
                throw new ArgumentException($"{nameof(serviceContext)} does not exists.");
            serviceContext.Opened -= ServiceContextHost_Opened;
            serviceContext.Closed -= ServiceContextHost_Closed;
            serviceContexts.Remove(serviceContext);
        }

        public static event EventHandler Opened;

        public static event EventHandler Closed;

        private static void ServiceContextHost_Opened(object sender, EventArgs e)
        {
            Opened?.Invoke(sender, e);
        }

        private static void ServiceContextHost_Closed(object sender, EventArgs e)
        {
            Closed?.Invoke(sender, e);
        }
    }
}
