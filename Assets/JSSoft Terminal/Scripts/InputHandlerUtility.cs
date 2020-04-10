// MIT License
// 
// Copyright (c) 2019 Jeesu Choi
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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JSSoft.UI
{
    public static class InputHandlerUtility
    {
        public static IInputHandler GetDefaultHandler()
        {
            if (TerminalEnvironment.IsMac == true)
                return MacOSInputHandler;
            else if (TerminalEnvironment.IsWindows == true)
                return WindowsInputHandler;
            else if (TerminalEnvironment.IsIPhone == true)
                return IOSInputHandler;
            throw new NotImplementedException();
        }

        public static IInputHandler MacOSInputHandler { get; } = new InputHandlers.MacOSInputHandler();

        public static IInputHandler WindowsInputHandler { get; } = new InputHandlers.WindowsInputHandler();

        public static IInputHandler IOSInputHandler { get; } = new InputHandlers.WindowsInputHandler();
    }
}
