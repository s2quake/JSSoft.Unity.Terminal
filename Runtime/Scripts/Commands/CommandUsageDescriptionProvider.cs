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