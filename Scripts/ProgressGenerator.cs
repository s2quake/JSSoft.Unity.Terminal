// MIT License
// 
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
