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
        public abstract void Pack(ArchiveFile[] input, Stream output);
        public abstract ArchiveFile[] Unpack(Stream input, bool recur);
    }
}
