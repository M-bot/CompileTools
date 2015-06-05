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
            get { return new string[] { "" }; }
        }
        public override bool Verify(Stream input)
        {
            throw new NotImplementedException();
        }
        public override void ConvertTo(Stream input, Stream output)
        {
            short width = 256;
            short height = 128;
            short numOfColors = 256;
            blockWidth = 128;
            blockHeight = 64;
            WriteString(output, "MMV3");
            WriteInt16(output, 0);
            WriteInt16(output, 1);
            WriteInt16(output, 75);
            WriteInt32(output, 0);
            WriteInt32(output, 0);
            WriteInt16(output, numOfColors);
            output.WriteByte((byte)Math.Log(blockWidth,2));
            output.WriteByte((byte)Math.Log(blockHeight, 2));
            WriteInt16(output, width);
            WriteInt16(output, height);
            WriteInt16(output, 0);
            WriteInt16(output, numOfColors); 
            Color[] colors = new Color[numOfColors];
            for (int i = 0; i < numOfColors; i++)
            {
                byte[] color = new byte[4];
                for (int j = 0; j < 4; j++)
                    output.WriteByte(color[j] = (byte)input.ReadByte());
                colors[i] = Color.FromArgb(color[3], color[0], color[1], color[2]);
            }

            for (int i = 0; i < 4; i++)
            {
                output.WriteByte(1);
                for (int x = 0; x < 4; x++)
                    output.WriteByte(1);
                WriteInt16(output, 0);
                output.WriteByte(0);
                output.WriteByte(0xFF);
                output.WriteByte(0);
            }
            for(int i = 0; i < 75; i++)
            {
                Bitmap bitmap = new Bitmap("test/out" + i + ".png");
                int x = 0, y = 0, dx = blockWidth, dy = blockHeight;
                for (int block = 0; block < 4; block++)
                {

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
            }
        }

        private static int blockHeight = 0;
        private static int blockWidth = 0;
        private static byte[, , ,] bloxels = new byte[4, 0x3F * 4, 2, 2];
        private static byte[,] image;

        public override void ConvertFrom(Stream input, Stream output)
        {
            String identity = ReadString(input, 4);
            ReadString(input, 4);
            int numOfFrames = ReadInt16(input);
            ReadString(input, 8);
            int numOfColors = ReadInt16(input);
            blockWidth = 1 << input.ReadByte();
            blockHeight = 1 << input.ReadByte();
            int width = ReadInt16(input);
            int height = ReadInt16(input);
            image = new byte[height, width];
            ReadInt16(input);
            int importantColors = ReadInt16(input);

            Console.WriteLine("Identity: " + identity);
            Console.WriteLine("Frames: " + numOfFrames);
            Console.WriteLine("Colors: " + numOfColors + " " + importantColors);
            Console.WriteLine("Dimensions: " + width + "px by " + height + "px");
            Console.WriteLine("Block size: " + blockWidth + "px by " + blockHeight + "px");
            Console.WriteLine();

            Color[] colors = new Color[numOfColors];
            for (int i = 0; i < numOfColors; i++ )
            {
                byte[] color = new byte[4];
                for (int j = 0; j < 4; j++ )
                    output.WriteByte(color[j] = (byte)input.ReadByte());
                colors[i] = Color.FromArgb(color[3], color[0], color[1], color[2]);
            }

            int pos = 0;
            for (int frame = 0; frame < numOfFrames; frame++)
            {
                int x = 0, y = 0, dx = blockWidth, dy = blockHeight;
                for (int block = 0; block < 4; block++ )
                {
                    readBlock(input, block, frame, y, x);
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

                Stream output2 = new FileStream("test/out" + frame + ".png", FileMode.Create);
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
                pos += 0x3F;
                pos %= 0x3F * 4;
            }
            Console.WriteLine("Success!");
        }

        public static void readBlock(Stream input, int block, int frame, int imageY, int imageX)
        {
            int numOfBloxels = input.ReadByte();
            if (numOfBloxels == 0x00 && NextByte(input) == 0xFF)
            {
                ReadInt16(input);
                return;
            }
            if (numOfBloxels == 0xFF && NextByte(input) == 0xFE)
            {
                input.ReadByte();
                return;
            }
            if (numOfBloxels == 0x01) // TODO: Fix this flag
            {
                ReadString(input, 9);
                return;
            }

            for (int i = 0; i < numOfBloxels; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        bloxels[block, frame % 4 * 0x3F + i, j, k] = (byte)input.ReadByte();
                    }
                }
            }

            int blockY = 0, blockX = 0;
            int lastByteRead = 0;
            for (int blockPointer = 0; blockPointer < blockHeight * blockWidth / 4; )
            {
                int currentByte = input.ReadByte();
                int amountToWrite = 0;
                bool copy = false;
                if (currentByte == 255)
                {
                    amountToWrite = input.ReadByte();
                    copy = true;
                    currentByte = lastByteRead;
                }
                else if (currentByte == 254)
                {
                    amountToWrite = input.ReadByte() + 1;
                }
                else
                {
                    amountToWrite = 1;
                    copy = true;
                }
                for (int j = 0; j < amountToWrite; j++)
                {
                    if (copy)
                    {
                        for (int bloxelY = 0; bloxelY < 2; bloxelY++)
                        {
                            for (int bloxelX = 0; bloxelX < 2; bloxelX++)
                            {
                                image[imageY + bloxelY + blockY, imageX + bloxelX + blockX] = bloxels[block, currentByte, bloxelY, bloxelX];
                            }
                        }
                    }
                    if (blockX + 2 >= blockWidth)
                    {
                        blockY += 2;
                        blockX = 0;
                    }
                    else
                    {
                        blockX += 2;
                    }
                }

                blockPointer += amountToWrite;
                lastByteRead = currentByte;
            }
        }
    }
}
