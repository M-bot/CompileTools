using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

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
            int numOfColors = ReadInt16(input);
            ReadInt16(input);
            int width = ReadInt16(input);
            int height = ReadInt16(input);
            ReadInt16(input);
            int importantColors = ReadInt16(input);
            //ReadString(input, 4);

            Console.WriteLine("Identity: " + identity);
            Console.WriteLine("Frames: " + numOfFrames);
            Console.WriteLine("Colors: " + numOfColors + " " + importantColors);
            Console.WriteLine("Dimensions: " + height + "px by " + width + "px");

            Color[] colors = new Color[numOfColors];
            for (int i = 0; i < numOfColors; i++ )
            {
                byte[] color = new byte[4];
                color[2] = (byte)input.ReadByte();
                color[1] = (byte)input.ReadByte();
                color[0] = (byte)input.ReadByte();
                color[3] = (byte)input.ReadByte();
                colors[i] = Color.FromArgb(color[3], color[2], color[1], color[0]);
            }
            Console.WriteLine();

            byte[,] image = new byte[height, width];
            input.Seek(0x45D, SeekOrigin.Begin);
            for (int i = 0; i < 10; i++)
            {
                int x = 0, y = 0, dx = width / 2, dy = height / 2;
                for (int j = 0; j < 4; j++ )
                {
                    if (nextInt16(input) == -256)
                    {
                        //Console.WriteLine("Skip Frame: " + i + " Block: " + j);
                        ReadInt16(input);
                    }
                    else
                    {
                        Console.WriteLine("Block ("+i+","+j+") starts at 0x{0:X}", input.Position);
                        readBlock(input, image, y, x, height / 2, width / 2);
                        //Console.WriteLine("Read Frame: " + i + " Block: " + j);
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

                Stream output2 = new FileStream("test/out" + i + ".png", FileMode.Create);
                Bitmap b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        b.SetPixel(k, j, colors[image[j, k]]);
                    }
                }
                b.Save(output2, ImageFormat.Png);
                output2.Close();
                first = false;
            }
        }

        public static int nextInt16(Stream input)
        {
            int x = ReadInt16(input);
            input.Seek(-2, SeekOrigin.Current);
            return x;
        }

        static byte[, ,] lblocks = new byte[10000, 2, 2];
        static bool first = true;
        public static void readBlock(Stream input, byte[,] image, int oy, int ox, int height, int width)
        {
            input.ReadByte();
            int numOfBlocks = input.ReadByte();
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


            int y = 0, x = 0;
            int last = 0;
            for (int c = 0; c < height * width / 4; )
            {
                int flag = input.ReadByte();
                int move = 0;
                bool copy = false;
                if (flag == 255)
                {
                    int amount = input.ReadByte();
                    move = amount;
                    c += amount;
                    copy = true;
                    flag = last;
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
                    copy = true;
                    c++;
                }

                for (int j = 0; j < move; j++)
                {
                    if (copy)
                    {
                        for (int dy = 0; dy < 2; dy++)
                        {
                            for (int dx = 0; dx < 2; dx++)
                            {
                                if (first || (!first && flag >= 0x3F))
                                    image[oy + dy + y, ox + dx + x] = blocks[flag % 0x3F, dy, dx];
                            }
                        }
                    }
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
                last = flag;
            }
            lblocks = blocks;
        }
    }
}
