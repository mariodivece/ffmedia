#pragma warning disable CA2000 // Dispose objects before losing scope: Not needed. Lifecycle is managed by packet store.
using FFMedia.Engine;

namespace FFMedia.Extensions;

internal static class PacketStoreExtensions
{
    /// <summary>
    /// Enqueues a codec flush packet to signal the start of a packet sequence.
    /// </summary>
    /// <param name="store">The packet store to enqueue into.</param>
    /// <returns>The result of the <see cref="PacketStore.Enqueue(FFPacket)"/> operation.</returns>
    public static bool EnqueueFlush(this PacketStore store) =>
        store.Enqueue(FFPacket.CreateFlushPacket());

    /// <summary>
    /// Enqueues a null packet to signal the end of a packet sequence.
    /// </summary>
    /// <param name="store">The packet store to enqueue into.</param>
    /// <param name="streamIndex">The stream index to which this packet belongs.</param>
    /// <returns>The result of the <see cref="PacketStore.Enqueue(FFPacket)"/> operation.</returns>
    public static bool EnqueueNull(this PacketStore store, int streamIndex) =>
        store.Enqueue(FFPacket.CreateNullPacket(streamIndex));
}

#pragma warning restore CA2000 // Dispose objects before losing scope