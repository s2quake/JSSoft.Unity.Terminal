using System.Collections.Generic;
using Ntreev.Library.Commands;

namespace JSSoft.Communication.Shells
{
    class CommandContext : CommandContextBase
    {
        public CommandContext(IEnumerable<ICommand> commands, IEnumerable<ICommandProvider> methods)
            : base(commands, methods)
        {

        }

        public new string[] GetCompletion(string[] items, string find)
        {
            return base.GetCompletion(items, find);
        }
    }
}