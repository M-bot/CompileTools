using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompileTools
{
    public abstract class CompressionMethod : Method
    {
        public abstract void Compress(Stream input, Stream output);
        public abstract ReferenceFile Decompress(ReferenceFile input);
    }
}
