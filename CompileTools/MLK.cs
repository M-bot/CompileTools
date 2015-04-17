using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace CompileTools
{
    public class MLK : ArchiveMethod
    {
        public override string Name
        {
            get
            {
                return "MLK";
            }
        }

        public override bool Verify(Stream input)
        {
            return true; //Unverifiable 
        }

        public override void Pack(ReferenceFile[] input, Stream output)
        {
            /*
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
            }*/
        }

        public override ReferenceFile[] Unpack(Stream input, bool recur, bool decomp)
        {
            int indexNumber = ReadInt16(input);
            CompressionMethod decompressor = new LZ77CNX(); //Should be changed to go through a global list of compression methods
            
            List<FileIndex> indices = new List<FileIndex>();
            for (int x = 0; x < indexNumber; x++)
            {
                indices.Add(new FileIndex("track" + (x+1) + ".cnx", ReadInt32(input), ReadInt32(input)));
                if(x != indexNumber-1) input.ReadByte(); //Stupid spacer
            }

            List<ReferenceFile> output = new List<ReferenceFile>();
            for (int x = 0; x < indices.Count; x++) 
            {
                FileIndex index = indices[x];
                MemoryStream current = new MemoryStream();
                for(int y = 0; y < index.FileSize; y++)
                {
                    current.WriteByte((byte)input.ReadByte());
                }
                ReferenceFile outputFile = new ReferenceFile(current, index.FileName.Trim(), "");
                if (decomp)
                    output.Add(decompressor.Decompress(outputFile));
                else
                    output.Add(outputFile);
                
            }

            return output.ToArray<ReferenceFile>();
        }
    }
}
