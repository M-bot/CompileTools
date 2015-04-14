using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using CompileTools;

using CompileTools.CLI.Commands;

namespace CompileTools.CLI
{
    class Program
    {
        public static ArchiveMethod archiver = new FLDF0200();
        public static CompressionMethod compressor = new LZ77CNX();
        public static ConversionMethod converter = new GMP200();

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Command root = new CommandRouter("", new DelegateCommand("help", HelpCommand), new Command[] {
                new DelegateCommand("help", HelpCommand),
                new DelegateCommand("compress", CompressCommand),
                new DelegateCommand("decompress", DecompressCommand),
                new DelegateCommand("pack", PackCommand),
                new DelegateCommand("unpack", UnpackCommand),
                new DelegateCommand("convert", ConvertCommand)
            });

            while (true)
            {
                Console.Write("> ");
                string junk = Console.ReadLine();

                Stopwatch watch = new Stopwatch();

                watch.Start();
                try
                {
                    root.Execute(junk);
                }
                catch (CommandParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An unspecified error occured while running this command.");
                    Console.WriteLine(ex.Message);
                }

                watch.Stop();

                Console.WriteLine();
                Console.WriteLine("Command took " + watch.ElapsedMilliseconds + "ms to execute.");
            }
        }

        public static void HelpCommand(string inputs)
        {
            Console.WriteLine("compress <file> - Compress file to CNX format.");
            Console.WriteLine("decompress <file> - Decompress file from CNX format.");
            Console.WriteLine("pack - Currently unimplemented.");
            Console.WriteLine("unpack <file> - Unpack file archive to a new folder called Unpacked.");
            Console.WriteLine("convert <from> <to> <file> - Convert a file from a given format to another format.");
        }


        public static void CompressCommand(string inputs)
        {
            if (inputs.Trim().Length == 0)
            {
                Console.WriteLine("Usage: compress <file>");
                return;
            }

            // Get rid of any excess quotations
            string file = inputs;
            if (file.StartsWith("\"") && file.EndsWith("\""))
                file = file.Substring(1, file.Length - 1);

            string outputFile = Path.GetFileNameWithoutExtension(file) + ".cnx";

            FileStream input, output;
            try
            {
                input = new FileStream(file, FileMode.Open);
                output = new FileStream(outputFile, FileMode.Create);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            try
            {
                compressor.Compress(input, output);

                Console.WriteLine("Compressing " + input.Length + " bytes, result is " + output.Length + " bytes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Compression failed for the following reason: ");
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                input.Close();
                output.Close();
            }
        }

        public static void DecompressCommand(string inputs)
        {
            if (inputs.Trim().Length == 0)
            {
                Console.WriteLine("Usage: decompress <file>");
                return;
            }

            // Get rid of any excess quotations
            string file = inputs;
            if (file.StartsWith("\"") && file.EndsWith("\""))
                file = file.Substring(1, file.Length - 1);

            string outputFile = Path.GetFileNameWithoutExtension(file) + ".gmp";

            FileStream input, output;

            try
            {
                input = new FileStream(file, FileMode.Open);
                output = new FileStream(outputFile, FileMode.Create);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            try
            {
                compressor.Decompress(input, output);

                Console.WriteLine("Decompression of " + input.Length + " bytes complete, expanded file is " + output.Length + " bytes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Decompression failed, likely due to file corruption. Error: ");
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                input.Close();
                output.Close();
            }

        }

        public static void PackCommand(string inputs)
        {
            if (inputs.Trim().Length == 0)
            {
                Console.WriteLine("Error: Pack command currently not implemented.");
                return;
            }

            Console.WriteLine("Error: Pack command currently not implemented. Use Flame's for now.");
        }

        public static void UnpackCommand(string inputs)
        {
            if (inputs.Trim().Length == 0)
            {
                Console.WriteLine("Usage: unpack <file>");
                return;
            }

            bool recur = false;

            if (inputs.StartsWith("-r"))
            {
                recur = true;
                inputs = inputs.Substring(2).Trim();
            }

            // Get rid of any excess quotations
            string file = inputs;
            if (file.StartsWith("\"") && file.EndsWith("\""))
                file = file.Substring(1, file.Length - 1);

            FileStream input;

            try
            {
                input = new FileStream(file, FileMode.Open);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            ArchiveFile[] files = archiver.Unpack(input, recur);
            foreach (ArchiveFile outputFile in files)
            {
                try
                {
                    string dir = Path.Combine("Unpacked", outputFile.FileDirectory);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    using (FileStream current = new FileStream(dir + outputFile.FileName, FileMode.Create))
                    {
                        outputFile.File.Seek(0, SeekOrigin.Begin);

                        for (int x = 0; x < outputFile.File.Length; x++)
                            current.WriteByte((byte)outputFile.File.ReadByte());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to extract file " + outputFile.FileName);
                    Console.WriteLine(ex.Message);
                }
            }

            input.Close();
        }

        public static void ConvertCommand(string inputs)
        {
            if (inputs.Trim().Length == 0)
            {
                Console.WriteLine("Usage: convert <from> <to> <file>");
                Console.WriteLine("Supported Formats: bmp, gmp");
                return;
            }

            string[] stuff = inputs.ToLower().Split(' ');
            if (stuff[0].Equals("gmp") && stuff[1].Equals("bmp"))
            {
                string file = inputs.Substring(8).Trim();
                if (file.StartsWith("\"") && file.EndsWith("\""))
                    file = file.Substring(1, file.Length - 1);

                FileStream input, output;

                try
                {
                    input = new FileStream(file, FileMode.Open);
                    output = new FileStream(Path.GetFileNameWithoutExtension(file) + ".bmp", FileMode.Create);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                try
                {
                    converter.ConvertFrom(input, output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to convert from " + stuff[0] + " to " + stuff[1]);
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    input.Close();
                    output.Close();
                }
            }

            if (stuff[0].Equals("bmp") && stuff[1].Equals("gmp"))
            {
                string file = inputs.Substring(8).Trim();
                if (file.StartsWith("\"") && file.EndsWith("\""))
                    file = file.Substring(1, file.Length - 1);

                FileStream input, output;

                try
                {
                    input = new FileStream(file, FileMode.Open);
                    output = new FileStream(Path.GetFileNameWithoutExtension(file) + ".gmp", FileMode.Create);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                try
                {
                    converter.ConvertTo(input, output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to convert from " + stuff[0] + " to " + stuff[1]);
                    Console.WriteLine(ex.Message);
                    return;
                }
                finally
                {
                    input.Close();
                    output.Close();
                }
            }
        }
    }
}
