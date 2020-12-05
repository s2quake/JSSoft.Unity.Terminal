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

using System;
using System.Collections.Generic;

namespace JSSoft.Unity.Terminal
{
    class PropertyNotifier : IDisposable
    {
        private readonly Action<string> action;
        private readonly List<string> propertyList = new List<string>();

        public PropertyNotifier(Action<string> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Begin()
        {
            this.Begin(0);
        }

        public void Begin(int capacity)
        {
            this.propertyList.Clear();
            if (capacity > this.propertyList.Capacity)
            {
                this.propertyList.Capacity = capacity;
            }
        }

        public void SetField<T>(ref T field, T value, string propertyName)
        {
            if (object.Equals(field, value) == false)
            {
                field = value;
                this.propertyList.Add(propertyName);
            }
        }

        public void End()
        {
            foreach (var item in this.propertyList)
            {
                this.action(item);
            }
            this.propertyList.Clear();
        }

        public void Dispose()
        {
            this.End();
        }
    }
}