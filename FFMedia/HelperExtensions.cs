using System.Text;

namespace FFMedia;

internal static class HelperExtensions
{





    /// <summary>
    /// Enqueues a codec flush packet to signal the start of a packet sequence.
    /// </summary>
    /// <returns>The result of the <see cref="Enqueue(FFPacket)"/> operation.</returns>
    public static bool EnqueueFlush(this PacketStore store) =>
        store.Enqueue(FFPacket.CreateFlushPacket());

    /// <summary>
    /// Enqueues a null packet to signal the end of a packet sequence.
    /// </summary>
    /// <param name="streamIndex">The stream index to which this packet belongs.</param>
    /// <returns>The result of the <see cref="Enqueue(FFPacket)"/> operation.</returns>
    public static bool EnqueueNull(this PacketStore store, int streamIndex) =>
        store.Enqueue(FFPacket.CreateNullPacket(streamIndex));

    public static bool IsValidTimestamp(this long ts) => ts != ffmpeg.AV_NOPTS_VALUE;

    /// <summary>
    /// Converts a <see cref="ffmpeg.AV_NOPTS_VALUE"/> to a null value.
    /// </summary>
    /// <param name="r">The number.</param>
    /// <returns>The nullable number.</returns>
    public static long? ToNullable(this long r) => r == ffmpeg.AV_NOPTS_VALUE ? null : r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string? ReadString(this nint address)
    {
        if (address == default)
            return default;

        var source = (byte*)address.ToPointer();
        var length = 0;
        while (source[length] != 0)
            ++length;

        if (length == 0)
            return string.Empty;

        var byteSpan = new Span<byte>(source, length);
        return Encoding.UTF8.GetString(byteSpan);
    }

}
