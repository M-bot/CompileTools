using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CompileTools
{
    public class ITV : ConversionMethod
    {
        public override string Name
        {
            get { return "ITV"; }
        }
        public override string[] Outputs
        {
            get { return new string[] { ".itv" }; }
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
            throw new NotImplementedException();
        }
        public override void ConvertFrom(Stream input, Stream output)
        {
            String identity = ReadString(input, 4);
            ReadString(input, 4);
            int numOfFrames = ReadInt16(input);
            ReadString(input, 8);
            int numOfColors = ReadInt16(input) - 1;
            ReadInt16(input);
            int width = ReadInt16(input);
            int height = ReadInt16(input);
            ReadInt16(input);
            int importantColors = ReadInt16(input) - 1;
            ReadString(input, 4);

            Console.WriteLine("Identity: " + identity);
            Console.WriteLine("Frames: " + numOfFrames);
            Console.WriteLine("Colors: " + numOfColors + " " + importantColors);
            Console.WriteLine("Dimensions: " + height + "px by " + width + "px");

            MemoryStream palette = new MemoryStream();
            for (int i = 0; i < numOfColors; i++ )
            {
                byte[] color = new byte[4];
                color[2] = (byte)input.ReadByte();
                color[1] = (byte)input.ReadByte();
                color[0] = (byte)input.ReadByte();
                color[3] = (byte)input.ReadByte();
                for (int j = 0; j < 4; j++)
                    palette.WriteByte(color[j]);
            }
            Console.WriteLine();

            input.Seek(0x45D, SeekOrigin.Begin);
            for (int i = 0; i < 10; i++)
            {
                byte[,] image = new byte[height, width];
                int x = 0, y = 0, dx = width / 2, dy = height / 2;
                for (int j = 0; j < 4; j++ )
                {
                    if (nextInt16(input) == -256)
                    {
                        Console.WriteLine("Skip Frame: " + i + " Block: " + j);
                        ReadInt16(input);
                    }
                    else
                    {
                        byte[,] copy = readBlock(input, height / 2, width / 2);
                        for (int k = 0; k < height / 2; k++)
                        {
                            for (int l = 0; l < width / 2; l++)
                            {
                                image[k + y, l + x] = copy[k, l];
                            }
                        }
                        Console.WriteLine("Read Frame: " + i + " Block: " + j);
                    }

                    if (x + dx >= width)
                    {
                        y += dy;
                        x = 0;
                    }
                    else
                    {
                        x += dx;
                    }
                }
                 
                // BMP header
                WriteString(output, "BM");                  // ID field
                WriteInt32(output, (int)input.Length);      // BMP size
                WriteInt32(output, 0);                      // Unused
                WriteInt32(output, numOfColors * 4 + 54);              // Offset where the pixel array can be found

                // DIB header
                WriteInt32(output, 0x28);                   // Number of bytes int the DIB header from this point
                WriteInt32(output, width);                // Number of horizontal pixels
                WriteInt32(output, height);                // Number of vertical pixels
                WriteInt16(output, 1);                      // Number of color planes to be used
                WriteInt16(output, 8);        // Number of bits per pixel
                WriteInt32(output, 0);                      // No pixel array compression used
                WriteInt32(output, height * width);             // Size of raw bitmap data including padding
                WriteInt32(output, 0);                      // Print resolution of the iamge
                WriteInt32(output, 0);                      // So useless in our case
                WriteInt32(output, numOfColors);              // Number of colors in palette
                WriteInt32(output, 0);                      // Number of important colors. 0 means all.

                // Start of palette data
                palette.Seek(0, SeekOrigin.Begin);
                CopyBytes(palette, output);

                // Start of bitmap data
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        output.WriteByte(image[j, k]);
                    }
                }
                output.Close();
                output = new FileStream("out" + i + ".bmp", FileMode.Create);
            }
        }

        public static int nextInt16(Stream input)
        {
            int x = ReadInt16(input);
            input.Seek(-2, SeekOrigin.Current);
            return x;
        }

        public static byte[,] readBlock(Stream input, int height, int width)
        {
            input.ReadByte();
            int numOfBlocks = input.ReadByte();
            Console.WriteLine(numOfBlocks);
            byte[, ,] blocks = new byte[numOfBlocks, 2, 2];
            for (int i = 0; i < numOfBlocks; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        blocks[i, j, k] = (byte)input.ReadByte();
                    }
                }
            }

            byte[,] image = new byte[height, width];
            int y = 0, x = 0;
            for (int c = 0; c < height * width / 4; )
            {
                int flag = input.ReadByte();
                int move = 0;
                if (flag == 255)
                {
                    int amount = input.ReadByte();
                    move = amount;
                    c += amount;
                }
                else if (flag == 254)
                {
                    int amount = input.ReadByte() + 1;
                    move = amount;
                    c += amount;
                }
                else
                {
                    move = 1;
                    for (int dy = 0; dy < 2; dy++)
                    {
                        for (int dx = 0; dx < 2; dx++)
                        {
                            image[dy + y, dx + x] = blocks[flag % 0x3F, dy, dx];
                        }
                    }
                    c++;
                }

                for (int j = 0; j < move; j++)
                {
                    if (x + 2 >= width)
                    {
                        y += 2;
                        x = 0;
                    }
                    else
                    {
                        x += 2;
                    }
                }
            }
            return image;
        }
    }
}
