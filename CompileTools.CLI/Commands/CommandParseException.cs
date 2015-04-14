using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI
{
    public class CommandParseException : Exception
    {
        public CommandParseException(string reason)
            : base(reason)
        {

        }
    }
}
