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

using System;

namespace JSSoft.Unity.Terminal
{
    public static class InputHandlerInstances
    {
        public static IInputHandler DefaultHandler
        {
            get
            {
                if (TerminalEnvironment.IsStandalone == true)
                {
                    if (TerminalEnvironment.IsWindows == true)
                        return PowershellInputHandler;
                    return TerminalInputHandler;
                }
                else if (TerminalEnvironment.IsMobile == true)
                {
                    return MobileInputHandler;
                }
                throw new NotImplementedException();
            }
        }

        public static IInputHandler TerminalInputHandler { get; } = new InputHandlers.TerminalInputHandler();

        public static IInputHandler PowershellInputHandler { get; } = new InputHandlers.PowershellInputHandler();

        public static IInputHandler MobileInputHandler { get; } = new InputHandlers.MobileInputHandler();
    }
}
