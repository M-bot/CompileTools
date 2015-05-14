using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI.Commands
{
    class PackCommand : Command
    {
        public PackCommand() : base("pack", "[-d] [-r] <file> <using>") { }

        public override void Execute(string[] args)
        {
            bool decomp = ParseArgs(ref args, "-d");
            bool recur = ParseArgs(ref args, "-r");


            string file = QuotationRemover(args[0]);
            string ext = Path.GetExtension(file).ToLower();

            string method = args.Length > 1 ? "." + args[1] : ext;
            ArchiveMethod archiver = ArchiveMethod.FindArchiver(method);
            string outputFile = Path.GetFileNameWithoutExtension(file) + method;

            if (archiver == null)
                Console.WriteLine("These are not the formats we are looking for...");
            else if (archiver.Inputs.Contains(ext))
                Pack(file, outputFile, archiver);
            else if (archiver.Outputs.Contains(ext))
                Unpack(file, recur, decomp, archiver);
            else
                Console.WriteLine("These are not the formats we are looking for...");


        }

        private static void Pack(string file, string outputFile, ArchiveMethod archiver)
        {
            string dir = Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + "/";

            string[] filenames = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            FileReference[] files2 = new FileReference[filenames.Length + 1];

            files2[0] = new FileReference(new FileStream(file, FileMode.Open), file, "");
            for (int x = 0; x < files2.Length - 1; x++)
            {
                files2[x + 1] = new FileReference(new FileStream(filenames[x], FileMode.Open), Path.GetFileName(filenames[x]), "");
            }

            dir = "Packed/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileStream output = new FileStream(dir + outputFile, FileMode.Create);
            archiver.Pack(files2, output);

            foreach (FileReference f in files2)
            {
                f.Stream.Close();
            }
            output.Close();
        }

        private static void Unpack(string file, bool recur, bool decomp, ArchiveMethod archiver)
        {
            FileReference input;

            try
            {
                input = new FileReference(new FileStream(file, FileMode.Open), Path.GetFileName(file), Path.GetDirectoryName(file));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            FileReference[] files = archiver.Unpack(input, recur, decomp);

            foreach (FileReference outputFile in files)
            {
                try
                {
                    string dir = Path.Combine("Unpacked/", outputFile.FileDirectory);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    using (FileStream current = new FileStream(dir + outputFile.FileName, FileMode.Create))
                    {
                        outputFile.Stream.Seek(0, SeekOrigin.Begin);

                        for (int x = 0; x < outputFile.Stream.Length; x++)
                            current.WriteByte((byte)outputFile.Stream.ReadByte());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to extract file " + outputFile.FileName);
                    Console.WriteLine(ex.Message);
                }
            }

            input.Stream.Close();
        }

    }
}
