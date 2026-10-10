using System;
using System.Buffers.Binary;
using System.Text;

using ContreJourDX.Desktop.Platform.Audio;

using Xunit;

namespace ContreJourDX.Desktop.Platform.Tests
{
    public class WavFileTests
    {
        [Fact]
        public void WrapsSamplesInACanonicalPcmHeader()
        {
            byte[] samples = [1, 2, 3, 4, 5, 6, 7, 8];

            byte[] wav = WavFile.Wrap16Bit(channels: 2, frequency: 44100, samples);

            Assert.Equal(44 + samples.Length, wav.Length);
            Assert.Equal("RIFF", Encoding.ASCII.GetString(wav, 0, 4));
            Assert.Equal(36 + samples.Length, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(4)));
            Assert.Equal("WAVEfmt ", Encoding.ASCII.GetString(wav, 8, 8));
            Assert.Equal(16, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(16)));
            Assert.Equal(1, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(20)));
            Assert.Equal(2, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(22)));
            Assert.Equal(44100, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(24)));
            Assert.Equal(44100 * 4, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(28)));
            Assert.Equal(4, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(32)));
            Assert.Equal(16, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(34)));
            Assert.Equal("data", Encoding.ASCII.GetString(wav, 36, 4));
            Assert.Equal(samples.Length, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(40)));
            Assert.Equal(samples, wav[44..]);
        }

        [Fact]
        public void MonoHasTwoBytesPerFrame()
        {
            byte[] wav = WavFile.Wrap16Bit(channels: 1, frequency: 22050, [0, 0]);

            Assert.Equal(22050 * 2, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(28)));
            Assert.Equal(2, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(32)));
        }
    }
}
