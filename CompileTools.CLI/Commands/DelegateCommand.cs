using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI.Commands
{
    public class DelegateCommand : Command
    {
        public delegate void CommandExecutor(string remaining);

        private CommandExecutor commandExec;

        public CommandExecutor Delegate
        {
            get { return commandExec; }
        }

        public DelegateCommand(string cmd, CommandExecutor commandExec)
            : base(cmd)
        {
            this.commandExec = commandExec;
        }

        public override void Execute(string remaining)
        {
            commandExec(remaining);
        }
    }
}
