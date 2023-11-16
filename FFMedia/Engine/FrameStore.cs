namespace FFMedia.Engine;


/// <summary>
/// Represents a queue-style data structure used to read and write media frames.
/// Internally, it is implemented with 2 queues. One with readable frames and
/// one with writable frames. Writable frames are leased, and then equeued on to
/// the readable frame queue. Readable frames are peeeked and then dequeued and back
/// on to the writable frames queue.
/// </summary>
public partial class FrameStore<TMedia> :
    IDisposable,
    ISerialGroupable
    where TMedia : class, IMediaFrame
{
    private readonly ExclusiveLock SyncLocker = new();
    private readonly FrameGraph Frames;

    private long m_IsDisposed;
    private IDisposable? CurrentLock;

    /// <summary>
    /// Creates a new instance of the <see cref="FrameStore{TMedia}"/> class.
    /// </summary>
    /// <param name="component">The associated component.</param>
    /// <param name="capacity">The maximum capacity.</param>
    public FrameStore(IMediaComponent<TMedia> component, int capacity)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        Frames = new(this, capacity);
        Capacity = capacity;
        Component = component;
    }

    /// <summary>
    /// Gets the maximum capacity of this <see cref="FrameStore{TMedia}"/>.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the associated <see cref="IMediaComponent{TMedia}"/>.
    /// </summary>
    public IMediaComponent<TMedia> Component { get; }

    /// <inheritdoc />
    public int GroupIndex => Component.GroupIndex;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Dispose()"/> method has been called.
    /// </summary>
    private bool IsDisposed => Interlocked.Read(ref m_IsDisposed) != 0;

    public IFrameGraph<TMedia>? Lock()
    {
        while (!IsDisposed)
        {
            var locker = SyncLocker.TryLock();
            if (locker is not null)
            {
                CurrentLock = locker;
                return PrepareFrameGraph();
            }
        }

        return null;
    }

    private FrameGraph PrepareFrameGraph()
    {
        // TODO: If implemented by Frame store, write the pending frames
        // TODO: clear frames not belonging to the group index
        // TODO: Use a frame pool to return the frame
        // TODO: Expose Frame graph interface members
        Frames.GroupIndex = GroupIndex;
        return Frames;
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool alsoManaged)
    {
        if (Interlocked.Increment(ref m_IsDisposed) > 1 || !alsoManaged)
            return;

        CurrentLock = SyncLocker.Lock();
        CurrentLock?.Dispose();
        SyncLocker.Dispose();
        CurrentLock = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}
