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


namespace JSSoft.Unity.Terminal.Commands
{
    public static class CommandStrings
    {
        public static class ExitCommand
        {
            public const string Summary = "Exit the application.";
            public const string Summary_ko_KR = "프로그램을 종료합니다.";

            public static class ExitCode
            {
                public const string Summary = "Specifies the exit code. The default is 0.";
                public const string Summary_ko_KR = "종료코드를 나타냅니다. 기본값은 0 입니다.";
            }
        }

        public static class InfoCommand
        {
            public const string Summary = "Displays information.";
            public const string Summary_ko_KR = "정보를 표시합니다.";

            public static class PropertyName
            {
                public const string Summary = "Specifies the name of the property to display. If not specified, all properties are displayed.";
                public const string Summary_ko_KR = "표시할 속성의 이름을 지정합니다. 지정되지 않으면 모든 속성이 표시됩니다.";
            }
        }

        public static class PingCommand
        {
            public const string Summary = "Ping any given IP address.";
            public const string Summary_ko_KR = "주어진 IP 주소에 PING 메시지를 보냅니다.";

            public static class Address
            {
                public const string Summary = "Specifies the ping destination address.";
                public const string Summary_ko_KR = "Ping 대상 주소를 지정합니다.";
            }

            public static class Count
            {
                public const string Summary = "Specifies the number of pings. The default is 3.";
                public const string Summary_ko_KR = "Ping의 횟수를 지정합니다. 기본값은 3 입니다.";
            }

            public static class Timeout
            {
                public const string Summary = "Specify the maximum response time. Default value is 4000 (4 seconds).";
                public const string Summary_ko_KR = "최대 응답 시간을 지정하십시오. 기본값은 4000(4초) 입니다.";
            }
        }

        public static class ResetCommand
        {
            public const string Summary = "Initialize the terminal.";
            public const string Summary_ko_KR = "터미널을 초기화합니다.";
        }

        public static class ResolutionCommand
        {
            public const string Summary = "Change resolution properties.";
            public const string Summary_ko_KR = "해상도 속성을 변경합니다.";

            public const string Example = @"resolution 3
resolution 1024x768
resolution 1024x768@60hz";

            public static class Resolution
            {
                public const string Summary = @"Specifies the index or format of the resolution.";
                public const string Summary_ko_KR = @"해상도의 색인 또는 서식을 지정합니다.";

            }

            public static class IsWindowMode
            {
                public const string Summary = "Change to window mode.";
                public const string Summary_ko_KR = "창 모드로 변경합니다.";
            }

            public static class IsFullScreen
            {
                public const string Summary = "Change to full screen mode.";
                public const string Summary_ko_KR = "전체화면 모드로 변경합니다.";
            }

            public static class IsList
            {
                public const string Summary = "Displays a list of supported resolutions.";
                public const string Summary_ko_KR = "지원되는 해상도의 목록을 표시합니다.";
            }
        }

        public static class StyleCommand
        {
            public const string Summary = "Change terminal style.";
            public const string Summary_ko_KR = "터미널 스타일을 변경합니다.";

            public static class IsList
            {
                public const string Summary = "Displays a list of supported styles.";
                public const string Summary_ko_KR = "지원되는 스타일의 목록을 표시합니다.";
            }

            public static class IsRemove
            {
                public const string Summary = "Removes the currently applied style.";
                public const string Summary_ko_KR = "현재 적용된 스타일을 제거합니다.";
            }

            public static class StyleName
            {
                public const string Summary = "Specifies the name of the style to apply. If not specified, displays the name of the currently applied style.";
                public const string Summary_ko_KR = "적용할 스타일의 이름을 지정합니다. 지정되지 않으면 현재 적용된 스타일의 이름을 표시합니다.";
            }
        }

        public static class SceneCommand
        {
            public const string Summary = "Change scene.";
            public const string Summary_ko_KR = "장면을 변경합니다.";

            public static class IsList
            {
                public const string Summary = "Displays a list of scene.";
                public const string Summary_ko_KR = "장면의 목록을 표시합니다.";
            }

            public static class SceneName
            {
                public const string Summary = "Specifies the name or index of the scene to load. If not specified, displays the name of the active scene.";
                public const string Summary_ko_KR = "로드할 장면의 이름 또는 번호를 지정합니다. 지정되지 않으면 현재 로드된 장면의 이름을 표시합니다.";
            }
        }

        public static class VerboseCommand
        {
            public const string Summary = "Displays or sets the details for the message.";
            public const string Summary_ko_KR = "메시지에 대한 세부 정보를 표시하거나 설정합니다.";

            public static class Value
            {
                public const string Summary = "Specifies whether to display details. If not specified, displays the current value.";
                public const string Summary_ko_KR = "세부 정보 표시의 여부를 지정합니다. 지정되지 않으면 현재 값을 표시합니다.";
            }
        }

        public static class VersionCommand
        {
            public const string Summary = "Displays version.";
            public const string Summary_ko_KR = "버전을 표시합니다.";
        }

        public static class InternetProtocolCommand
        {
            public const string Summary = "Displays ip address.";
            public const string Summary_ko_KR = "ip 주소를 표시합니다.";
        }

        public static class ConfigCommand
        {
            public const string Summary = "Get and set properties.";
            public const string Summary_ko_KR = "속성을 가져오거나 설정합니다.";

            public static class ConfigName
            {
                public const string Summary = "Specifies the name of the property.";
                public const string Summary_ko_KR = "속성의 이름을 지정합니다.";
            }

            public static class Value
            {
                public const string Summary = "Specify a value. If no value is specified, displays the value of the property.";
                public const string Summary_ko_KR = "값을 지정합니다. 값이 지정되지 않으면 해당 속성의 값을 표시합니다.";
            }

            public static class ListSwitch
            {
                public const string Summary = "Displays the names and values of all properties.";
                public const string Summary_ko_KR = "모든 속성의 이름과 값을 표시합니다.";
            }

            public static class DetailSwitch
            {
                public const string Summary = "Displays details.";
                public const string Summary_ko_KR = "자세히 표시합니다.";
            }

            public static class ResetSwitch
            {
                public const string Summary = "Returns the value of the property to its default value.";
                public const string Summary_ko_KR = "속성의 값을 기본값으로 되돌립니다.";
            }
        }

        public static class DateCommand
        {
            public const string Summary = "Displays the date and time.";
            public const string Summary_ko_KR = "날짜와 시간을 표시합니다.";

            public static class Format
            {
                public const string Summary = "Specifies the format to display the date and time.";
                public const string Summary_ko_KR = "날짜와 시간을 표시할 서식을 지정합니다.";
            }

            public static class Locale
            {
                public const string Summary = "Specifies the culture to display the date and time.";
                public const string Summary_ko_KR = "날짜와 시간을 표시할 문화권을 지정합니다.";
            }

            public static class UTC
            {
                public const string Summary = "Displays the date and time as UTC.";
                public const string Summary_ko_KR = "날짜와 시간을 UTC로 표시합니다.";
            }
        }

        public static class ComponentCommand
        {
            public const string Summary = "Provides commands for Component.";
            public const string Summary_ko_KR = "Component 관련 명령어를 제공합니다.";

            public static class Add
            {
                public const string Summary = "Add components to GameObject.";
                public const string Summary_ko_KR = "GameObject 에 컴포넌트를 추가합니다.";
            }

            public static class Remove
            {
                public const string Summary = "Remove the component from the GameObject.";
                public const string Summary_ko_KR = "GameObject 에서 컴포넌트를 제거합니다.";
            }

            public static class Activate
            {
                public const string Summary = "Activates the component.";
                public const string Summary_ko_KR = "컴포넌트를 활성화합니다.";
            }

            public static class Deactivate
            {
                public const string Summary = "Deactivates the component.";
                public const string Summary_ko_KR = "컴포넌트를 비활성화합니다.";
            }

            public static class ShowList
            {
                public const string Summary = "Displays a list of components.";
                public const string Summary_ko_KR = "컴포넌트 목록을 표시합니다.";
            }

            public static class Path
            {
                public const string Summary = "Specifies the path of the object used for GameObject.Find().";
                public const string Summary_ko_KR = "GameObject.Find() 에 사용되는 객체의 경로를 지정합니다.";
            }

            public static class Index
            {
                public const string Summary = "Specifies the index of the component.";
                public const string Summary_ko_KR = "컴포넌트의 색인을 지정합니다.";
            }
        }

        public static class GameObjectCommand
        {
            public const string Summary = "Provides commands for GameObject.";
            public const string Summary_ko_KR = "GameObject 관련 명령어를 제공합니다.";

            public static class Create
            {
                public const string Summary = "Create GameObject.";
                public const string Summary_ko_KR = "GameObject를 생성합니다.";
            }

            public static class Rename
            {
                public const string Summary = "Rename GameObject.";
                public const string Summary_ko_KR = "GameObject의 이름을 변경합니다.";
            }

            public static class Move
            {
                public const string Summary = "Move the GameObject to the target object.";
                public const string Summary_ko_KR = "GameObject를 대상 객체로 이동합니다.";
            }

            public static class Delete
            {
                public const string Summary = "Delete GameObject.";
                public const string Summary_ko_KR = "GameObject를 삭제합니다.";
            }

            public static class Activate
            {
                public const string Summary = "Activates GameObject.";
                public const string Summary_ko_KR = "GameObject를 활성화합니다.";
            }

            public static class Deactivate
            {
                public const string Summary = "Deactivates GameObject.";
                public const string Summary_ko_KR = "GameObject를 비활성화 합니다.";
            }

            public static class ShowList
            {
                public const string Summary = "Add components to GameObject.";
                public const string Summary_ko_KR = "GameObject의 목록을 표시합니다.";
            }

            public static class Path
            {
                public const string Summary = "Specifies the path of the object used for GameObject.Find().";
                public const string Summary_ko_KR = "GameObject.Find() 에 사용되는 객체의 경로를 지정합니다.";
            }

            public static class Name
            {
                public const string Summary = "Specifies the name of the GameObject to create.";
                public const string Summary_ko_KR = "생성할 GameObject 이름을 지정합니다.";
            }

            public static class NewName
            {
                public const string Summary = "Specifies the new name of the GameObject to change.";
                public const string Summary_ko_KR = "변경할 GameObject 새로운 이름을 지정합니다.";
            }

            public static class ParentPath
            {
                public const string Summary = "Specifies the path of the GameObject to be moved.";
                public const string Summary_ko_KR = "이동할 대상이 되는 GameObject 의 경로를 지정합니다.";
            }

            public static class IsRecursive
            {
                public const string Summary = "Recursively displays a list of child objects.";
                public const string Summary_ko_KR = "자식 객체의 목록까지 재귀적으로 표시합니다.";
            }
        }
    }
}