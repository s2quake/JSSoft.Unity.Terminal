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
using System.ComponentModel;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalValidationEvents
    {
        private static readonly HashSet<INotifyValidated> objs = new HashSet<INotifyValidated>();

        public static void Register(INotifyValidated obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (objs.Contains(obj) == true)
                return;
            objs.Add(obj);
            obj.Enabled += Object_Enabled;
            obj.Disabled += Object_Disabled;
            obj.Validated += Object_Validated;
            obj.PropertyChanged += Object_PropertyChanged;
        }

        public static void Unregister(INotifyValidated obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (objs.Contains(obj) == false)
                return;
            obj.Enabled -= Object_Enabled;
            obj.Disabled -= Object_Disabled;
            obj.Validated -= Object_Validated;
            obj.PropertyChanged -= Object_PropertyChanged;
            objs.Remove(obj);
        }

        public static event EventHandler Enabled;

        public static event EventHandler Disabled;

        public static event EventHandler Validated;

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void Object_Enabled(object sender, EventArgs e)
        {
            Enabled?.Invoke(sender, e);
        }

        private static void Object_Disabled(object sender, EventArgs e)
        {
            Disabled?.Invoke(sender, e);
        }

        private static void Object_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(sender, e);
        }

        private static void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
