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

                { "SlidingController.grid", "Indicates the grid that will be focused when the target layout is displayed." },
                { "SlidingController.direction", "Indicates the direction in which the target layout is displayed or disappeared." },
                { "SlidingController.Show", "Set the target layout to Displayed." },
                { "SlidingController.Hide", "Set the target layout to Invisible." },
                { "SlidingController.Reset", "Initializes the layout status." },
                { "TerminalSlidingController.keyCode", "Indicates the key to which the action is to be performed." },
                { "TerminalSlidingController.modifiers", "Indicates the 'Modifiers' key to which the action is to be performed." },

                { "TerminalDockController.dock", "Indicates where the terminal will be docked." },
                { "TerminalDockController.isRatio", "Indicates whether the unit value that the terminal occupies in the layout is in ratio or length." },
                { "TerminalDockController.length", "Indicates the length that the terminal occupies in the layout." },
                { "TerminalDockController.ratio", "Indicates the percentage of the terminal in the layout." },

                { "TerminalHostBase.isAsync", "Indicates whether to run the command asynchronously. If the value is 'false', the asynchronous command cannot be executed." },
                { "TerminalHostBase.isVerbose", "Indicates whether more detailed information is printed when an exception or error message is printed." },
                { "TerminalHostBase.exceptionRedirection", "Indicates whether the message is redirected using Debug.LogException when an exception occurs." },
                { "TerminalHostBase.useExceptionForegroundColor", "Indicates whether text colors are used for displayed error messages in the terminal." },
                { "TerminalHostBase.useExceptionBackgroundColor", "Indicates whether background color is used for displayed error messages in the terminal." },
                { "TerminalHostBase.exceptionForegroundColor", "Indicates the text color in the error message displayed in the terminal." },
                { "TerminalHostBase.exceptionBackgroundColor", "Indicates background color for error messages displayed in the terminal." },

                { "CommandContextHost.text", "Indicates the message to be displayed when the terminal starts." },
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

                { "SlidingController.grid", "대상 레이아웃이 표시될때 포커스될 그리드를 나타냅니다." },
                { "SlidingController.direction", "대상 레이아웃이 표시되거나 사라질때의 방향을 나타냅니다." },
                { "SlidingController.Show", "대상 레이아웃을 표시됨으로 설정합니다." },
                { "SlidingController.Hide", "대상 레이아웃을 안보임으로 설정합니다." },
                { "SlidingController.Reset", "레이아웃 상태를 초기화합니다." },
                { "TerminalSlidingController.keyCode", "해당 동작이 수행될 키를 나타냅니다." },
                { "TerminalSlidingController.modifiers", "해당 동작이 수행될 'Modifiers' 키를 나타냅니다." },

                { "TerminalDockController.dock", "터미널이 도킹될 위치를 나타냅니다." },
                { "TerminalDockController.isRatio", "터미널이 레이아웃에서 차지하는 단위값을 비율로 할지 길이로 할지에 대한 여부를 나타냅니다." },
                { "TerminalDockController.length", "터미널이 레이아웃에서 차지하는 길이를 나타냅니다." },
                { "TerminalDockController.ratio", "터미널이 레이아웃에서 차지하는 비율을 나타냅니다." },

                { "TerminalHostBase.isAsync", "명령의 실행을 비동기로 실행할지에 대한 여부를 나타냅니다. 값이 'false' 일 경우 비동기 명령어는 실행할 수 없습니다." },
                { "TerminalHostBase.isVerbose", "예외나 에러 메세지가 출력될때 더 자세한 정보까지 출력되는지에 대한 여부를 나타냅니다." },
                { "TerminalHostBase.exceptionRedirection", "예외가 발생할때 메세지가 Debug.LogException 를 사용하여 리디렉션이 될지에 대한 여부를 나타냅니다." },
                { "TerminalHostBase.useExceptionForegroundColor", "터미널의 표시되는 에러 메세지에 문자 색상을 사용할지에 대한 여부를 나타냅니다." },
                { "TerminalHostBase.useExceptionBackgroundColor", "터미널의 표시되는 에러 메세지에 배경색을 사용할지에 대한 여부를 나타냅니다." },
                { "TerminalHostBase.exceptionForegroundColor", "터미널의 표시되는 에러 메세지에 문자 색상을 나타냅니다." },
                { "TerminalHostBase.exceptionBackgroundColor", "터미널의 표시되는 에러 메세지에 배경색을 나타냅니다." },

                { "CommandContextHost.text", "터미널이 시작될때 표시될 메세지를 나타냅니다." },
            });
        }

        public static string GetString(string id)
        {
            var cultureInfo = CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;
            return GetString(id, cultureInfo);
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