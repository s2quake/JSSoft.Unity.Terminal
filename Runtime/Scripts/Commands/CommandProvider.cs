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

using JSSoft.Library.Commands;
using System.Collections.Generic;

namespace JSSoft.Unity.Terminal.Commands
{
    public class CommandProvider : ICommandProvider
    {
        public virtual IEnumerable<ICommand> Provide(ITerminal terminal, ICommandConfigurationProvider configurationProvider)
        {
            yield return new VersionCommand();
            yield return new ResetCommand(terminal);
            yield return new InfoCommand(terminal);
            yield return new ExitCommand(terminal);
            yield return new VerboseCommand(terminal);
            yield return new ResolutionCommand(terminal);
            yield return new PingCommand(terminal);
            yield return new StyleCommand(terminal);
            yield return new InternetProtocolCommand(terminal);
            yield return new SceneCommand(terminal);
            yield return new DateCommand(terminal);
            yield return new GameObjectCommand(terminal);
            yield return new ComponentCommand(terminal);
            yield return new TerminalCommand(terminal);
            yield return new CultureCommand(terminal);

            yield return new ConfigCommand(terminal, configurationProvider);
        }
    }
}
