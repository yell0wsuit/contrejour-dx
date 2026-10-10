using System;
using System.IO;

namespace ContreJourDX.Desktop.Platform.Audio
{
    // Builds an in-memory WAV file. SDL3-CS binds the mixer's raw-sample loader with the wrong
    // parameter type for its format, so converted samples go through the WAV decoder instead, which
    // reads them back unchanged (adapted from cuttherope-dx).
    internal static class WavFile
    {
        private const int HeaderBytes = 44;

        public static byte[] Wrap16Bit(int channels, int frequency, ReadOnlySpan<byte> samples)
        {
            int frameBytes = channels * sizeof(short);
            byte[] wav = new byte[HeaderBytes + samples.Length];
            using (BinaryWriter writer = new(new MemoryStream(wav)))
            {
                writer.Write("RIFF"u8);
                writer.Write(HeaderBytes - 8 + samples.Length);
                writer.Write("WAVEfmt "u8);
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(frequency);
                writer.Write(frequency * frameBytes);
                writer.Write((short)frameBytes);
                writer.Write((short)16);
                writer.Write("data"u8);
                writer.Write(samples.Length);
            }
            samples.CopyTo(wav.AsSpan(HeaderBytes));
            return wav;
        }
    }
}
