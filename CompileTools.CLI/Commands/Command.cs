using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI
{
    public abstract class Command
    {
        private string prefix;
        private string usage;

        public string Name
        {
            get { return prefix;  }
        }
        public string Usage
        {
            get { return "Usage: " + prefix + " " + usage; }
        }

        public Command(string prefix, string usage)
        {
            this.prefix = prefix;
            this.usage = usage;
        }

        public abstract void Execute(string[] args);

        public static string QuotationRemover(string quoted)
        {
            if (quoted.StartsWith("\"") && quoted.EndsWith("\""))
                return quoted.Substring(1, quoted.Length - 1);
            return quoted;
        }

        public static string[] ArgsRemover(string[] args, int index)
        {
            string[] newArgs = new string[args.Length - 1];
            System.Array.Copy(args, 0, newArgs, 0, index);
            System.Array.Copy(args, index + 1, newArgs, index, newArgs.Length - index);
            return newArgs;
        }

        public static bool ParseArgs(ref string[] args, string arg)
        {
            if(args.Contains(arg))
            {
                args = ArgsRemover(args, Array.IndexOf(args, arg));
                return true;
            }
            return false;
        }
    }
}
