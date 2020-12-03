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

using System.Text;

namespace JSSoft.Unity.Terminal
{
    public class ProgressGenerator : IProgressGenerator
    {
        public ProgressGenerator()
        {
        }

        public ProgressGenerator(ITerminalGrid grid)
        {
            this.Grid = grid;
        }

        public string Generate(string message, float value)
        {
            if (this.Grid != null)
            {
                return this.GeneratePattern2(message, value);
            }
            return message;
        }

        public ITerminalGrid Grid { get; }

        // ProgressMessage: 
        // 100%|#############################]
        private string GeneratePattern1(string message, float value)
        {
            var width = this.Grid.BufferWidth;
            var percent = $"{(int)(value * 100),3:D}%|";
            var w = (int)((width - percent.Length) * value);
            var progress = "#".PadRight(w, '#');
            var sb = new StringBuilder();
            sb.AppendLine(message);
            sb.Append($"{percent}{progress}");
            return sb.ToString();
        }

        // ProgressMessage...: [100%] [#############################]
        private string GeneratePattern2(string message, float value)
        {
            var width = this.Grid.BufferWidth;
            var text = $"{message}: ";
            var percent = $"[{(int)(value * 100),3:D}%] ";
            var column = (int)((width - text.Length - percent.Length - 2));
            var w = (int)(column * value);
            var progress = "#".PadRight(w, '#').PadRight(column, ' ');
            return $"{text}{percent}[{progress}]";
        }
    }
}
