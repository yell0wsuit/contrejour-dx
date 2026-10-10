using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ContreJourDX.Regression
{
    // Writes captured frames as 8-bit RGBA PNGs with no filtering. It does not use MonoGame, so frames
    // from the step-4 Skia host can be saved and compared by eye with the same code.
    internal static class PngWriter
    {
        private static readonly byte[] Signature = [137, 80, 78, 71, 13, 10, 26, 10];

        private static readonly uint[] CrcTable = CreateCrcTable();

        public static void Write(string path, byte[] rgba, int width, int height)
        {
            using FileStream file = File.Create(path);
            file.Write(Signature);

            byte[] header = new byte[13];
            BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(0), (uint)width);
            BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(4), (uint)height);
            header[8] = 8; // bits per channel
            header[9] = 6; // RGBA
            WriteChunk(file, "IHDR", header);

            using MemoryStream compressed = new();
            using (ZLibStream zlib = new(compressed, CompressionLevel.Fastest, leaveOpen: true))
            {
                int stride = width * 4;
                byte[] row = new byte[stride];
                for (int y = 0; y < height; y++)
                {
                    Array.Copy(rgba, y * stride, row, 0, stride);
                    // The back buffer's alpha is not what the screen shows; make the image opaque.
                    for (int x = 3; x < stride; x += 4)
                    {
                        row[x] = byte.MaxValue;
                    }
                    zlib.WriteByte(0); // filter: none
                    zlib.Write(row);
                }
            }
            WriteChunk(file, "IDAT", compressed.ToArray());
            WriteChunk(file, "IEND", []);
        }

        private static void WriteChunk(FileStream stream, string type, byte[] data)
        {
            byte[] number = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(number, (uint)data.Length);
            stream.Write(number);
            byte[] typeBytes = Encoding.ASCII.GetBytes(type);
            stream.Write(typeBytes);
            stream.Write(data);
            uint crc = UpdateCrc(UpdateCrc(0xFFFFFFFFu, typeBytes), data) ^ 0xFFFFFFFFu;
            BinaryPrimitives.WriteUInt32BigEndian(number, crc);
            stream.Write(number);
        }

        private static uint UpdateCrc(uint crc, byte[] data)
        {
            foreach (byte b in data)
            {
                crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            return crc;
        }

        private static uint[] CreateCrcTable()
        {
            uint[] table = new uint[256];
            for (uint n = 0; n < 256; n++)
            {
                uint c = n;
                for (int k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
                }
                table[n] = c;
            }
            return table;
        }
    }
}
