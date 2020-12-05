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

namespace JSSoft.Unity.Terminal
{
    class TerminalBlock
    {
        private TerminalColor?[] foregroundColors = new TerminalColor?[] { };
        private TerminalColor?[] backgroundColors = new TerminalColor?[] { };
        private string text = string.Empty;

        public string Text
        {
            get => this.text;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (this.Text != value)
                {
                    ArrayUtility.Resize(ref this.foregroundColors, value.Length);
                    ArrayUtility.Resize(ref this.backgroundColors, value.Length);
                    this.text = value;
                }
            }
        }

        public void Highlight(ISyntaxHighlighter highlighter, TerminalTextType textType)
        {
            highlighter.Highlight(textType, this.Text, this.foregroundColors, this.backgroundColors);
        }

        public bool Contains(int index)
        {
            return index >= 0 && index < this.text.Length;
        }

        public TerminalColor? GetForegroundColor(int index)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index < this.foregroundColors.Length)
                return this.foregroundColors[index];
            return null;
        }

        public TerminalColor? GetBackgroundColor(int index)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index < this.backgroundColors.Length)
                return this.backgroundColors[index];
            return null;
        }

        public void SetForegroundColor(int index, TerminalColor? color)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.foregroundColors[index] = color;
        }

        public void SetBackgroundColor(int index, TerminalColor? color)
        {
            if (index < 0 || index >= this.text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.backgroundColors[index] = color;
        }
    }
}