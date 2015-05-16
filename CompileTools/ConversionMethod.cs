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
        public static List<ConversionMethod> methods = new List<ConversionMethod>();

        public static void Load()
        {
            methods.Add(new GMP200());
            methods.Add(new ITV());
        }
        public static ConversionMethod FindConvertor(string method)
        {
            foreach (ConversionMethod cm in methods)
            {
                if (cm.Outputs.Contains(method) || cm.Inputs.Contains(method))
                    return cm;
            }
            return null;
        }

        public abstract void ConvertTo(Stream input, Stream output);
        public abstract void ConvertFrom(Stream input, Stream output);
    }
}
