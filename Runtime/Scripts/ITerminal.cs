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
using System.ComponentModel;
using UnityEngine;

namespace JSSoft.Unity.Terminal
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

        void Execute();

        void MoveToFirst();

        void MoveToLast();

        void MoveLeft();

        void MoveRight();

        void Cancel();

        string Progress(string message, float value);

        TerminalData Save();

        void Load(TerminalData data);

        string Command { get; set; }

        string Prompt { get; set; }

        string OutputText { get; }

        string PromptText { get; }

        string Text { get; }

        string ProgressText { get; }

        int CursorPosition { get; set; }

        bool IsReadOnly { get; set; }

        bool IsExecuting { get; }

        TerminalColor? ForegroundColor { get; set; }

        TerminalColor? BackgroundColor { get; set; }

        ICommandCompletor CommandCompletor { get; set; }

        ISyntaxHighlighter SyntaxHighlighter { get; set; }

        IProgressGenerator ProgressGenerator { get; set; }

        GameObject GameObject { get; }

        TerminalDispatcher Dispatcher { get; }

        event EventHandler Validated;

        event EventHandler Enabled;

        event EventHandler Disabled;

        event EventHandler CancellationRequested;

        event EventHandler<TerminalExecuteEventArgs> Executing;

        event EventHandler<TerminalExecutedEventArgs> Executed;

        event PropertyChangedEventHandler PropertyChanged;

        event EventHandler<TextChangedEventArgs> TextChanged;
    }
}
