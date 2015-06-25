using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompileTools
{
    public class GMP200 : ConversionMethod
    {
        private bool forcedOpaqueness;
        private int transparencyColor = -1;

        public override string Name
        {
            get { return "GMP-200"; }
        }
        public override string[] Outputs
        {
            get { return new string[] { ".gmp" }; }
        }

        public override string[] Inputs
        {
            get { return new string[] { ".bmp" }; }
        }
        public override bool Verify(Stream input)
        {
            throw new NotImplementedException();
        }
        public override void ConvertTo(Stream input, Stream output)
        {
            Console.WriteLine("Force transparency (true|false): ");
            Boolean.TryParse(Console.ReadLine(), out forcedOpaqueness);
            Console.WriteLine("Transparency Color (0-255): ");
            Int32.TryParse(Console.ReadLine(), out transparencyColor);

            string id = ReadString(input, 2);
            int bmpSize = ReadInt32(input);
            WriteInt32(input, 0);
            int bitmapOff = ReadInt32(input);
            ReadInt32(input);
            int horiPix = ReadInt32(input);
            int vertPix = ReadInt32(input);
            ReadInt16(input);
            int bitDepth = ReadInt16(input);
            ReadInt32(input);
            int bitmapSize = ReadInt32(input);
            ReadInt32(input);
            ReadInt32(input);
            int usedColors = ReadInt32(input);
            ReadInt32(input);

            int palStart = 0x20;
            int dataStart = palStart + usedColors * 4;
            WriteString(output, "GMP-200", 8);          // ID field
            WriteInt32(output, vertPix);                // Number of vertical pixels
            WriteInt32(output, horiPix);                // Number of horizontal pixels
            WriteInt32(output, 0);                      // Unused
            WriteInt32(output, palStart);               // Palette starting position
            WriteInt32(output, dataStart);              // Data starting position
            WriteInt16(output, (Int16)usedColors);      // Number of colors in palette
            WriteInt16(output, (Int16)bitDepth);        // Number of bits per pixel

            // Start of palette data
            for (int x = 0; x < usedColors; x++)
            {
                byte[] color = new byte[4];
                for (int j = 0; j < 4; j++)
                    color[j] = (byte)input.ReadByte();
                if (forcedOpaqueness)
                {
                    if(transparencyColor != x)
                        color[0] = 0xFF;
                }
                for (int j = 0; j < 4; j++)
                    output.WriteByte(color[j]);
            }

            // Start of bitmap data
            for (int x = 0; x < bitmapSize; x++)
            {
                output.WriteByte((byte)input.ReadByte());
            }
        }
        public override void ConvertFrom(Stream input, Stream output)
        {
            string identity = ReadString(input, 8);
            int vertPix = ReadInt32(input);
            int horiPix = ReadInt32(input);
            ReadInt32(input);
            int palStart = ReadInt32(input);
            int dataStart = ReadInt32(input);
            int usedColors = ReadInt16(input);
            int bitDepth = ReadInt16(input);


            int maxColors = 1 << bitDepth;
            int bitmapOff = usedColors * 4 + 54;
            int bitmapSize = vertPix * horiPix;

            Console.WriteLine(identity + " " + vertPix + " " + horiPix + " " + palStart + " " + dataStart + " " + usedColors + " " + bitDepth);


            // BMP header
            WriteString(output, "BM");                  // ID field
            WriteInt32(output, (int)input.Length);      // BMP size
            WriteInt32(output, 0);                      // Unused
            WriteInt32(output, bitmapOff);              // Offset where the pixel array can be found

            // DIB header
            WriteInt32(output, 0x28);                   // Number of bytes int the DIB header from this point
            WriteInt32(output, horiPix);                // Number of horizontal pixels
            WriteInt32(output, vertPix);                // Number of vertical pixels
            WriteInt16(output, 1);                      // Number of color planes to be used
            WriteInt16(output, (Int16)bitDepth);        // Number of bits per pixel
            WriteInt32(output, 0);                      // No pixel array compression used
            WriteInt32(output, bitmapSize);             // Size of raw bitmap data including padding
            WriteInt32(output, 0);                      // Print resolution of the iamge
            WriteInt32(output, 0);                      // So useless in our case
            WriteInt32(output, usedColors);              // Number of colors in palette
            WriteInt32(output, 0);                      // Number of important colors. 0 means all.

            // Start of palette data
            for(int x = 0; x < usedColors; x++)
            {
                WriteInt32(output, ReadInt32(input));
            }

            // Start of bitmap data
            for (int x = 0; x < bitmapSize; x++)
            {
                output.WriteByte((byte)input.ReadByte());
            }


        }
    }
}
