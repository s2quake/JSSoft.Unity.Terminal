using System;
using Ntreev.Library.Threading;
using UnityEngine;

namespace JSSoft.UI
{
    public static class ITerminalExtensions
    {
        public static void AppendLine(this ITerminal terminal, string value)
        {
            terminal.Append(value + Environment.NewLine);
        }
    }
}