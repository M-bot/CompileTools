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

        public override string[] Outputs
        {
            get { return new string[] { ".fld" }; }
        }

        public override string[] Inputs
        {
            get { return new string[] { "*" }; }
        }

        public override bool Verify(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            string head = ReadString(input, 8);
            input.Seek(0, SeekOrigin.Begin);
            return head.Equals("FLDF0200");
        }

        public override void Pack(FileReference[] input, Stream output)
        {
            int dirLength = (int)(Math.Ceiling(input[0].FileDirectory.Length/4.0) * 4);
            WriteString(output, "FLDF0200");
            WriteInt32(output, 20 + dirLength);
            WriteInt32(output, input.Length);
            WriteInt32(output, 0);
            WriteString(output, input[0].FileDirectory, dirLength);

            int filePointer = 20 + dirLength + 20 * input.Length;

            foreach (FileReference file in input)
            {
                WriteString(output, file.FileName, 12);
                WriteInt32(output, filePointer);
                WriteInt32(output, (int)file.Stream.Length);
                filePointer += (int)file.Stream.Length;
            }

            foreach (FileReference file in input)
            {
                for (int y = 0; y < file.Stream.Length; y++)
                {
                    output.WriteByte((byte)file.Stream.ReadByte());
                }
            }
        }

        public override FileReference[] Unpack(FileReference input, bool recur, bool decomp)
        {
            string head = ReadString(input.Stream, 8);
            int indexPointer = ReadInt32(input.Stream);
            int indexNumber = ReadInt32(input.Stream);
            int useless = ReadInt32(input.Stream);
            string dir = ReadString(input.Stream, indexPointer - 0x14);
            CompressionMethod decompressor = new LZ77CNX(); //Should be changed to go through a global list of compression methods

            List<FileIndex> indices = new List<FileIndex>();
            for (int x = 0; x < indexNumber; x++)
            {
                indices.Add(new FileIndex(ReadString(input.Stream, 12), ReadInt32(input.Stream), ReadInt32(input.Stream)));
            }

            List<FileReference> output = new List<FileReference>();
            for (int x = 0; x < indices.Count; x++) 
            {
                FileIndex index = indices[x];
                MemoryStream current = new MemoryStream();
                for(int y = 0; y < index.FileSize; y++)
                {
                    current.WriteByte((byte)input.Stream.ReadByte());
                }
                FileReference outputFile = new FileReference(current, index.FileName.Trim(), dir);
                if (recur && Verify(current))
                    output.AddRange(Unpack(outputFile, recur, decomp));
                else if(decomp)
                    output.Add(decompressor.Decompress(outputFile));
                else
                    output.Add(outputFile);
                
            }

            return output.ToArray<FileReference>();
        }
    }
}
