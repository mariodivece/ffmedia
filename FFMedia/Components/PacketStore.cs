﻿using System.Threading.Channels;

namespace FFMedia.Components;

/// <summary>
/// Represents a queue-style data structure that stores
/// stream packets that belong to a specific component.
/// This class is backed by a <see cref="Channel{T}"/> and it is
/// thread-safe.
/// </summary>
public sealed class PacketStore : ISerialGroupable, IDisposable
{
    private readonly Channel<FFPacket> PacketChannel;
    private long isDisposed;
    private long m_IsClosed = 1; // starts in a blocked state
    private int m_ByteSize;
    private long m_DurationUnits;
    private int m_GroupIndex;

    /// <summary>
    /// Creates a new instance of the <see cref="PacketStore"/> class.
    /// </summary>
    public PacketStore()
    {
        PacketChannel = Channel.CreateUnbounded<FFPacket>(new()
        {
            SingleWriter = false,
            SingleReader = false,
            AllowSynchronousContinuations = true,
        });
    }

    /// <summary>
    /// Gets whether the packet queue is closed.
    /// When closed, packets cannot be queued or dequeued.
    /// </summary>
    public bool IsClosed
    {
        get => Interlocked.Read(ref m_IsClosed) > 0;
        private set => Interlocked.Exchange(ref m_IsClosed, value ? 1 : 0);
    }

    /// <summary>
    /// Gets the numer of packets in the queue.
    /// </summary>
    public int Count => PacketChannel.Reader.CanCount ? PacketChannel.Reader.Count : 0;

    /// <summary>
    /// Gets the total size in bytes of the packets
    /// and their content buffers.
    /// </summary>
    public int ByteSize => Interlocked.CompareExchange(ref m_ByteSize, 0, 0);

    /// <summary>
    /// Gets the packet queue duration in stream time base units.
    /// </summary>
    public long DurationUnits => Interlocked.Read(ref m_DurationUnits);

    /// <summary>
    /// Gets the group (serial) the packet queue is currently on.
    /// </summary>
    public int GroupIndex => Interlocked.CompareExchange(ref m_GroupIndex, 0, 0);

    /// <summary>
    /// Opens the queue for enqueueing and dequeueing.
    /// Automatically enqueues a special flush packet.
    /// This method has to be called in order to start enqueuing packets.
    /// </summary>
    public void Open()
    {
        if (Interlocked.Read(ref isDisposed) > 0)
            throw new ObjectDisposedException(nameof(PacketStore));

        IsClosed = false;
        this.EnqueueFlush();
    }

    /// <summary>
    /// Closes the packet queue preventing more packets from being queued.
    /// This method can only be called once.
    /// </summary>
    public void Close()
    {
        if (IsClosed)
            return;

        IsClosed = true;
        _ = PacketChannel.Writer.TryComplete();
    }

    /// <summary>
    /// Equeues a data packet. Will return false and release the passed packet
    /// if the queue is closed.
    /// </summary>
    /// <remarks>
    /// ffplay.c packet_queue_put_private
    /// </remarks>
    /// <param name="packet">The packet to enqueue.</param>
    /// <returns>True when the operation succeeds. False otherwise.</returns>
    public bool Enqueue(FFPacket packet)
    {
        if (IsClosed)
        {
            packet?.Dispose();
            return false;
        }

        if (packet is null)
            throw new ArgumentNullException(nameof(packet));

        packet.GroupIndex = packet.IsFlushPacket
            ? Interlocked.Increment(ref m_GroupIndex)
            : Interlocked.CompareExchange(ref m_GroupIndex, 0, 0);

        Interlocked.Add(ref m_ByteSize, packet.Size + packet.StructureSize);
        Interlocked.Add(ref m_DurationUnits, packet.DurationUnits);
        if (!PacketChannel.Writer.TryWrite(packet))
        {
            packet.Dispose();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tries to obtain the next available packet in the queue.
    /// Will return false if the queue is closed.
    /// </summary>
    /// <remarks>
    /// ffplay.c packet_queue_get
    /// </remarks>
    /// <param name="blockWait">When true, waits for a newly available packet.</param>
    /// <param name="packet">The dequeued packet. May contain null if no packet was dequeued.</param>
    /// <returns>True when the operation succeeds. False otherwise.</returns>
    public bool TryDequeue(bool blockWait, [MaybeNullWhen(false)] out FFPacket? packet)
    {
        packet = default;

        if (IsClosed)
            return default;

        if (blockWait)
        {
            var waitTask = PacketChannel.Reader.WaitToReadAsync(CancellationToken.None);
            if (!waitTask.IsCompleted)
                _ = waitTask.AsTask().GetAwaiter().GetResult();
        }

        if (PacketChannel.Reader.TryRead(out packet) && packet is not null)
        {
            Interlocked.Add(ref m_ByteSize, -(packet.Size + packet.StructureSize));
            Interlocked.Add(ref m_DurationUnits, -packet.DurationUnits);
            return true;
        }

        return default;
    }

    /// <summary>
    /// Clears all the packets in the queues and disposes them.
    /// </summary>
    public void Clear()
    {
        while (Count > 0)
        {
            if (PacketChannel.Reader.TryRead(out var packet))
                packet?.Dispose();
        }

        Interlocked.Exchange(ref m_ByteSize, 0);
        Interlocked.Exchange(ref m_DurationUnits, 0);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Increment(ref isDisposed) > 1)
            return;

        Close();
        Clear();
    }
}
