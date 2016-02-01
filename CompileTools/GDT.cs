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
    public class GDT : ConversionMethod
    {


        public override string Name
        {
            get { return "GDT"; }
        }
        public override string[] Outputs
        {
            get { return new string[] { ".gdt" }; }
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

        }

        int height = 0;
        public override void ConvertFrom(Stream input, Stream output)
        {
            ReadInt16(input);                           // Always x88 xE4?
            ReadInt16(input);                           // ???
            ReadInt16(input);                           // ???

            int width = ReadInt16(input);               // The width
            height = ReadInt16(input);              // The height with scanlines subtracted
            input.ReadByte();                           // Always x11?

            int numberOfBlocks = width / 8;             // Block width is 8px, height is same as image
            pixels = new int[numberOfBlocks,3,height];

            Bitmap bmp = new Bitmap(width, height * 2, PixelFormat.Format32bppRgb);

            long indexOfPrevPlane = 0;
            long endOfPrevPlane = 0;
            for (int block = 0; block < numberOfBlocks; block++)
            {
                for (int plane = 0; plane < 3; plane++)
                {
                    // Read the encoding data and split into half bytes
                    indexOfPrevPlane = input.Position;
                    int data = input.ReadByte();
                    int datat = (data & 0xF0) >> 4;
                    int datab = data & 0x0F;
                    int curLine = 0;

                    if (data == 0) continue;

                    if (datat == 1)
                    {
                        CopyData(block + (plane == 0 ? -1 : 0), block, (plane + 2) % 3, plane);
                    }

                    if (datat == 8)
                    {
                        CopyData(block - datab - 1, block, plane, plane);
                        while (true)
                        {
                            int line = input.ReadByte();
                            data = input.ReadByte();
                            if ((line & 0x80) == 0x80)
                            {
                                if (line == 0xFF) break;
                                WriteData(block, plane, line & ~0x80, data, 1);
                                break;
                            }
                            WriteData(block, plane, line, data, 1);
                        }
                        goto print;
                    }

                    datab &= ~0x8;
                    switch(datab)
                    {
                        case 2:
                            while (curLine < height)
                            {
                                data = input.ReadByte();
                                datat = (data & 0xF0) >> 4;
                                datab = data & 0xF;
                                datab = datab == 0xE ? input.ReadByte() : datab;
                                if (datat == 0x3)
                                {
                                    WriteData(block, plane, curLine, input, datab + 1);
                                }
                                else if (datat != 0x0)
                                {
                                    if (datat == 0x2)
                                    {
                                        data = input.ReadByte();
                                    }
                                    
                                    if (datat >= 0x4 && datat <= 0x07)  // ?????? Maybe
                                    {
                                        datat -= 0x02;
                                        input.Position -= datat;
                                        data = input.ReadByte();
                                        input.Position += datat - 1;
                                    }

                                    if (datat == 0xA)
                                    {
                                        data = 0xC0;
                                    }
                                    WriteData(block, plane, curLine, data, datab + 1);
                                }

                                curLine += datab + 1;
                            }
                            break;
                        case 4:
                            while (curLine < height)
                            {
                                data = input.ReadByte();
                                datat = (data & 0xF0) >> 4;
                                datab = data & 0x0F;
                                int count = 1;

                                // Checking if nibbles equal each other
                                if (datat == datab)
                                {
                                    count = input.ReadByte();
                                    count = count == 0x7E ? input.ReadByte() : count;
                                
                                    // Repeat a direct write
                                    if ((count & 0xF0) == 0x80)
                                    {
                                        count -= 0x80;
                                        if (data == 0)
                                        {
                                            data = (input.ReadByte() << 8) | input.ReadByte();
                                            count *= 2;
                                        }
                                        else
                                        {
                                            data = input.ReadByte();
                                        }
                                    }

                                    // WTF shifting thing ???
                                    if ((count & 0xF0) == 0xC0)
                                    {
                                        count -= 0xC0;
                                        data = input.ReadByte();

                                        for(int x = 0; x < count; x++,data >>= 1, data |= 0x80)
                                        {
                                            WriteData(block, plane, curLine + x, data, 1);
                                        }
                                        curLine += count;
                                        continue;
                                    }
                                }

                                WriteData(block, plane, curLine, data, count);
                                curLine += count;
                            }
                            break;
                        case 6:
                            while (curLine < height)
                            {
                                data = input.ReadByte();
                                datat = (data & 0xF0) >> 4;
                                datab = data & 0x0F;

                                // Checking for direct write 
                                if (datat == 0)
                                {
                                    datat = datab == 0xE ? input.ReadByte() : datab;
                                    WriteData(block, plane, curLine, input, datab);
                                    curLine += datat;
                                    continue;
                                }

                                // Getting actual length if value is greater than 0xD
                                if (datat == 0xE)
                                {
                                    datat = datab;
                                    data = input.ReadByte();
                                    datab = data & 0x0F;
                                    datat = (datat << 4) | (data >> 4);
                                }

                                // Finding pattern
                                switch(datab)
                                {
                                    case 0:
                                        data = 0x00;
                                        break;
                                    case 1:
                                        data = 0x22;
                                        break;
                                    case 2:
                                        data = 0x55;
                                        break;
                                    case 3:
                                        data = 0x77;
                                        break;
                                    case 4:
                                        data = 0xFF;
                                        break;
                                    case 5:
                                        data = 0xDD;
                                        break;
                                    case 6:
                                        data = 0xAA;
                                        break;
                                    case 7:
                                        data = input.ReadByte();
                                        break;
                                    case 0xA:
                                        data = 0x55AA;
                                        break;
                                    case 0xD:
                                        data = 0xDD77;
                                        break;
                                    case 0xF:
                                        data = input.ReadByte();
                                        break;
                                    default:
                                        Console.WriteLine("NR"+datab);
                                        break;
                                }

                                WriteData(block, plane, curLine, data, datat);
                                curLine += datat;
                            }
                            break;
                    }
                    
                    print : endOfPrevPlane = input.Position;


                    input.Position = indexOfPrevPlane;
                    char[] s = ReadStringU(input, (int)(endOfPrevPlane - indexOfPrevPlane)).ToCharArray();
                    Console.Write("{0:D2} : ",block);
                    for (int x = 0; x < s.Length; x++)
                    {
                        Console.Write("{0:X2} ", (int)s[x]);
                    }
                    Console.WriteLine();
                    input.Position = endOfPrevPlane;

                    for (int b = 0; b < numberOfBlocks; b++)
                        for (int p = 0; p < 3; p++)
                            for (int l = 0; l < height; l++)
                            {
                                data = pixels[b, p, l];
                                for (int d = 7; d >= 0; d--, data >>= 1)
                                    if (data % 2 == 1)
                                    {
                                        Color oldc = bmp.GetPixel(b * 8 + d, l * 2);
                                        Color newc = Color.FromArgb(oldc.ToArgb() | GetColor(p));
                                        bmp.SetPixel(b * 8 + d, l * 2, newc);
                                    }
                            }
                    Stream output2 = new FileStream("test/b" + block.ToString("D3") + "p" + plane + ".png", FileMode.Create);
                    bmp.Save(output2, ImageFormat.Png);
                    output2.Close();
                }
            }

            for (int b = 0; b < numberOfBlocks; b++ )
                for (int p = 0; p < 3; p++)
                    for (int l = 0; l < height; l++)
                    {
                        int data = pixels[b, p, l];
                        for (int d = 7; d >= 0; d--, data >>= 1)
                            if (data % 2 == 1)
                            {
                                Color oldc = bmp.GetPixel(b * 8 + d, l * 2);
                                Color newc = Color.FromArgb(oldc.ToArgb() | GetColor(p));
                                bmp.SetPixel(b * 8 + d, l * 2, newc);
                            }
                    }

            bmp.Save(output, ImageFormat.Png);

        }

        int[,,] pixels;

        public int GetColor(int plane)
        {
            unchecked
            {
                if (plane == 0)
                    return (int)0xFF0066FF;
                if (plane == 1)
                    return (int)0xFFFF6600;
                if (plane == 2)
                    return (int)0xFF00FF00;
                return (int)0xFFFFFFFF;
            }
        }

        public void WriteData(int block, int plane, int line, int data, int count)
        {
            if ((data & 0xFF00) > 0)
            {
                for (int l = 0; l < count/2; l++)
                {
                    pixels[block, plane, line + l * 2] = (data & 0xFF00) >> 8;
                    pixels[block, plane, line + l * 2 + 1] = data & 0xFF;
                }
                return;
            }
            for(int l = 0; l < count; l++)
            {
                if(line + l >= height)
                {
                    Console.WriteLine("We have a problem Houston.");
                    return;
                }
                pixels[block, plane, line + l] = data;
            }
        }

        public void WriteData(int block, int plane, int line, Stream input, int count)
        {
            for (int l = 0; l < count; l++)
            {
                pixels[block, plane, line + l] = input.ReadByte();
            }
        }

        public void CopyData(int b1, int b2, int p1, int p2)
        {
            for(int l = 0; l < height; l++)
            {
                pixels[b2, p2, l] = pixels[b1, p1, l];
            }
        }

        public void CopyData(int b1, int b2)
        {
            for (int p = 0; p < 3; p++)
            {
                for (int l = 0; l < height; l++)
                {
                    pixels[b2, p, l] = pixels[b1, p, l];
                }
            }
        }
    }
}
