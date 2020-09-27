using System;
using System.Reflection;
using JSSoft.Library;

namespace JSSoft.Unity.Terminal.Commands
{
    public interface ICommandConfiguration
    {
        Type Type { get; }

        string Name { get; }

        string Comment { get; set; }

        object DefaultValue { get; set; }

        object Value { get; set; }

        event EventHandler Changed;
    }
}
