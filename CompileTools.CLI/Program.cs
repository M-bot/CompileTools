using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using CompileTools;

namespace CompileTools.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            ArchiveMethod archiver = new FLDF0200();
            CompressionMethod compressor = new LZ77CNX();
            ConversionMethod converter = new GMP200();

            while (true)
            {
                Console.Write("> ");
                string junk = Console.ReadLine();

                if (junk.StartsWith("compress "))
                {
                    string file = junk.Substring("compress ".Length);

                    Stream input = new FileStream(file, FileMode.Open);

                    Stopwatch watch = new Stopwatch();

                    FileStream output = new FileStream(Path.GetFileNameWithoutExtension(file)+".cnx", FileMode.Create);

                    watch.Start();
                    compressor.Compress(input, output);
                    watch.Stop();

                    Console.WriteLine("Compressing " + input.Length + " bytes, result is " + output.Length + " bytes.");
                    Console.WriteLine("Operation took " + watch.ElapsedMilliseconds + " ms.");

                    output.Close();
                    input.Close();
                }
                else if (junk.StartsWith("decompress "))
                {
                    string file = junk.Substring("decompress ".Length);
                    Stream input = new FileStream(file, FileMode.Open);
                    Stream output = new FileStream(Path.GetFileNameWithoutExtension(file) + ".gmp", FileMode.Create);

                    Stopwatch watch = new Stopwatch();

                    watch.Start();
                    compressor.Decompress(input, output);
                    watch.Stop();

                    Console.WriteLine("Decompression of " + input.Length + " bytes complete, expanded file is " + output.Length + " bytes.");
                    Console.WriteLine("Took " + watch.ElapsedMilliseconds + " ms.");

                    input.Close();
                    output.Close();
                }
                else if (junk.StartsWith("pack "))
                {
                    Console.WriteLine("This packer (its bit perfect) works but is difficult (since no metadata for repacking) to use. Use Flame's for now.");
                }
                else if (junk.StartsWith("unpack "))
                {
                    string stuff = junk.Substring("unpack ".Length);
                    bool recur = false;
                    if(recur = stuff.StartsWith("-r "))
                    {
                        stuff = stuff.Substring("-r ".Length);
                    }
                    string filename = stuff;
                    Stream input = new FileStream(filename, FileMode.Open);

                    ArchiveMethod.ArchiveFile[] files = archiver.Unpack(input, recur);
                    foreach (ArchiveMethod.ArchiveFile file in files)
                    {
                        string dir = "Unpacked\\" + file.FileDirectory;
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        FileStream current = new FileStream(dir + file.FileName, FileMode.Create);
                        file.File.Seek(0, SeekOrigin.Begin);
                        for (int x = 0; x < file.File.Length; x++)
                        {
                            current.WriteByte((byte)file.File.ReadByte());
                        }
                        current.Flush();
                        current.Close();
                    }

                    input.Close();
                }
                else if (junk.StartsWith("convert "))
                {
                    string[] stuff = junk.Split(' ');
                    if(stuff[1].Equals("gmp") && stuff[2].Equals("bmp"))
                    {
                        string file = stuff[3];
                        Stream input = new FileStream(file, FileMode.Open);
                        Stream output = new FileStream(Path.GetFileNameWithoutExtension(file) + ".bmp", FileMode.Create);

                        converter.ConvertFrom(input, output);

                        input.Close();
                        output.Close();
                    }

                    if (stuff[1].Equals("bmp") && stuff[2].Equals("gmp"))
                    {
                        string file = stuff[3];
                        Stream input = new FileStream(file, FileMode.Open);
                        Stream output = new FileStream(Path.GetFileNameWithoutExtension(file) + ".gmp", FileMode.Create);

                        converter.ConvertTo(input, output);

                        input.Close();
                        output.Close();
                    }
                }
            }
        }
    }
}
