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
        public static MLK mlk = new MLK();
        public static FLDF0200 fld = new FLDF0200();
        public static IT3 it3 = new IT3();
        public static CompressionMethod compressor = new LZ77CNX();
        public static ConversionMethod converter = new GMP200();

        public static void Main(string[] args)
        {
            CompressionMethod.Load();
            ConversionMethod.Load();
            ArchiveMethod.Load();

            Console.ForegroundColor = ConsoleColor.White;

            Command root = new CommandRouter("", new Command[] {
                new CompressCommand(),
                new ConvertCommand(),
                new PackCommand()
            });

            if (args.Length > 0)
            {
                root.Execute(args);
                return;
            }

            while (true)
            {
                Console.Write("> ");
                string junk = Console.ReadLine();

                Stopwatch watch = new Stopwatch();

                watch.Start();
                try
                {
                    root.Execute(junk.Split(' '));
                }
                catch (CommandParseException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }

                watch.Stop();

                Console.WriteLine();
                Console.WriteLine("Command took " + watch.ElapsedMilliseconds + "ms to execute.");
            }
        }  
    }
}
