using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CompileTools
{
    public class FileReference
    {
        public Stream Stream;
        public string FileName;
        public string FileDirectory;

        public FileReference(Stream stream, string fileName, string fileDirectory)
        {
            Stream = stream;
            FileName = fileName;
            FileDirectory = fileDirectory;
        }
    }
}
