namespace FFMedia;

internal static class HelperExtensions
{
    public static bool IsValidTimestamp(this long ts) => ts != ffmpeg.AV_NOPTS_VALUE;

    public static void DelayOnce() => Thread.Sleep(1);

    /// <summary>
    /// Enqueues a codec flush packet to signal the start of a packet sequence.
    /// </summary>
    /// <returns>The result of the <see cref="Enqueue(FFPacket)"/> operation.</returns>
    public static bool EnqueueFlush(PacketStore store) =>
        store.Enqueue(FFPacket.CreateFlushPacket());

    /// <summary>
    /// Enqueues a null packet to signal the end of a packet sequence.
    /// </summary>
    /// <param name="streamIndex">The stream index to which this packet belongs.</param>
    /// <returns>The result of the <see cref="Enqueue(FFPacket)"/> operation.</returns>
    public static bool EnqueueNull(PacketStore store, int streamIndex) =>
        store.Enqueue(FFPacket.CreateNullPacket(streamIndex));
}
