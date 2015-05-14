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

        public DelegateCommand(string cmd, string usage, CommandExecutor commandExec)
            : base(cmd, usage)
        {
            this.commandExec = commandExec;
        }

        public override void Execute(string[] args)
        {
            string dummyCommand = string.Join(" ", args);
            commandExec(dummyCommand);
        }
    }
}
