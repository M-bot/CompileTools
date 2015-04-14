using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace CompileTools
{
    public class FLDF0200 : ArchiveMethod
    {
        public override string Name
        {
            get
            {
                return "FLDF0200";
            }
        }

        public override bool Verify(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            string head = ReadString(input, 8);
            input.Seek(0, SeekOrigin.Begin);
            return head.Equals("FLDF0200");
        }

        public override void Pack(ArchiveFile[] input, Stream output)
        {
            int dirLength = (int)(Math.Ceiling(input[0].FileDirectory.Length/4.0) * 4);
            WriteString(output, "FLDF0200");
            WriteInt32(output, 20 + dirLength);
            WriteInt32(output, input.Length);
            WriteInt32(output, 0);
            WriteString(output, input[0].FileDirectory, dirLength);

            int filePointer = 20 + dirLength + 20 * input.Length;

            foreach (ArchiveFile file in input)
            {
                WriteString(output, file.FileName, 12);
                WriteInt32(output, filePointer);
                WriteInt32(output, (int)file.File.Length);
                filePointer += (int)file.File.Length;
            }

            foreach (ArchiveFile file in input)
            {
                for (int y = 0; y < file.File.Length; y++)
                {
                    output.WriteByte((byte)file.File.ReadByte());
                }
            }
        }

        public override ArchiveFile[] Unpack(Stream input, bool recur)
        {
            string head = ReadString(input, 8);
            int indexPointer = ReadInt32(input);
            int indexNumber = ReadInt32(input);
            int useless = ReadInt32(input);
            string dir = ReadString(input, indexPointer - 0x14);

            List<FileIndex> indices = new List<FileIndex>();
            for (int x = 0; x < indexNumber; x++)
            {
                indices.Add(new FileIndex(ReadString(input, 12), ReadInt32(input), ReadInt32(input)));
            }

            List<ArchiveFile> output = new List<ArchiveFile>();
            for (int x = 0; x < indices.Count; x++) 
            {
                FileIndex index = indices[x];
                MemoryStream current = new MemoryStream();
                for(int y = 0; y < index.FileSize; y++)
                {
                    current.WriteByte((byte)input.ReadByte());
                }
                if (recur && Verify(current))
                    output.AddRange(Unpack(current, recur));
                else
                    output.Add(new ArchiveFile(current, index.FileName.Trim(), dir));
                
            }

            return output.ToArray<ArchiveFile>();
        }

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
}
