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

        public string Name
        {
            get { return prefix;  }
        }

        public Command(string prefix)
        {
            this.prefix = prefix;
        }

        public abstract void Execute(string remaining);
    }
}
