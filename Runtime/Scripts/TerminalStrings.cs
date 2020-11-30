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


using System.Collections.Generic;
using System.Globalization;

namespace JSSoft.Unity.Terminal
{
    public static class TerminalStrings
    {
        private static readonly Dictionary<string, Dictionary<string, string>> stringsByLocale = new Dictionary<string, Dictionary<string, string>>();

        static TerminalStrings()
        {
            stringsByLocale.Add("common", new Dictionary<string, string>()
            {
                { "Terminal.outputText", "Indicates the output string of the terminal." },
                { "Terminal.prompt", "Indicates the prompt set for the terminal." },
                { "Terminal.command", "Indicates the command entered in the terminal." },
                { "Terminal.isReadOnly", "Sets whether or not read-only. When set to read-only, the input of characters is restricted and the execution of commands is also restricted." },
                { "Terminal.dispatcher", "Object that helps the terminal run tasks on its own thread during asynchronous operations." },

                { "TerminalGrid.style", "Indicates the style of the terminal. If the style is set, some related property settings for the style are restricted." },
                { "TerminalGrid.font", "Indicates the font of the terminal. Generally, the font uses the mono font." },
                { "TerminalGrid.backgroundColor", "Indicates the background color of the terminal." },
                { "TerminalGrid.foregroundColor", "Indicates the text color of the terminal." },
                { "TerminalGrid.selectionColor", "Indicates the background color of the terminal's selection." },
                { "TerminalGrid.selectionTextColor", "Indicates the text color of the terminal's selection." },
                { "TerminalGrid.cursorColor", "Indicates the background color of the cursor in the terminal." },
                { "TerminalGrid.cursorTextColor", "Indicates the text color of the cursor in the terminal." },
                { "TerminalGrid.fallbackTexture", "Indicates the texture for displaying characters not supported by the font." },
                { "TerminalGrid.colorPalette", "Indicates the color palette of the terminal." },
                { "TerminalGrid.cursorStyle", "Indicates the style of the cursor." },
                { "TerminalGrid.cursorThickness", "Indicates the thickness of the cursor when it is in a state where it can be marked as a line." },
                { "TerminalGrid.isCursorBlinkable", "Indicates whether the cursor blinks." },
                { "TerminalGrid.cursorBlinkDelay", "Indicates how long the blinking of the cursor repeats." },
                { "TerminalGrid.isScrollForwardEnabled", "Sets whether the maximum area of the terminal is visible and scrollable." },
                { "TerminalGrid.behaviourList", "Indicates a list of terminal behavior. Users can apply different interactions across terminals by adding new behaviours." },
                { "TerminalGrid.maxBufferHeight", "Indicates the height of the maximum buffer that the terminal can display." },
                { "TerminalGrid.padding", "Indicates the inner margin of the terminal." },
            });

            stringsByLocale.Add("ko-KR", new Dictionary<string, string>()
            {
                { $"Terminal.outputText", "터미널의 출력된 문자열을 나타냅니다." },
                { $"Terminal.prompt", "터미널에 설정된 프롬프트를 나타냅니다." },
                { $"Terminal.command", "터미널에 입력된 명령어를 나타냅니다." },
                { $"Terminal.isReadOnly", "읽기 전용에 대한 여부를 설정합니다. 읽기 전용으로 설정되어 있으면 문자의 입력이 제한되며 명령어 실행 또한 제한됩니다." },
                { $"Terminal.dispatcher", "비동기 작업중에 터미널이 소유된 스레드에서 작업을 실행할 수 있도록 도와주는 객체입니다." },

                { "TerminalGrid.style", "터미널의 스타일을 나타냅니다. 스타일이 설정되어 있으면 스타일의 관련된 일부 속성 설정이 제한됩니다." },
                { "TerminalGrid.font", "터미널의 폰트를 나타냅니다. 일반적으로 폰트는 mono 폰트를 사용합니다." },
                { "TerminalGrid.backgroundColor", "터미널의 배경색을 나타냅니다." },
                { "TerminalGrid.foregroundColor", "터미널의 글자 색상을 나타냅니다." },
                { "TerminalGrid.selectionColor", "터미널의 선택영역의 배경색을 나타냅니다." },
                { "TerminalGrid.selectionTextColor", "터미널의 선택영역의 글자 색상을 나타냅니다." },
                { "TerminalGrid.cursorColor", "터미널의 커서의 배경색을 나타냅니다." },
                { "TerminalGrid.cursorTextColor", "터미널의 커서의 글자 색상을 나타냅니다." },
                { "TerminalGrid.fallbackTexture", "폰트에서 지원하지 않는 문자를 표시하기 위한 텍스쳐를 나타냅니다." },
                { "TerminalGrid.colorPalette", "터미널의 색상 팔레트를 나타냅니다." },
                { "TerminalGrid.cursorStyle", "커서의 스타일을 나타냅니다." },
                { "TerminalGrid.cursorThickness", "커서가 선으로 표시될 수 있는 상태였을때 커서의 굵기를 나타냅니다." },
                { "TerminalGrid.isCursorBlinkable", "커서의 깜빡임 여부를 나타냅니다." },
                { "TerminalGrid.cursorBlinkDelay", "커서의 깜빡임이 반복되는 시간을 나타냅니다." },
                { "TerminalGrid.isScrollForwardEnabled", "터미널의 최대 영역을 표시하고 스크롤할 수 있는지에 대한 여부를 설정합니다." },
                { "TerminalGrid.behaviourList", "터미널의 동작의 목록을 나타냅니다. 사용자는 새로운 동작을 추가함으로써 터미널마다 다양한 상호작용을 적용할 수 있습니다." },
                { "TerminalGrid.maxBufferHeight", "터미널이 표시할 수 있는 최대 버퍼의 높이를 나타냅니다." },
                { "TerminalGrid.padding", "터미널의 안쪽 여백을 나타냅니다." },
            });
        }

        public static string GetString(string id, CultureInfo cultureInfo)
        {
            if (stringsByLocale.ContainsKey(cultureInfo.Name) == true)
            {
                var strings = stringsByLocale[cultureInfo.Name];
                if (strings.ContainsKey(id) == true)
                    return IdentifyString(strings[id], cultureInfo);
            }
            {
                var strings = stringsByLocale["common"];
                if (strings.ContainsKey(id) == true)
                    return IdentifyString(strings[id], cultureInfo);
            }
            return string.Empty;
        }

        private static string IdentifyString(string text, CultureInfo cultureInfo)
        {
            if (text.StartsWith("&") == true)
            {
                var id = text.Substring(1);
                return GetString(id, cultureInfo);
            }
            return text;
        }
    }
}