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
        public static List<CompressionMethod> methods = new List<CompressionMethod>();

        public static void Load()
        {
            methods.Add(new LZ77CNX());
        }

        public static CompressionMethod FindCompressor(string method)
        {
            foreach(CompressionMethod cm in methods)
            {
                if (cm.Outputs.Contains(method) || cm.Inputs.Contains(method))
                    return cm;
            }
            return null;
        }

        public abstract void Compress(Stream input, Stream output);
        public abstract FileReference Decompress(FileReference input);
    }
}
