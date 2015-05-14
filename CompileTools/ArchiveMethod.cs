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
        public static List<ArchiveMethod> methods = new List<ArchiveMethod>();

        public static void Load()
        {
            methods.Add(new IT3());
            //TODO: Add packing support for these
            //methods.Add(new FLDF0200());
            //methods.Add(new MLK());
        }
        public static ArchiveMethod FindArchiver(string method)
        {
            foreach (ArchiveMethod cm in methods)
            {
                if (cm.Outputs.Contains(method) || cm.Inputs.Contains(method))
                    return cm;
            }
            return null;
        }

        public abstract void Pack(FileReference[] input, Stream output);
        public abstract FileReference[] Unpack(FileReference input, bool recur, bool decomp);
    }
}
