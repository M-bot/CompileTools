using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools
{
    public class FileIndex
    {
        public string FileName;
        public int FilePointer;
        public int FileSize;

        public FileIndex(string fileName, int filePointer, int fileSize)
        {
            this.FileName = fileName;
            this.FilePointer = filePointer;
            this.FileSize = fileSize;
        }
    }
}
