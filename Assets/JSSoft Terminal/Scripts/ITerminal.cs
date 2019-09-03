using System;
using Ntreev.Library.Threading;
using UnityEngine;

namespace JSSoft.UI
{
    public interface ITerminal
    {
        void Append(string value);

        void Reset();

        void ResetColor();

        string Command { get; }

        string Prompt { get; set; }

        Color32? ForegroundColor { get; set; }

        Color32? BackgroundColor { get; set; }
    }
}
