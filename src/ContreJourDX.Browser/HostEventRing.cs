using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Threading;

namespace ContreJourDX.Browser
{
    /// <summary>What a host event carries.</summary>
    internal enum HostEventKind
    {
        None = 0,

        // Word0 phase | kind << 8, Word1 pointer id, Word2/Word3 CSS x/y bits, Word4 DOM buttons.
        Pointer = 1,

        // Word0 down flag, Word1 BrowserKeys id.
        Key = 2,

        // Word0 units, 120 per notch, positive away from the player.
        Wheel = 3,

        // Word0 visible-and-focused flag, Word1 hidden flag.
        Active = 4,

        // Word0/Word1 CSS width/height bits, Word2 pixel ratio bits.
        Resize = 5,

        // The player pressed Play.
        Start = 6,
    }

    /// <summary>One record as it travels through the ring.</summary>
    /// <remarks>
    /// The payload is five raw words because the kinds disagree about their shape and
    /// the record has to stay a fixed size. Float payloads are carried as their bit
    /// patterns; <see cref="BitConverter.Int32BitsToSingle"/> reads them back.
    /// </remarks>
    internal readonly record struct HostEvent(
        HostEventKind Kind,
        int Word0,
        int Word1,
        int Word2,
        int Word3,
        int Word4);

    /// <summary>
    /// A single-writer, single-reader ring in wasm memory. The page writes it and the game
    /// loop drains it once per frame. Shared memory in the threaded build, which is why it
    /// is a ring and not a direct call.
    /// </summary>
    /// <remarks>
    /// Both indices count records rather than addressing slots, so the reader can tell
    /// a full ring from an empty one without a spare slot. The slot is the index masked
    /// by the capacity, which is why the capacity is a power of two.
    /// </remarks>
    internal static class HostEventRing
    {
        /// <summary>Records the ring holds before it starts dropping.</summary>
        internal const int Capacity = 1024;

        /// <summary>Bytes of header ahead of the first record.</summary>
        internal const int HeaderBytes = 16;

        /// <summary>Bytes per record: the kind and five payload words.</summary>
        internal const int RecordBytes = 24;

        private const int WriteIndexOffset = 0;
        private const int ReadIndexOffset = 4;
        private const int DroppedOffset = 8;
        private const int CapacityOffset = 12;

        /// <summary>Total bytes the ring occupies.</summary>
        internal static int BufferBytes => HeaderBytes + (RecordBytes * Capacity);

        /// <summary>Zeroes the header and records the capacity the writer must honor.</summary>
        internal static void Initialize(Span<byte> buffer)
        {
            buffer[..HeaderBytes].Clear();
            BinaryPrimitives.WriteInt32LittleEndian(
                buffer[CapacityOffset..], Capacity);
        }

        /// <summary>Copies out every record written since the last drain.</summary>
        /// <returns>How many records were written into <paramref name="into"/>.</returns>
        internal static int Drain(Span<byte> buffer, Span<HostEvent> into)
        {
            int write = Volatile.Read(ref AsInt(buffer, WriteIndexOffset));
            int read = BinaryPrimitives.ReadInt32LittleEndian(
                buffer[ReadIndexOffset..]);

            int available = write - read;
            int count = available < into.Length ? available : into.Length;
            for (int index = 0; index < count; index++)
            {
                ReadOnlySpan<byte> record =
                    buffer.Slice(SlotOffset(read + index), RecordBytes);
                into[index] = new HostEvent(
                    (HostEventKind)BinaryPrimitives.ReadInt32LittleEndian(record),
                    BinaryPrimitives.ReadInt32LittleEndian(record[4..]),
                    BinaryPrimitives.ReadInt32LittleEndian(record[8..]),
                    BinaryPrimitives.ReadInt32LittleEndian(record[12..]),
                    BinaryPrimitives.ReadInt32LittleEndian(record[16..]),
                    BinaryPrimitives.ReadInt32LittleEndian(record[20..]));
            }

            Volatile.Write(ref AsInt(buffer, ReadIndexOffset), read + count);
            return count;
        }

        /// <summary>How many records the writer has had to throw away.</summary>
        internal static int DroppedCount(ReadOnlySpan<byte> buffer)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(buffer[DroppedOffset..]);
        }

        private static int SlotOffset(int index)
        {
            return HeaderBytes + (((int)((uint)index & (Capacity - 1))) * RecordBytes);
        }

        private static ref int AsInt(Span<byte> buffer, int offset)
        {
            return ref MemoryMarshal
                .Cast<byte, int>(buffer.Slice(offset, 4))[0];
        }
    }
}
