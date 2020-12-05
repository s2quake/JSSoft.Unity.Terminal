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

using System;
using System.Globalization;
using JSSoft.Library.Commands;
using UnityEngine;

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class CultureCommand : TerminalCommandBase
    {
        private static readonly CultureInfo defaultCulture;
        private static readonly CultureInfo defaultUICulture;

        static CultureCommand()
        {
            defaultCulture = CultureInfo.DefaultThreadCurrentCulture;
            defaultUICulture = CultureInfo.DefaultThreadCurrentUICulture;
        }

        public CultureCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandPropertyRequired(DefaultValue = "")]
        public string Culture
        {
            get; set;
        }

        [CommandPropertySwitch("ui")]
        public bool IsUI
        {
            get; set;
        }

        [CommandPropertySwitch("reset")]
        [CommandPropertyTrigger(nameof(Culture), "")]
        public bool ResetSwitch
        {
            get; set;
        }

        protected override void OnExecute()
        {
            if (this.ResetSwitch == true)
            {
                if (this.IsUI == true)
                    CultureInfo.DefaultThreadCurrentUICulture = defaultUICulture;
                else
                    CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            }
            else if (this.Culture == string.Empty)
            {
                if (this.IsUI == true)
                    this.WriteLine($"{nameof(CultureInfo.CurrentUICulture)}: {CultureInfo.DefaultThreadCurrentUICulture}");
                else
                    this.WriteLine($"{nameof(CultureInfo.CurrentCulture)}: {CultureInfo.DefaultThreadCurrentCulture}");
            }
            else
            {
                var culture = CultureInfo.CreateSpecificCulture(this.Culture);
                if (this.IsUI == true)
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                else
                    CultureInfo.DefaultThreadCurrentCulture = culture;
            }
        }
    }
}