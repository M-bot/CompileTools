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

            // Input handled correctly:
            // GAMEOVER.GDT
            // MAP100.GDT (and likely other maps as well, but not tested)
            // all title cards when cropped (remove bottom and right sides; they're only black anyway)
            // EVO (SNES) chapter title 1
            // above images with 16-bit color blocks pasted haphazardly

            // Inputs handled incorrectly:
            // unedited title card images - smearing occurs
            // game title screen (AV04B.GDT) - smearing occurs
            // EVO chapter 1 title screen, doubled in size (but still 640x400 px) - smearing occurs
            // 50% of scanlined images become blank if it's misaligned (you can shift the image up/down to fix this)

            Bitmap bmp = new Bitmap(Bitmap.FromStream(input));
            WriteInt16(output, unchecked((short)0xE488));
            WriteInt32(output, 0);
            WriteInt16(output, (short)bmp.Width);
            WriteInt16(output, (short)(bmp.Height/2));
            output.WriteByte(0x11);

            List<List<int>> planes = new List<List<int>>();
            int currentPlane = -1;

            List<int> blankPlane = new List<int>(Enumerable.Repeat(0x00, bmp.Height / 2));

            // we look at pixel (width + dw) a lot, and dw goes up to 8, so ensure it's in bounds here
            for (int width = 0; width < (bmp.Width - 8); width+=8)
            {
                // First, check if the block is entirely black. If so, we can write "00-00-00" and skip it.
                bool allBlack = true;
                for (int height = 0; height < bmp.Height && allBlack; height += 2)
                {
                    for (int dw = 0; dw < 8 && allBlack; dw++)
                    {
                        if (bmp.GetPixel(width + dw, height).ToArgb() != Color.Black.ToArgb())
                        {
                            allBlack = false;
                        }
                    }
                }

                if (allBlack)
                {
                    // Write a blank block, then proceed to the next block
                    output.WriteByte(0x00);
                    output.WriteByte(0x00);
                    output.WriteByte(0x00);

                    planes.Add(blankPlane);
                    planes.Add(blankPlane);
                    planes.Add(blankPlane);
                    currentPlane += 3;

                    continue;
                }

                for (int plane = 0; plane < 3; plane++)                               // for each plane (B R G):
                {
                    // planeData is the raw line-by-line representation.
                    List<int> planeData = new List<int>();
                    currentPlane++;
                    Console.WriteLine("Processing plane: " + currentPlane);

                   
                    for (int height = 0; height < bmp.Height; height+=2)
                    {
                        int data = 0;
                        for (int dw = 0; dw < 8; dw++)                                // dw = difference in width; column in the block

                        {
                            // If the pixel's color contains a certain threshold of the plane's ideal RGB color, then it's part of that plane data.
                            // pure color -> ingame color
                            // #0000FF B  -> #0066FF (lighter blue)
                            // #FF0000 R  -> #FF6600 (burnt orange)
                            // #00FF00 G  -> #00FF00 (same green)

                            if ((bmp.GetPixel(width + dw, height).B > 0) && plane == 0)
                                // If there's any blue at all, add the data to the blue plane
                            {
                                data |= 1 << (7 - dw);
                            }
                            if ((bmp.GetPixel(width + dw, height).R > 0) && plane == 1)
                                // if there's any red at all, add the data to the red plane
                            {
                                data |= 1 << (7 - dw);
                            }
                            if ((bmp.GetPixel(width + dw, height).G > 102) && plane == 2)
                                // If there's green, but not the mere 0x66 green mixed in the burnt orange/light blue, add it
                            {
                                data |= 1 << (7 - dw);
                            }
                        }

                        planeData.Add(data);
                    }

                    planes.Add(planeData);

                    List<int?> planeRLE = new List<int?>();
                    int? runLengthData = null;
                    int runLength = 0;
                    foreach (var data in planeData)
                    {
                        if (runLengthData == null)
                        {
                            runLengthData = data;
                            runLength = 1;
                        }
                        else
                        {
                            if (data == runLengthData)
                            {
                                runLength++;
                            }
                            else
                            {
                                // run length of 4 has its own prefix control code, due to collision with 0x04 plane definer.
                                if (runLength == 4)
                                {
                                    planeRLE.Add(0xff);
                                    planeRLE.Add(0x84);
                                    planeRLE.Add(runLengthData);
                                }
                                else
                                {
                                    // 0x04 only does run-length encoding on data with equal nibbles!! (often 0x00 or 0xFF)
                                    if (runLengthData >> 4 == (runLengthData & 0xF))
                                    {
                                        planeRLE.Add(runLengthData);
                                        planeRLE.Add(runLength);
                                    }
                                    else
                                    {
                                        for (int i=0; i<runLength; i++)
                                        {
                                            planeRLE.Add(runLengthData);
                                        }
                                    }
                                }
                                runLengthData = data;
                                runLength = 1;
                            }
                        }
                    }
                    // Add the last run as well, which is not caught in the above loop.
                    planeRLE.Add(runLengthData);
                    planeRLE.Add(runLength);

                    // See if 0x10 (repeat previous plane) is a good option.
                    // TODO: This is not a good way of doing this. Hard to test, understand, etc.
                    //int diffWithPreviousPlane = 0;
                    //List<int> xorWithPreviousPlane = new List<int>();
                    //for (int i = 0; i < planeData.Count; i++)
                   // {
                    //    if (planes[currentPlane][i] != planes[currentPlane-1][i])
                     //   {
                     //       //Console.WriteLine(xorWithPreviousPlane);
                     //       Console.WriteLine("this data:" + planes[currentPlane][i]);
                     //       Console.WriteLine("last data: " + planes[currentPlane - 1][i]);
                            //int xor = planeData[i] ^ planes[currentPlane - 1][i];
                            //Console.WriteLine("xor:" + xor);
                            //xorWithPreviousPlane.Add(planeData[i] ^ planes[currentPlane - 1][i]);
                     //       diffWithPreviousPlane++;
                     //   }
                        //else
                        //{
                        //    xorWithPreviousPlane.Add(0);
                        // }
                    //}
                    //Console.WriteLine("diff with previous plane: " + diffWithPreviousPlane);

                    if (currentPlane == 0)
                    {
                        //Console.WriteLine("writing RLE");
                        output.WriteByte(0x04);
                        foreach (int d in planeRLE)
                        {
                            output.WriteByte((byte)d);
                        }

                    }
                    else
                    {
                        //if (diffWithPreviousPlane == 0)
                        //{
                        //   // seems to be no restriction on how many 0x10s in a row.
                        //    // But it gives me that lovely cyan instead of the white it should be... why?
                        //    output.WriteByte(0x10);
                        //    Console.WriteLine("repeat plane - 0x10");
                        //}
                        //else { 
                        //    Console.WriteLine("writing RLE");
                        //    output.WriteByte(0x04);
                        //    foreach (int d in planeRLE)
                        //   {
                        //        output.WriteByte((byte)d);
                        //}
                        //}
                        Console.WriteLine("writing RLE");
                        output.WriteByte(0x04);
                        foreach (int d in planeRLE)
                        {
                            output.WriteByte((byte)d);
                        }
                    }
                }
            }

            if (output.Length == 11)
            {
                // It's probably misreading a scanliend image.
                Console.WriteLine("Warning: Looks like the output image is blank.");
                Console.WriteLine("If the input image has scanlines, try shifting it up or down a line.");
            }
        }

        int height = 0;
        bool wtf = false;
        public override void ConvertFrom(Stream input, Stream output)
        {
            // Discarding 6 bytes that appears to always start with 0x88 0xE4
            // They probably provide information but this program doesn't know how to deal with them
            ReadInt16(input);                           
            ReadInt16(input);                           
            ReadInt16(input);                           

            // Reading the width and height, which is then followed by the end of the "header"
            // The last byte appears to always be 0x11
            int width = ReadInt16(input);               
            height = ReadInt16(input);                  
            input.ReadByte();                           

            // The format encodes the image in a series of blocks that are 8 pixels wide and image height tall.
            // Each block is seperated into three color planes that hold various instructions on what to draw.
            // We initialize the buffer we will be writing to as three dimensional array of integers. 
            // Each integer represents an 8 pixel draw line at a certain height within a block's color plane.
            int numberOfBlocks = width / 8;             
            pixels = new int[numberOfBlocks, 3, height];

            
            // This is the buffer that will be used to save the file once we are done decoding
            Bitmap bmp = new Bitmap(width, height * 2, PixelFormat.Format32bppRgb);
            
            // A try finally is used to make sure we always dump the last image created into the file
            // This is done since we don't know how to entirely decode the image at times and the 
            // program will fail. Dumping the last image allows us to see the progress it made.
            try
            {

                long indexOfPrevPlane = 0;
                long endOfPrevPlane = 0;
                for (int block = 0; block < numberOfBlocks; block++)
                {
                    for (int plane = 0; plane < 3; plane++)
                    {
                        indexOfPrevPlane = input.Position;
                        
                        // Reads the encoding flag and splits it into nibbles
                        int data = input.ReadByte();
                        int datat = data >> 4;
                        int datab = data & 0x0F;
                        
                        // Since the each plane is not a continous set of a data  we have to keep track
                        // of the currrent line so we can write data in the correct area as well as keep
                        // within the bounds of the image
                        int curLine = 0;
                        
                        // Use a boolean to notify of known errors that didn't cause the program to crash
                        bool error = false;

                        // The format allows an empty (black) plane to be encoded as 0x00
                        if (data == 0) continue;


                        wtf = false;
                        if ((datab & 0x8) == 0x8)
                        {
                            // copy the blue plane of this block
                            CopyData(block, block, 0, plane);
                            wtf = true;
                        }
                        datab &= ~0x8;

                        if (datat == 0xC)
                        {
                            CopyData(block - 1, block, plane, plane);
                        }

                        if ((datat & 0x1) == 0x1)
                        {
                            // if the flag begins with 0x1, copy the previous plane
                            // if current plane is the blue plane, that means copy the (green?) plane of the previous block
                            CopyData(block + (plane == 0 ? -1 : 0), block, (plane + 2) % 3, plane);
                            wtf = true;
                        }
                        datat &= ~0x1;

                        if (datat == 8)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            CopyData(block - datab - 1, block, plane, plane);
                            while (true)
                            {
                                int line = input.ReadByte();
                                data = input.ReadByte();
                                if (line >= height)
                                {
                                    if (line == 0xFF) break;
                                    WriteData(block, plane, line &= ~0x80, data, 1);
                                    break;
                                }
                                WriteData(block, plane, line, data, 1);
                            }
                            goto print;
                        }
                        switch (datab)
                        {
                            case 0:
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.Green;
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
                                        switch (datat)
                                        {
                                            case 2:
                                                data = input.ReadByte();
                                                break;
                                            case 4:
                                            case 5:
                                            case 6:
                                            case 7:
                                                datat -= 0x02;
                                                input.Position -= datat;
                                                data = input.ReadByte();
                                                input.Position += datat - 1;
                                                break;
                                            case 8:
                                                data = 0xFF;
                                                break;
                                            case 0xA:
                                                data = 0xC0;
                                                break;
                                            default:
                                                error = true;
                                                Console.WriteLine("2NR" + datat);
                                                break;

                                        }
                                        WriteData(block, plane, curLine, data, datab + 1);
                                    }

                                    curLine += datab + 1;
                                }
                                break;
                            case 3:
                                data = input.ReadByte();
                                WriteData(block, plane, 0, data, height);
                                break;
                            case 4:
                                // description of the 0x04 plane encoding
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                while (curLine < height)
                                {
                                    data = input.ReadByte();       // first byte: direct binary representation of a line
                                    datat = (data & 0xF0) >> 4;    // upper nibble
                                    datab = data & 0x0F;           // lower nibble
                                    int count = 1;

                                    // Checking if nibbles equal each other
                                    if (datat == datab)
                                    {
                                        count = input.ReadByte();

                                        // WTF shifting thing ???
                                        if ((count & 0xF0) == 0xC0)
                                        {
                                            count -= 0xC0;
                                            if (data == 0xFF) 
                                                data = input.ReadByte();
                                            if (data == 0x00)
                                            {
                                                // This obviously doesn't work for the rotate thing
                                                data = (input.ReadByte() << 8) | input.ReadByte();
                                                count *= 2;
                                            }

                                            for (int x = 0; x < count; x++)
                                            {
                                                WriteData(block, plane, curLine + x, data, 1);
                                                datab = (data & 0x1) << 7;      // Rotate left 1
                                                data = (data >> 1) | datab;
                                            }
                                            curLine += count;
                                            continue;
                                        }

                                        // Repeat a direct write
                                        if ((count & 0x80) == 0x80)
                                        {
                                            count -= 0x80;
                                            switch (datab)
                                            {
                                                case 0:
                                                    data = (input.ReadByte() << 8) | input.ReadByte();
                                                    count *= 2;
                                                    break;
                                                case 5:
                                                    data = 0x55AA;
                                                    break;
                                                case 0xA:
                                                    data = 0xAA55;
                                                    break;
                                                case 0xF:
                                                    data = input.ReadByte();
                                                    break;
                                                default:
                                                    error = true;
                                                    Console.WriteLine("4NR" + datab);
                                                    break;
                                            }
                                        }

                                        count = count == 0x7E ? input.ReadByte() : count;
                                    }

                                    WriteData(block, plane, curLine, data, count);
                                    curLine += count;
                                }
                                break;
                            case 6:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                while (curLine < height)
                                {
                                    data = input.ReadByte();
                                    datat = (data & 0xF0) >> 4;
                                    datab = data & 0x0F;

                                    if (data == 0)
                                    {
                                        datat = input.ReadByte();
                                        datat = ((datat & 0x80) == 0x80 ? datat - 0x80 : datat) * 2;
                                        data = (input.ReadByte() << 8) | input.ReadByte();
                                        WriteData(block, plane, curLine, data, datat);
                                        goto end6;
                                    }

                                    // Checking for direct write 
                                    if (datat == 0)
                                    {
                                        datat = datab == 0xE ? input.ReadByte() : datab;
                                        WriteData(block, plane, curLine, input, datat);
                                        goto end6;
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
                                    switch (datab)
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
                                        case 9:
                                            data = 0x2288;
                                            break;
                                        case 0xA:
                                            data = 0x55AA;
                                            break;
                                        case 0xB:
                                            data = 0x77DD;
                                            break;
                                        case 0xD:
                                            data = 0xDD77;
                                            break;
                                        case 0xE:
                                            data = 0xAA55;
                                            break;
                                        case 0xF:
                                            data = input.ReadByte();
                                            for (int x = 0; x < datat; x++)
                                            {
                                                WriteData(block, plane, curLine + x, data, 1);
                                                datab = (data & 0x3) << 6;      // Rotate left 2
                                                data = (data >> 2) | datab;
                                            }
                                            goto end6;
                                        default:
                                            error = true;
                                            Console.WriteLine("NR" + datab);
                                            break;
                                    }

                                    WriteData(block, plane, curLine, data, datat);
                           end6:    curLine += datat;
                                }
                                break;
                            default:
                                error = true;
                                break;
                        }

                    print: endOfPrevPlane = input.Position;

                        if (error)
                            Console.ForegroundColor = ConsoleColor.Red;
                        input.Position = indexOfPrevPlane;
                        char[] s = ReadStringU(input, (int)(endOfPrevPlane - indexOfPrevPlane)).ToCharArray();
                        Console.Write("{0:D2} : ", block);
                        for (int x = 0; x < s.Length; x++)
                        {
                            Console.Write("{0:X2} ", (int)s[x]);
                        }
                        Console.WriteLine();
                        input.Position = endOfPrevPlane;
                        Console.ForegroundColor = ConsoleColor.White;

                        for (int b = 0; b < numberOfBlocks; b++)
                            for (int p = 0; p < 3; p++)
                                for (int l = 0; l < height; l++)
                                {
                                    data = pixels[b, p, l];
                                    for (int d = 7; d >= 0; d--, data >>= 1)
                                        if (data % 2 == 1)
                                        {
                                            Color oldc = bmp.GetPixel(b * 8 + d, l * 2);
                                            // take the binary or of what's already there and the current plane color
                                            Color newc = Color.FromArgb(oldc.ToArgb() | GetColor(p));
                                            bmp.SetPixel(b * 8 + d, l * 2, newc);
                                        }
                                }

                        Directory.CreateDirectory("test/");
                        Stream output2 = new FileStream("test/b" + block.ToString("D3") + "p" + plane + ".png", FileMode.Create);
                        bmp.Save(output2, ImageFormat.Png);
                        output2.Close();
                    }
                }
            }
            finally
            {
                for (int b = 0; b < numberOfBlocks; b++)
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

        }

        int[,,] pixels;

        public int GetColor(int plane)
        {
            unchecked
            {
                if (plane == 0)
                    // Values are used in FromArgb / ToArgb.
                    // lighter blue
                    return (int)0xFF0066FF;
                if (plane == 1)
                    // burnt orange
                    return (int)0xFFFF6600;
                if (plane == 2)
                    // normal green
                    return (int)0xFF00FF00;
                return (int)0xFFFFFFFF;
            }
        }

        public void WriteData(int block, int plane, int line, int data, int count)
        {
            if (wtf)
                // wtf indeed!
            {
                // hmm. when does this apply? data should be 0xFF at its highest, right?
                if ((data & 0xFF00) > 0)
                {
                    for (int l = 0; l < count / 2; l++)
                    {
                        pixels[block, plane, line + l * 2] ^= (data & 0xFF00) >> 8;
                        pixels[block, plane, line + l * 2 + 1] ^= data & 0xFF;
                    }
                    if (count % 2 == 1)
                        pixels[block, plane, line + count - 1] ^= (data & 0xFF00) >> 8;
                    return;
                }
                for (int l = 0; l < count; l++)
                {
                    // use the bitwise logical XOR between the data and... what's already there??
                    // oh yeah, the plane that got copied with CopyData.
                    pixels[block, plane, line + l] ^= data;
                }
            }
            else
            {
                if ((data & 0xFF00) > 0)
                {
                    for (int l = 0; l < count / 2; l++)
                    {
                        pixels[block, plane, line + l * 2] = (data & 0xFF00) >> 8;
                        pixels[block, plane, line + l * 2 + 1] = data & 0xFF;
                    }
                    if (count % 2 == 1)
                        pixels[block, plane, line + count - 1] = (data & 0xFF00) >> 8;
                    return;
                }
                for (int l = 0; l < count; l++)
                {
                    pixels[block, plane, line + l] = data;
                }
            }
        }

        public void WriteData(int block, int plane, int line, Stream input, int count)
        {
            for (int l = 0; l < count; l++)
            {
                if (wtf)
                {
                    pixels[block, plane, line + l] ^= input.ReadByte();
                }
                else
                {
                    pixels[block, plane, line + l] = input.ReadByte();
                }
            }
        }

        public void CopyData(int b1, int b2, int p1, int p2)
        {
            for(int l = 0; l < height; l++)
            {
                pixels[b2, p2, l] ^= pixels[b1, p1, l];
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
