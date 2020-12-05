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
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [VersionCommand]
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class VersionCommand : CommandBase
    {
        protected override void OnExecute()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Identifier: {Application.identifier}");
            sb.AppendLine($"Application Version: {Application.version}");
            sb.AppendLine($"ProductName: {Application.productName}");
            sb.AppendLine($"Unity Version: {Application.unityVersion}");
            sb.AppendLine($"u-terminal(file): {GetTerminalFileVersion()}");
            sb.AppendLine($"u-terminal(info): {GetTerminalInfoVersion()}");
            this.Out.WriteLine(sb.ToString());
        }

        private static string GetTerminalFileVersion()
        {
            var attr = Attribute.GetCustomAttribute(typeof(Terminal).Assembly, typeof(AssemblyFileVersionAttribute));
            if (attr is AssemblyFileVersionAttribute versionAttribute)
                return versionAttribute.Version;
            return "none version";
        }

        private static string GetTerminalInfoVersion()
        {
            var attr = Attribute.GetCustomAttribute(typeof(Terminal).Assembly, typeof(AssemblyInformationalVersionAttribute));
            if (attr is AssemblyInformationalVersionAttribute versionAttribute)
                return versionAttribute.InformationalVersion;
            return "none version";
        }
    }
}
