using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileTools.CLI.Commands
{
    public class ConvertCommand : Command
    {
        public ConvertCommand() : base("convert", "<file> <using>") { }

        public override void Execute(string[] args)
        {
            string file = QuotationRemover(args[0]);
            string ext = Path.GetExtension(file).ToLower();

            string method = args.Length > 1 ? "." + args[1] : ext;
            ConversionMethod converter = ConversionMethod.FindConvertor(method);
            string outputFile = Path.GetFileNameWithoutExtension(file) + method;

            if (converter == null)
                Console.WriteLine("These are not the formats we are looking for...");
            else if(converter.Inputs.Contains(ext))
                Convert(file, outputFile, converter, true);
            else if (converter.Outputs.Contains(ext))
                Convert(file, outputFile, converter, false);
            else
                Console.WriteLine("These are not the formats we are looking for...");
        }

        private static void Convert(string file, string outputFile, ConversionMethod converter, bool to)
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
                if (to)
                    converter.ConvertTo(input, output);
                else
                    converter.ConvertFrom(input, output);
            }
            catch (Exception ex)
            {
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
