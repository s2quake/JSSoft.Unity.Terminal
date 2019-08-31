using System;
using Ntreev.Library.Threading;

namespace JSSoft.UI
{
    public interface ITerminal
    {
        void Append(string value);

        void Reset();

        string Command { get; }

        string Prompt { get; set; }

        Dispatcher Dispatcher {get;}
    }
}
