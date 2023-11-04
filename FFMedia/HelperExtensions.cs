namespace FFMedia;

internal static class HelperExtensions
{



    public static bool IsValidTimestamp(this long ts) => ts != ffmpeg.AV_NOPTS_VALUE;

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

    /// <summary>
    /// Converts a 0/1 rational number to a null value.
    /// </summary>
    /// <param name="r">The rational number.</param>
    /// <returns>The nullable number</returns>
    public static AVRational? ToNullable(this AVRational r) =>
        r.num == 0 && r.den == 1 ? null : r;

    /// <summary>
    /// Converts a null rational number to a 0/1 value.
    /// </summary>
    /// <param name="r">The rational number.</param>
    /// <returns>The non-nullable number</returns>
    public static AVRational FromNullable(this AVRational? r) =>
        r.GetValueOrDefault(new() { num = 0, den = 1});

    /// <summary>
    /// Converts a <see cref="ffmpeg.AV_NOPTS_VALUE"/> to a null value.
    /// </summary>
    /// <param name="r">The number.</param>
    /// <returns>The nullable number.</returns>
    public static long? ToNullable(this long r) =>
        r == ffmpeg.AV_NOPTS_VALUE ? null : r;

    /// <summary>
    /// Converts a null value to a <see cref="ffmpeg.AV_NOPTS_VALUE"/>.
    /// </summary>
    /// <param name="r">The number.</param>
    /// <returns>The non-nullable number.</returns>
    public static long FromNullable(this long? r) =>
        r.GetValueOrDefault(ffmpeg.AV_NOPTS_VALUE);
}
