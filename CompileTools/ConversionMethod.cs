using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompileTools
{
    public abstract class ConversionMethod : Method
    {
        public abstract void ConvertTo(Stream input, Stream output);
        public abstract void ConvertFrom(Stream input, Stream output);
    }
}
