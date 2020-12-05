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
using UnityEngine.EventSystems;

namespace JSSoft.Unity.Terminal
{
    public abstract class TerminalBase : UIBehaviour, ITerminal
    {
        public abstract string Command { get; set; }

        public abstract string Prompt { get; set; }

        public abstract string OutputText { get; }

        public abstract string PromptText { get; }

        public abstract string Text { get; }

        public abstract string ProgressText { get; }

        public abstract int CursorPosition { get; set; }

        public abstract bool IsReadOnly { get; set; }

        public abstract bool IsExecuting { get; }

        public abstract TerminalColor? ForegroundColor { get; set; }

        public abstract TerminalColor? BackgroundColor { get; set; }

        public abstract ICommandCompletor CommandCompletor { get; set; }

        public abstract ISyntaxHighlighter SyntaxHighlighter { get; set; }

        public abstract IProgressGenerator ProgressGenerator { get; set; }

        public abstract TerminalDispatcher Dispatcher { get; }

        public abstract event EventHandler Validated;

        public abstract event EventHandler Enabled;

        public abstract event EventHandler Disabled;

        public abstract event EventHandler CancellationRequested;

        public abstract event EventHandler<TerminalExecuteEventArgs> Executing;

        public abstract event EventHandler<TerminalExecutedEventArgs> Executed;

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract event EventHandler<TextChangedEventArgs> TextChanged;

        public abstract void Append(string value);

        public abstract void Backspace();

        public abstract void Delete();

        public abstract void Execute();

        public abstract void MoveLeft();

        public abstract void MoveRight();

        public abstract void MoveToFirst();

        public abstract void MoveToLast();

        public abstract void NextCompletion();

        public abstract void NextHistory();

        public abstract void PrevCompletion();

        public abstract void PrevHistory();

        public abstract void ResetColor();

        public abstract void ResetOutput();

        public abstract void Cancel();

        public abstract string Progress(string message, float value);

        public abstract TerminalData Save();

        public abstract void Load(TerminalData data);

        #region ITerminal

        void ITerminal.Reset() => this.ResetOutput();

        GameObject ITerminal.GameObject => this.gameObject;

        #endregion
    }
}
