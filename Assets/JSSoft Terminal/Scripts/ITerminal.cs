using System;

namespace JSSoft.UI
{
    public interface ITerminal
    {
        string Command { get; }

        string Prompt { get; set; }
    }
}
