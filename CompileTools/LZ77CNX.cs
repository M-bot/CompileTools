using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CompileTools
{
    public class LZ77CNX : CompressionMethod
    {
        public const uint SEARCH_WINDOW_SIZE = 0x800; // 2048
        public const uint LOOKAHEAD_SIZE = 0x1F + 4; // 35
        public const uint MINIMUM_MATCH_SIZE = 4;
        public const uint MAXIMUM_MATCH_SIZE = 35;

        public const byte FLAG_SKIP = 0;
        public const byte FLAG_SINGLE_BYTE = 1;
        public const byte FLAG_COMPRESSION_PAIR = 2;
        public const byte FLAG_MULTI_BYTE = 3;

        public const uint OFFSET_BITMASK = 0xFFE0;
        public const uint LENGTH_BITMASK = 0x001F;

        public const uint OFFSET_DIFFERENCE = 1;
        public const uint LENGTH_DIFFERENCE = 4;

        public const int HEADER_LENGTH = 16;

        public override string Name
        {
            get { return "LZ77-CNX"; }
        }

        public override bool Verify(Stream input)
        {
            throw new NotImplementedException();
        }

        // With some slight modification through the actual implementation of a sliding window, this could quickly become a
        // moderately fast streaming IO implementation (which would be great, because constant memory use).
        public override void Compress(Stream input, Stream output)
        {
            byte[] source = new byte[input.Length];
            input.Read(source, 0, source.Length);

            byte[] header = ASCIIEncoding.ASCII.GetBytes("CNX.GMP"); //This wrong btw. Its not always GMP
            header[3] = 0x2; // Some wierd hack with the name.

            output.Write(header, 0, 7);
            output.WriteByte(0x10); // Spacer?
            WriteBigEndianInt32(output, 0);
            WriteBigEndianInt32(output, (int) input.Length);

            long windowPointer = 0; // The location of the sliding window; to the left is the search window and to the right is the lookahead buffer.
            
            // If we can't compress a byte, we throw it into the junk as punishment for it's uncompressionability(?)
            MemoryStream junkOutput = new MemoryStream();

            // The list of ops which we need to dump to the stream.
            Queue<Op> currentOps = new Queue<Op>();

            while (windowPointer < source.Length)
            {
                Match match = FindMatch(source, windowPointer, SEARCH_WINDOW_SIZE);
                if (!match.Success)
                    junkOutput.WriteByte(source[windowPointer++]); // No match, so write this byte to the junk and increment the pointer.
                else
                {
                    // Before we can output the match to the stream, we have to dump all the bytes that failed matches first
                    List<Op> junkOps = GetJunkOps(junkOutput);
                    junkOutput.Close();
                    junkOutput = new MemoryStream();

                    foreach (Op op in junkOps) currentOps.Enqueue(op);
                    currentOps.Enqueue(new MatchOp(match));

                    windowPointer += match.Length; // Our pointer moves forward by the amount of the match
                }

                WriteOps(output, currentOps);
            }

            // Clean the junk one last time
            List<Op> stragglers = GetJunkOps(junkOutput);
            foreach (Op op in stragglers) currentOps.Enqueue(op);

            if (currentOps.Count > 0)
            {
                while (currentOps.Count % 4 != 0)
                    currentOps.Enqueue(new MultibyteOp(new byte[0], 0, 0));

                WriteOps(output, currentOps);
            }

            // And finally emit the compressed size
            output.Seek(8, SeekOrigin.Begin);
            WriteBigEndianInt32(output, (int)(output.Length - HEADER_LENGTH));

            // Close our memory stream
            junkOutput.Close();
        }

        public void WriteOps(Stream output, Queue<Op> ops)
        {
            Op[] outputOps = new Op[4];

            while (ops.Count >= 4)
            {
                for (int x = 0; x < 4; x++)
                    outputOps[x] = ops.Dequeue();

                byte opCode = CombineOps(new byte[] { outputOps[0].OpCode, outputOps[1].OpCode, outputOps[2].OpCode, outputOps[3].OpCode });

                ByteOp.WriteByte(output, opCode);
                foreach (Op op in outputOps)
                    op.Write(output);
            }
        }

        public List<Op> GetJunkOps(MemoryStream stream)
        {
            List<Op> ops = new List<Op>();

            byte[] output = stream.ToArray();
            for (int x = 0; x < output.Length; x += 255)
            {
                int length = Math.Min(255, output.Length - x);
                if (length == 1)
                    ops.Add(new ByteOp(output[x]));
                else
                    ops.Add(new MultibyteOp(output, x, length));
            }
            return ops;
        }

        protected Match FindMatch(byte[] input, long windowPointer, uint maxSearchDistance)
        {
            // Brute force solution - simply searches back by maxSearchDistance, looking for the greatest match which is greater
            // than or equal to MINIMUM_MATCH_SIZE and less or equal to than MAXIMUM_SEARCH_SIZE
            uint bestOffset = 0;
            uint bestLength = 0;
            
            for(long index = windowPointer - 1; index >= Math.Max(0, windowPointer - maxSearchDistance); index--)
            {
                if (input[index] != input[windowPointer]) continue; // If the bytes don't match, continue on.

                // Determine the maximum match length - this can overflow into the lookahead window, which enables run-length encoding.
                uint length = 1;
                while (length < MAXIMUM_MATCH_SIZE && (windowPointer + length < input.Length)
                      && (input[index + length] == input[windowPointer + length])) length++;

                // Compare to our current best length
                if (length > bestLength && length >= MINIMUM_MATCH_SIZE)
                {
                    bestLength = length;
                    bestOffset = (uint) (windowPointer - index);

                    // This is a special case: we're not going to find a better match if we've already reached the limit, so quit immediately.
                    if (bestLength == MAXIMUM_MATCH_SIZE)
                        break;
                }
            }

            return new Match { Success = (bestLength != 0), Length = bestLength, Offset = bestOffset };
        }

        // See OpPointer.CombineOps; should probably move CombineOps to a static method in this class
        protected static byte[] UncombineOps(byte b)
        {
            byte[] res = new byte[4];
            for (int x = 0; x < 4; x++)
            {
                res[x] = (byte) (b & 0x3);
                b >>= 2;
            }
            return res;
        }

        protected static byte CombineOps(byte[] indivOps)
        {
            byte res = 0x0;
            for (int x = indivOps.Length - 1; x >= 0; x--)
                res = (byte)((res << 2) | indivOps[x]);
            return res;
        }

        public override void Decompress(Stream input, Stream output)
        {
            // Might want to check that header is GMP\x02CNX\x10
            byte[] junk = new byte[8];
            input.Read(junk, 0, 8);

            int compressedSize = ReadBigEndianInt32(input);
            int decompressedSize = ReadBigEndianInt32(input);

            // Console.WriteLine("Expecting a decompressed size of " + decompressedSize + " and a compressed size of " + compressedSize);

            byte[] result = new byte[decompressedSize];

            long currentPointer = 0;

            bool error = false;
            try
            {
                while (currentPointer < result.Length)
                {
                    byte header = (byte)input.ReadByte();
                    byte[] decodedHeaders = UncombineOps(header);

                    // Console.WriteLine("[{0:X8}] Found headers: " + string.Join(", ", decodedHeaders), input.Position - 1);

                    foreach (byte opNum in decodedHeaders)
                    {
                        // Console.WriteLine("Before operation " + opNum + ", pointer at {0:X8}.", currentPointer);
                        OpFactories[opNum](input).ReadInto(result, ref currentPointer);
                        if (opNum == FLAG_SKIP) // This is kinda funky, but you ignore all the remaining Ops if you get a skip flag.
                            break;
                    }

                    // Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                // Dump what we have
                output.Write(result, 0, (int)Math.Min(result.Length, currentPointer));
                error = true;

                Console.WriteLine("Error, dumping data early. Got to {0:X8}.", currentPointer);
            }

            if (!error)
                output.Write(result, 0, result.Length);
        }

        // A Match Struct is returned by FindMatch() when it finds an Offset and Length pair.
        public struct Match
        {
            public bool Success;
            public uint Offset;
            public uint Length;
        }

        // Utility classes for reading/writing specific types of ops

        public delegate Op OpFactory(Stream input);

        public static OpFactory[] OpFactories = 
        {
            (Stream str) => new SkipOp(str),
            (Stream str) => new ByteOp(str),
            (Stream str) => new MatchOp(str),
            (Stream str) => new MultibyteOp(str)
        };

        public abstract class Op
        {
            private byte opCode;

            public byte OpCode
            {
                get { return opCode; }
            }

            public Op(byte code)
            {
                opCode = code;
            }

            public abstract void Write(Stream output);
            public abstract void ReadInto(byte[] target, ref long pointer);
        }

        public class SkipOp : Op
        {
            public byte Amount;

            public SkipOp(byte amount)
                : base(0)
            {
                Amount = amount;
            }

            public SkipOp(Stream input)
                : base(0)
            {
                Amount = ByteOp.ReadByte(input);
                // This is super hacky. Would recommend fixing very much. Probably.
                byte[] junk = new byte[Amount];
                input.Read(junk, 0, Amount);
            }

            public override void Write(Stream output)
            {
                ByteOp.WriteByte(output, Amount);
            }

            public override void ReadInto(byte[] target, ref long pointer)
            {
                // Do nothing.
            }
        }

        public class MatchOp : Op
        {
            public Match Match;

            public MatchOp(Match match)
                : base(2)
            {
                Match = match;
            }

            // Obviously, passing a stream to a constructor is kinda stupid, but we deal with it.
            public MatchOp(Stream input) : base(2)
            {
                Match = ReadCompressionPair(input);
            }

            public static void WriteCompressionCode(Stream stream, Match match)
            {
                uint realOffset = match.Offset - OFFSET_DIFFERENCE;
                uint realLength = match.Length - LENGTH_DIFFERENCE;

                uint res = ((realOffset << 5) & OFFSET_BITMASK) | (realLength & LENGTH_BITMASK);

                stream.WriteByte((byte)((res >> 8) & 0xFF));
                stream.WriteByte((byte)(res & 0xFF));
            }

            public static Match ReadCompressionPair(Stream stream)
            {
                int encoded = (ByteOp.ReadByte(stream) << 8) | ByteOp.ReadByte(stream);

                uint length = (uint)(encoded & LENGTH_BITMASK);
                uint offset = (uint)((encoded & OFFSET_BITMASK) >> 5);

                return new Match { Success = true, Length = length + LENGTH_DIFFERENCE, Offset = offset + OFFSET_DIFFERENCE };
            }

            public override void Write(Stream output)
            {
                WriteCompressionCode(output, Match);
            }

            public override void ReadInto(byte[] target, ref long pointer)
            {
                for (int pos = 0; pos < Match.Length; pos++)
                {
                    target[pointer] = target[pointer - Match.Offset];
                    pointer++;
                }
            }
        }

        public class ByteOp : Op
        {
            public byte buf;

            public ByteOp(byte b)
                : base(1)
            {
                buf = b;
            }

            public ByteOp(Stream input)
                : base(1)
            {
                buf = ReadByte(input);
            }

            public static void WriteByte(Stream output, byte b)
            {
                output.WriteByte(b);
            }

            public static byte ReadByte(Stream input)
            {
                return (byte)input.ReadByte();
            }

            public override void Write(Stream output)
            {
                WriteByte(output, buf);
            }

            public override void ReadInto(byte[] target, ref long pointer)
            {
                target[pointer++] = buf;
 	        }
        }

        public class MultibyteOp : Op
        {
            public byte[] Buffer;
            public int Offset;
            public int WriteLength;

            public MultibyteOp(byte[] buff, int offset, int writeLength) : base(3) 
            {
                Buffer = buff;
                Offset = offset;
                WriteLength = writeLength;
            }

            public MultibyteOp(Stream input)
                : base(3)
            {
                Buffer = ReadMultiByte(input);
                Offset = 0;
                WriteLength = Buffer.Length;
            }

            public static void WriteMultiByte(Stream stream, byte[] arr, int offset, int length)
            {
                stream.WriteByte((byte)length);
                stream.Write(arr, offset, length);
            }

            public static byte[] ReadMultiByte(Stream stream)
            {
                byte length = ByteOp.ReadByte(stream);
                byte[] temp = new byte[length];
                stream.Read(temp, 0, length);
                return temp;
            }

            public override void Write(Stream output)
            {
                WriteMultiByte(output, Buffer, Offset, WriteLength);
            }

            public override void ReadInto(byte[] target, ref long pointer)
            {
                for (int x = 0; x < WriteLength; x++)
                    target[pointer++] = Buffer[x];
            }
        }
    }
}
