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

namespace JSSoft.Unity.Terminal.Commands
{
    public static class CommandStrings
    {
        private static readonly Dictionary<string, Dictionary<string, string>> stringsByLocale = new Dictionary<string, Dictionary<string, string>>();

        static CommandStrings()
        {
            stringsByLocale.Add("common", new Dictionary<string, string>()
            {
                { "ExitCommand", "Exit the application." },
                { "ExitCommand.ExitCode", "Specifies the exit code. The default is 0." },
                { "InfoCommand", "Displays information." },
                { "InfoCommand.PropertyName", "Specifies the name of the property to display. If not specified, all properties are displayed." },
                { "PingCommand", "Ping any given IP address." },
                { "PingCommand.Address", "Specifies the ping destination address." },
                { "PingCommand.Count", "Specifies the number of pings. The default is 3." },
                { "PingCommand.Timeout", "Specify the maximum response time. Default value is 4000 (4 seconds)." },
                { "ResetCommand", "Initialize the terminal." },
                { "ResolutionCommand", "Change resolution properties." },
                { "Example:ResolutionCommand", @"resolution 3
resolution 1024x768
resolution 1024x768@60hz" },
                { "ResolutionCommand.Resolution", "Specifies the index or format of the resolution." },
                { "ResolutionCommand.IsWindowMode", "Change to window mode." },
                { "ResolutionCommand.IsFullScreen", "Change to full screen mode." },
                { "ResolutionCommand.IsList", "Displays a list of supported resolutions." },
                { "StyleCommand", "Change terminal style." },
                { "StyleCommand.IsList", "Displays a list of supported styles." },
                { "StyleCommand.IsRemove", "Removes the currently applied style." },
                { "StyleCommand.StyleName", "Specifies the name of the style to apply. If not specified, displays the name of the currently applied style." },
                { "SceneCommand", "Change scene." },
                { "SceneCommand.IsList", "Displays a list of scene." },
                { "SceneCommand.SceneName", "Specifies the name or index of the scene to load. If not specified, displays the name of the active scene." },
                { "VerboseCommand", "Displays or sets the details for the message." },
                { "VerboseCommand.Value", "Specifies whether to display details. If not specified, displays the current value." },
                { "VersionCommand", "Displays version." },
                { "InternetProtocolCommand", "Displays ip address." },
                { "ConfigCommand", "Get and set properties." },
                { "ConfigCommand.ConfigName", "Specifies the name of the property." },
                { "ConfigCommand.Value", "Specify a value. If no value is specified, displays the value of the property." },
                { "ConfigCommand.ListPattern", "Displays the names and values of all properties." },
                { "ConfigCommand.DetailSwitch", "Displays details." },
                { "ConfigCommand.ResetSwitch", "Returns the value of the property to its default value." },
                { "DateCommand", "Displays the date and time." },
                { "DateCommand.Format", "Specifies the format to display the date and time." },
                { "DateCommand.Locale", "Specifies the culture to display the date and time." },
                { "DateCommand.UTC", "Displays the date and time as UTC." },
                { "ComponentCommand", "Provides commands for Component." },
                { "ComponentCommand.Add", "Add components to GameObject." },
                { "ComponentCommand.Add.path", "&ComponentCommand.Path" },
                { "ComponentCommand.Remove", "Remove the component from the GameObject." },
                { "ComponentCommand.Remove.path", "&ComponentCommand.Path" },
                { "ComponentCommand.Remove.index", "&ComponentCommand.Index" },
                { "ComponentCommand.Activate", "Activates the component." },
                { "ComponentCommand.Activate.path", "&ComponentCommand.Path" },
                { "ComponentCommand.Activate.index", "&ComponentCommand.Index" },
                { "ComponentCommand.Deactivate", "Deactivates the component." },
                { "ComponentCommand.Deactivate.path", "&ComponentCommand.Path" },
                { "ComponentCommand.Deactivate.index", "&ComponentCommand.Index" },
                { "ComponentCommand.ShowList", "Displays a list of components." },
                { "ComponentCommand.ShowList.path", "&ComponentCommand.Path" },
                { "ComponentCommand.Path", "Specifies the absolute path of the GameObject, such as /Monster/Arm/Hand." },
                { "ComponentCommand.Index", "Specifies the index of the component." },
                { "GameObjectCommand", "Provides commands for GameObject." },
                { "GameObjectCommand.Create", "Create GameObject." },
                { "GameObjectCommand.Create.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.Create.name", "&GameObjectCommand.Name" },
                { "GameObjectCommand.Rename", "Rename GameObject." },
                { "GameObjectCommand.Rename.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.Rename.newName", "&GameObjectCommand.NewName" },
                { "GameObjectCommand.Move", "Move the GameObject to the target object." },
                { "GameObjectCommand.Move.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.Move.parentPath", "&GameObjectCommand.ParentPath" },
                { "GameObjectCommand.Delete", "Delete GameObject." },
                { "GameObjectCommand.Delete.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.Activate", "Activates GameObject." },
                { "GameObjectCommand.Activate.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.Deactivate", "Deactivates GameObject." },
                { "GameObjectCommand.Deactivate.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.ShowList", "Add components to GameObject." },
                { "GameObjectCommand.ShowList.path", "&GameObjectCommand.Path" },
                { "GameObjectCommand.Path", "Specifies the absolute path of the GameObject, such as /Monster/Arm/Hand." },
                { "GameObjectCommand.Name", "Specifies the name of the GameObject to create." },
                { "GameObjectCommand.NewName", "Specifies the new name of the GameObject to change." },
                { "GameObjectCommand.ParentPath", "Specifies the path of the GameObject to be moved." },
                { "GameObjectCommand.IsRecursive", "Recursively displays a list of child objects." },
            });

            stringsByLocale.Add("ko-KR", new Dictionary<string, string>()
            {
                { "ExitCommand", "프로그램을 종료합니다." },
                { "ExitCommand.ExitCode", "종료코드를 나타냅니다. 기본값은 0 입니다." },
                { "InfoCommand", "정보를 표시합니다." },
                { "InfoCommand.PropertyName", "표시할 속성의 이름을 지정합니다. 지정되지 않으면 모든 속성이 표시됩니다." },
                { "PingCommand", "주어진 IP 주소에 PING 메시지를 보냅니다." },
                { "PingCommand.Address", "Ping 대상 주소를 지정합니다." },
                { "PingCommand.Count", "Ping의 횟수를 지정합니다. 기본값은 3 입니다." },
                { "PingCommand.Timeout", "최대 응답 시간을 지정하십시오. 기본값은 4000(4초) 입니다." },
                { "ResetCommand", "터미널을 초기화합니다." },
                { "ResolutionCommand", "해상도 속성을 변경합니다." },
                { "ResolutionCommand.Resolution", "해상도의 색인 또는 서식을 지정합니다." },
                { "ResolutionCommand.IsWindowMode", "창 모드로 변경합니다." },
                { "ResolutionCommand.IsFullScreen", "전체화면 모드로 변경합니다." },
                { "ResolutionCommand.IsList", "지원되는 해상도의 목록을 표시합니다." },
                { "StyleCommand", "터미널 스타일을 변경합니다." },
                { "StyleCommand.IsList", "지원되는 스타일의 목록을 표시합니다." },
                { "StyleCommand.IsRemove", "현재 적용된 스타일을 제거합니다." },
                { "StyleCommand.StyleName", "적용할 스타일의 이름을 지정합니다. 지정되지 않으면 현재 적용된 스타일의 이름을 표시합니다." },
                { "SceneCommand", "장면을 변경합니다." },
                { "SceneCommand.IsList", "장면의 목록을 표시합니다." },
                { "SceneCommand.SceneName", "로드할 장면의 이름 또는 번호를 지정합니다. 지정되지 않으면 현재 로드된 장면의 이름을 표시합니다." },
                { "VerboseCommand", "메시지에 대한 세부 정보를 표시하거나 설정합니다." },
                { "VerboseCommand.Value", "세부 정보 표시의 여부를 지정합니다. 지정되지 않으면 현재 값을 표시합니다." },
                { "VersionCommand", "버전을 표시합니다." },
                { "InternetProtocolCommand", "ip 주소를 표시합니다." },
                { "ConfigCommand", "속성을 가져오거나 설정합니다." },
                { "ConfigCommand.ConfigName", "속성의 이름을 지정합니다." },
                { "ConfigCommand.Value", "값을 지정합니다. 값이 지정되지 않으면 해당 속성의 값을 표시합니다." },
                { "ConfigCommand.ListPattern", "모든 속성의 이름과 값을 표시합니다." },
                { "ConfigCommand.DetailSwitch", "자세히 표시합니다." },
                { "ConfigCommand.ResetSwitch", "속성의 값을 기본값으로 되돌립니다." },
                { "DateCommand", "날짜와 시간을 표시합니다." },
                { "DateCommand.Format", "날짜와 시간을 표시할 서식을 지정합니다." },
                { "DateCommand.Locale", "날짜와 시간을 표시할 문화권을 지정합니다." },
                { "DateCommand.UTC", "날짜와 시간을 UTC로 표시합니다." },
                { "ComponentCommand", "Component 관련 명령어를 제공합니다." },
                { "ComponentCommand.Add", "GameObject 에 컴포넌트를 추가합니다." },
                { "ComponentCommand.Remove", "GameObject 에서 컴포넌트를 제거합니다." },
                { "ComponentCommand.Activate", "컴포넌트를 활성화합니다." },
                { "ComponentCommand.Deactivate", "컴포넌트를 비활성화합니다." },
                { "ComponentCommand.ShowList", "컴포넌트 목록을 표시합니다." },
                { "ComponentCommand.Path", "/Monster/Arm/Hand 처럼 GameObject의 절대 경로를 지정합니다. " },
                { "ComponentCommand.Index", "컴포넌트의 색인을 지정합니다." },
                { "GameObjectCommand", "GameObject 관련 명령어를 제공합니다." },
                { "GameObjectCommand.Create", "GameObject를 생성합니다." },
                { "GameObjectCommand.Rename", "GameObject의 이름을 변경합니다." },
                { "GameObjectCommand.Move", "GameObject를 대상 객체로 이동합니다." },
                { "GameObjectCommand.Delete", "GameObject를 삭제합니다." },
                { "GameObjectCommand.Activate", "GameObject를 활성화합니다." },
                { "GameObjectCommand.Deactivate", "GameObject를 비활성화 합니다." },
                { "GameObjectCommand.ShowList", "GameObject의 목록을 표시합니다." },
                { "GameObjectCommand.Path", "/Monster/Arm/Hand 처럼 GameObject의 절대 경로를 지정합니다. " },
                { "GameObjectCommand.Name", "생성할 GameObject 이름을 지정합니다." },
                { "GameObjectCommand.NewName", "변경할 GameObject 새로운 이름을 지정합니다." },
                { "GameObjectCommand.ParentPath", "이동할 대상이 되는 GameObject 의 경로를 지정합니다." },
                { "GameObjectCommand.IsRecursive", "자식 객체의 목록까지 재귀적으로 표시합니다." },
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