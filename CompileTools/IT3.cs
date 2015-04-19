using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CompileTools
{
    public class IT3 : ArchiveMethod
    {
        public override string Name
        {
            get { return "IT3"; }
        }

        public override bool Verify(Stream input)
        {
            throw new NotImplementedException();
        }

        public override void Pack(FileReference[] input, Stream output)
        {
            FileReference index = input[0];
            for (int pointer = 0; pointer < index.Stream.Length;)
            {
                string filename = ReadString(index.Stream, ReadInt32(index.Stream));
                FileReference found = FindFile(input, filename);
                if(Path.GetExtension(filename) == ".itp")
                {
                    WriteString(output,"TEXI");
                    WriteInt32(output, (int)found.Stream.Length + 36);
                    WriteString(output, Path.GetFileNameWithoutExtension(filename), 36);
                }
                CopyBytes(found.Stream, output);
                pointer += 4 + filename.Length;
            }
            
        }

        public override FileReference[] Unpack(FileReference input, bool recur, bool decomp)
        {
            //There's no tag that defines the size of the file so we have do this until we reach the end of the stream
            //Also I only know how to interpet one type of chunk so this will consoldiate anything it doesn't convert into part files for putting back together

            
            List<FileReference> output = new List<FileReference>(); 
            int count = 0;
            FileReference index = new FileReference(new MemoryStream(), input.FileName + ".index", "");
            for(int pointer = 0; pointer < input.Stream.Length; pointer++)
            {
                string fourcc = ReadString(input.Stream, 4);
                int size = ReadInt32(input.Stream);
                MemoryStream current = new MemoryStream();
                string filename = "";
                if(fourcc != "TEXI") // not the only thing we know how to deal with
                {
                    WriteString(current, fourcc);
                    WriteInt32(current, size);
                    filename = "part" + count++ + "." + fourcc;
                }
                else
                {
                    filename = ReadString(input.Stream, 36) + ".itp";
                    size -= 36;
                }
                pointer += 8 + CopyBytes(input.Stream, current, size);
                output.Add(new FileReference(current, filename, input.FileName + "/"));
                WriteInt32(index.Stream, filename.Length);
                WriteString(index.Stream, filename);
            }
            output.Add(index);
            return output.ToArray<FileReference>();
        }
    }
}
