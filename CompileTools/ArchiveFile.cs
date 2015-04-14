using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CompileTools
{
    public class ArchiveFile
    {
        public Stream File;
        public string FileName;
        public string FileDirectory;

        public ArchiveFile(Stream file, string fileName, string fileDirectory)
        {
            File = file;
            FileName = fileName;
            FileDirectory = fileDirectory;
        }
    }
}
