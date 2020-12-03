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
// Site : https://s2quake.github.io/u-terminal
// 
////////////////////////////////////////////////////////////////////////////////

namespace JSSoft.Unity.Terminal
{
    public class SyntaxHighlighter : ISyntaxHighlighter
    {
        public void Highlight(TerminalTextType textType, string text, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
            switch (textType)
            {
                case TerminalTextType.Command:
                    {
                        this.HighlightCommand(text, foregroundColors, backgroundColors);
                    }
                    break;
                case TerminalTextType.Progress:
                    {
                        this.HighlightProgress(text, foregroundColors, backgroundColors);
                    }
                    break;
            }
        }

        public static SyntaxHighlighter Default { get; } = new SyntaxHighlighter();

        private void HighlightCommand(string text, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
        }

        private void HighlightProgress(string text, TerminalColor?[] foregroundColors, TerminalColor?[] backgroundColors)
        {
        }
    }
}
