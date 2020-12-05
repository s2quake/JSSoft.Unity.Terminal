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
using UnityEngine;

namespace JSSoft.Unity.Terminal
{
    public abstract class TerminalKeyboardBase : ITerminalKeyboard
    {
        private ITerminalGrid grid;
        private string text;
        private RangeInt selection;
        private Rect area;
        private bool isOpened;

        protected TerminalKeyboardBase()
        {
            this.Opened += (s, e) => Current = this;
            this.Done += (s, e) => Current = null;
            this.Canceled += (s, e) => Current = null;
        }

        public void Open(ITerminalGrid grid, string text)
        {
            if (this.isOpened == true)
                throw new InvalidOperationException("keyboard already open.");
            this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
            this.text = text ?? throw new ArgumentNullException(nameof(text));
            this.selection = new RangeInt() { start = text.Length };
        }

        public void Close()
        {
            if (this.isOpened == false)
                throw new InvalidOperationException("keyboard already closed.");
            this.OnClose();
            this.isOpened = false;
            this.grid = null;
            this.text = null;
            this.selection = default(RangeInt);
            this.OnCanceled(EventArgs.Empty);
        }

        public void Update()
        {
            if (this.isOpened == false && this.text != null)
            {
                this.OnOpen(this.text);
                this.isOpened = true;
                this.OnOpened(new TerminalKeyboardEventArgs(this.Text, this.Selection, this.Area));
            }
            else if (this.isOpened == true)
            {
                var text = this.Text;
                var selection = this.Selection;
                var area = this.Area;
                var result = this.OnUpdate();
                if (result == true)
                {
                    this.isOpened = false;
                    this.OnDone(new TerminalKeyboardEventArgs(this.text, this.selection, this.area));
                    this.grid = null;
                    this.text = null;
                    this.selection = default(RangeInt);
                    this.area = default(Rect);
                }
                else if (result == false)
                {
                    this.isOpened = false;
                    this.grid = null;
                    this.text = null;
                    this.selection = default(RangeInt);
                    this.OnCanceled(EventArgs.Empty);
                }
                else if (this.text != text
                        || object.Equals(this.selection, selection) == false
                        || object.Equals(this.area, area) == false)
                {
                    this.text = text;
                    this.selection = selection;
                    this.area = area;
                    this.OnChanged(new TerminalKeyboardEventArgs(this.text, this.selection, this.area));
                }
            }
        }

        public bool IsOpened => this.isOpened;

        public abstract string Text { get; set; }

        public abstract RangeInt Selection { get; set; }

        public abstract Rect Area { get; }

        public ITerminalGrid Grid => this.grid;

        public ITerminal Terminal => this.grid?.Terminal;

        public static TerminalKeyboardBase Current { get; private set; }

        public event EventHandler<TerminalKeyboardEventArgs> Opened;

        public event EventHandler<TerminalKeyboardEventArgs> Done;

        public event EventHandler Canceled;

        public event EventHandler<TerminalKeyboardEventArgs> Changed;

        protected abstract void OnOpen(string text);

        protected abstract void OnClose();

        protected abstract bool? OnUpdate();

        protected virtual void OnOpened(TerminalKeyboardEventArgs e)
        {
            this.Opened?.Invoke(this, e);
        }

        protected virtual void OnDone(TerminalKeyboardEventArgs e)
        {
            this.Done?.Invoke(this, e);
        }

        protected virtual void OnCanceled(EventArgs e)
        {
            this.Canceled?.Invoke(this, e);
        }

        protected virtual void OnChanged(TerminalKeyboardEventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }
    }
}
