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


using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using JSSoft.Library.Commands;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandUsageDescriptionProvider : IUsageDescriptionProvider
    {
        public string GetDescription(object instance)
        {
            return string.Empty;
        }

        public string GetDescription(PropertyInfo propertyInfo)
        {
            return string.Empty;
        }

        public string GetDescription(ParameterInfo parameterInfo)
        {
            return string.Empty;
        }

        public string GetDescription(MethodInfo methodInfo)
        {
            return string.Empty;
        }

        public string GetExample(object instance)
        {
            var id = $"Example:{instance.GetType().Name}";
            return CommandStrings.GetString(id, CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture);
        }

        public string GetExample(MethodInfo methodInfo)
        {
            var type = methodInfo.DeclaringType;
            var name = methodInfo.Name;
            var id = $"Example:{type.Name}.{name}";
            if (type.DeclaringType != null)
                id = $"{type.DeclaringType.Name}.{id}";
            return CommandStrings.GetString(id, CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture);
        }

        public string GetSummary(object instance)
        {
            var id = instance.GetType().Name;
            return CommandStrings.GetString(id, CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture);
        }

        public string GetSummary(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.DeclaringType;
            var name = propertyInfo.Name;
            var id = $"{type.Name}.{name}";
            if (type.DeclaringType != null)
                id = $"{type.DeclaringType.Name}.{id}";
            return CommandStrings.GetString(id, CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture);
        }

        public string GetSummary(ParameterInfo parameterInfo)
        {
            var method = parameterInfo.Member;
            var type = method.DeclaringType;
            var name = parameterInfo.Name;
            var id = $"{type.Name}.{method.Name}.{name}";
            return CommandStrings.GetString(id, CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture);
        }

        public string GetSummary(MethodInfo methodInfo)
        {
            var type = methodInfo.DeclaringType;
            var name = methodInfo.Name;
            var id = $"{type.Name}.{name}";
            if (type.DeclaringType != null)
                id = $"{type.DeclaringType.Name}.{id}";
            return CommandStrings.GetString(id, CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture);
        }
    }
}