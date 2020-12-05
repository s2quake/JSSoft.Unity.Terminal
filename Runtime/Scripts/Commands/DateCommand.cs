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

namespace JSSoft.Unity.Terminal.Commands
{
    [UsageDescriptionProvider(typeof(CommandUsageDescriptionProvider))]
    public class DateCommand : TerminalCommandBase
    {
        public DateCommand(ITerminal terminal)
            : base(terminal)
        {
        }

        [CommandProperty]
        public string Format { get; set; }

        [CommandProperty]
        public string Locale { get; set; }

        [CommandPropertySwitch]
        public bool UTC { get; set; }

        protected override void OnExecute()
        {
            var dateTime = this.UTC == true ? DateTime.UtcNow : DateTime.Now;
            var cultureInfo = this.Locale != null ? CultureInfo.CreateSpecificCulture(this.Locale) : CultureInfo.CurrentCulture;
            var format = this.Format ?? "G";
            this.WriteLine(dateTime.ToString(format, cultureInfo));
        }
    }
}