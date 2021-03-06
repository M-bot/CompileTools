﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CompileTools
{
    public abstract class Method
    {
        public abstract string Name
        {
            get;
        }
        public abstract string[] Outputs
        {
            get;
        }
        public abstract string[] Inputs
        {
            get;
        }
        public abstract bool Verify(Stream input);

        public bool IsOutput(string s)
        {
            return Outputs.Contains(s) || Outputs.Contains("*");
        }

        public bool IsInput(string s)
        {
            return Inputs.Contains(s) || Outputs.Contains("*");
        }

        public static void WriteBigEndianInt32(Stream output, int number)
        {
            byte[] res = BitConverter.GetBytes(number);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(res);
            output.Write(res, 0, res.Length);
        }

        public static void WriteInt32(Stream output, int number)
        {
            byte[] res = BitConverter.GetBytes(number);
            output.Write(res, 0, res.Length);
        }

        public static void WriteInt16(Stream output, Int16 number)
        {
            byte[] res = BitConverter.GetBytes(number);
            output.Write(res, 0, res.Length);
        }

        public static int ReadBigEndianInt32(Stream input)
        {
            byte[] res = new byte[4];
            input.Read(res, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(res);

            return BitConverter.ToInt32(res, 0);
        }

        public static int ReadInt32(Stream input)
        {
            byte[] res = new byte[4];
            input.Read(res, 0, 4);

            return BitConverter.ToInt32(res, 0);
        }

        public static int ReadInt16(Stream input)
        {
            byte[] res = new byte[2];
            input.Read(res, 0, 2);

            return BitConverter.ToInt16(res, 0);
        }

        public static string ReadString(Stream input, int length)
        {
            char[] strArr = new char[length];
            int count = 0;
            for (int x = 0; x < strArr.Length; x++)
            {
                strArr[x] = (char)input.ReadByte();
                if(strArr[x] == '\0')
                    count++;
            }
            string str = new string(strArr);
            return str.Substring(0,str.Length - count);
        }
        public static string ReadStringU(Stream input, int length)
        {
            char[] strArr = new char[length]; 
            for (int x = 0; x < strArr.Length; x++)
            {
                strArr[x] = (char)input.ReadByte();
            }
            return new string(strArr);
        }

        public static void WriteString(Stream input, string str)
        {
            for (int x = 0; x < str.Length; x++)
            {
                input.WriteByte((byte)str[x]);
            }
        }

        public static void WriteString(Stream input, string str, int nullpadding)
        {
            for (int x = 0; x < str.Length; x++)
            {
                input.WriteByte((byte)str[x]);
            }
            for (int x = 0; x < nullpadding - str.Length; x++)
            {
                input.WriteByte((byte)0);
            }
        }
        public static int NextInt16(Stream input)
        {
            int x = ReadInt16(input);
            input.Seek(-2, SeekOrigin.Current);
            return x;
        }

        public static int NextByte(Stream input)
        {
            int x = input.ReadByte();
            input.Seek(-1, SeekOrigin.Current);
            return x;
        }

        public static int CopyBytes(Stream input, Stream output, int num)
        {
            for (int x = 0; x < num; x++)
            {
                output.WriteByte((byte)input.ReadByte());
            }
            return num;
        }

        public static int CopyBytes(Stream input, Stream output)
        {
            for (int x = 0; x < input.Length; x++)
            {
                output.WriteByte((byte)input.ReadByte());
            }
            return (int)input.Length;
        }

        public static FileReference FindFile(FileReference[] files, string filename)
        {
            FileReference found = null;
            foreach(FileReference f in files)
            {
                if(f.FileName == filename)
                {
                    found = f;
                }
            }
            return found;
        }
    }
}
