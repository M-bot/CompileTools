using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI.Commands
{
    public class CompressCommand : Command
    {
        public CompressCommand() : base("compress", "<file> <using>") { }

        public override void Execute(string[] args)
        {
            string file = QuotationRemover(args[0]);
            string ext = Path.GetExtension(file).ToLower();

            string method = args.Length > 1 ? "." + args[1] : ".cnx";
            CompressionMethod compressor = CompressionMethod.FindCompressor(method);
            string outputFile = Path.GetFileNameWithoutExtension(file) + method;

            if (compressor == null)
                Console.WriteLine("These are not the formats we are looking for...");
            else if (compressor.Inputs.Contains(ext))
                Compress(file, outputFile, compressor);
            else if (compressor.Outputs.Contains(ext))
                Decompress(file, compressor);
            else
                Console.WriteLine("These are not the formats we are looking for...");
        }

        private static void Compress(string file, string outputFile, CompressionMethod compressor)
        {
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

        private static void Decompress(string file, CompressionMethod compressor)
        {
            FileReference input;
            Stream output = null;

            try
            {
                input = new FileReference(new FileStream(file, FileMode.Open), Path.GetFileName(file), Path.GetDirectoryName(file));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            try
            {
                FileReference outputFile = compressor.Decompress(input);
                output = new FileStream(outputFile.FileDirectory + "/" + outputFile.FileName, FileMode.Create);
                outputFile.Stream.Seek(0, SeekOrigin.Begin);
                for (int x = 0; x < outputFile.Stream.Length; x++)
                    output.WriteByte((byte)outputFile.Stream.ReadByte());

                Console.WriteLine("Decompression of " + input.Stream.Length + " bytes complete, expanded file is " + outputFile.Stream.Length + " bytes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Decompression failed, likely due to file corruption. Error: ");
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                input.Stream.Close();
                if (output != null) output.Close();
            }
        }

    }
}
