using JSSoft.UI;
using UnityEngine;

namespace JSSoft.Communication.Shells
{
    public class TerminalHost : MonoBehaviour
    {
        private CommandContext commandContext;

        public void Awake()
        {

        }

        public void Start()
        {
            this.commandContext = Container.GetService<CommandContext>();
            if (this.terminal != null)
            {
                this.terminal.onCompletion = this.commandContext.GetCompletion;
            }
        }

        public TerminalPro terminal;
    }
}