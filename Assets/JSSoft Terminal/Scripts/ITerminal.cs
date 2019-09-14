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

        void Delete();

        void Backspace();

        void NextCompletion();

        void PrevCompletion();

        void NextHistory();

        void PrevHistory();

        void Focus();

        string Command { get; }

        string Prompt { get; set; }

        string OutputText { get; }

        int CursorPosition { get; set; }

        Color32? ForegroundColor { get; set; }

        Color32? BackgroundColor { get; set; }

        OnCompletion onCompletion { get; set; }

        OnDrawPrompt onDrawPrompt { get; set; }

        event EventHandler<TerminalExecuteEventArgs> Executed;
    }

    public delegate string[] OnCompletion(string[] items, string find);

    public delegate void OnDrawPrompt(string prompt, Color32?[] foregroundColors, Color32?[] backgroundColors);
}
