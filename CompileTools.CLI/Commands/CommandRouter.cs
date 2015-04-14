using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI
{
    public class CommandRouter : Command
    {
        private Command defaultCommand;
        private Dictionary<string, Command> routes;

        public Command DefaultCommand
        {
            get { return defaultCommand; }
        }

        public CommandRouter(string command, Command defaultCommand)
            : this(command, defaultCommand, new Command[0])
        {
        }

        public CommandRouter(string command, Command defaultCommand, Command[] routes)
            : base(command)
        {
            this.defaultCommand = defaultCommand;
            this.routes = new Dictionary<string, Command>();

            foreach (Command cmd in routes)
                this.routes.Add(cmd.Name, cmd);
        }

        public override void Execute(string remaining)
        {
            string trimmed = remaining.Trim();

            // Execute the default command if no further commands are provided.
            if (trimmed.Length == 0)
                defaultCommand.Execute("");

            // Determine our next command, removing any excess commands if there are any.
            string actualCommand = trimmed, actualRemaining = "";
            if (actualCommand.Contains(" "))
            {
                actualCommand = actualCommand.Substring(0, actualCommand.IndexOf(" "));
                actualRemaining = trimmed.Substring(trimmed.IndexOf(" ") + 1);
            }

            if (!routes.ContainsKey(actualCommand))
                throw new CommandParseException("Command/Subcommand '" + actualCommand + "' does not exist or is currently hiding in a place where we can't find it.");

            routes[actualCommand].Execute(actualRemaining);
        }
    }
}
