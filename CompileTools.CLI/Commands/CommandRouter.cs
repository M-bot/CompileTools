using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI
{
    public class CommandRouter : Command
    {
        private Dictionary<string, Command> routes;

        public CommandRouter(string command)
            : this(command, new Command[0])
        {
        }

        public CommandRouter(string command, Command[] routes)
            : base(command, "")
        {
            this.routes = new Dictionary<string, Command>();

            foreach (Command cmd in routes)
                this.routes.Add(cmd.Name, cmd);
        }

        public override void Execute(string[] args)
        {
            if(args[0].Length == 0 || args[0] == "help")
            {
                foreach (Command c in routes.Values)
                {
                    Console.WriteLine(c.Usage);
                }
                return;
            }
            string[] newArgs = new string[args.Length - 1];
            System.Array.Copy(args, 1, newArgs, 0, newArgs.Length);
            if (newArgs.Length == 0)
            {
                Console.WriteLine(routes[args[0]].Usage);
                return;
            }
            routes[args[0]].Execute(newArgs);
        }
    }
}
