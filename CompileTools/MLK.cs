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

        public override string[] Outputs
        {
            get { return new string[] { ".mlk" }; }
        }

        public override string[] Inputs
        {
            get { return new string[] { ".cnx" }; }
        }

        public override bool Verify(Stream input)
        {
            return true; //Unverifiable 
        }

        public override void Pack(FileReference[] input, Stream output)
        {
            // TODO: Finish packing
        }

        public override FileReference[] Unpack(FileReference input, bool recur, bool decomp)
        {
            int indexNumber = ReadInt16(input.Stream);
            CompressionMethod decompressor = new LZ77CNX(); //Should be changed to go through a global list of compression methods
            
            List<FileIndex> indices = new List<FileIndex>();
            for (int x = 0; x < indexNumber; x++)
            {
                indices.Add(new FileIndex("track" + (x+1) + ".cnx", ReadInt32(input.Stream), ReadInt32(input.Stream)));
                if(x != indexNumber-1) input.Stream.ReadByte(); //Stupid spacer
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
                FileReference outputFile = new FileReference(current, index.FileName.Trim(), "");
                if (decomp)
                    output.Add(decompressor.Decompress(outputFile));
                else
                    output.Add(outputFile);
                
            }

            return output.ToArray<FileReference>();
        }
    }
}
