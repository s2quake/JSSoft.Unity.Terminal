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
                { $"Terminal.outputText", "" },
                { $"Terminal.prompt", "" },
                { $"Terminal.command", "" },
                { $"Terminal.isReadOnly", "" },
                { $"Terminal.isVerbose", "" },
                { $"Terminal.dispatcher", "" },
            });

            stringsByLocale.Add("ko-KR", new Dictionary<string, string>()
            {
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