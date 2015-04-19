using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompileTools
{
    public abstract class ArchiveMethod : Method
    {
        public abstract void Pack(FileReference[] input, Stream output);
        public abstract FileReference[] Unpack(FileReference input, bool recur, bool decomp);
    }
}
