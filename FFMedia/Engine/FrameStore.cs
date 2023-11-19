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
    ISerialGroupable,
    IComponentService<TMedia>
    where TMedia : class, IMediaFrame
{
    private readonly ExclusiveLock SyncLocker = new();
    private readonly FrameGraph Frames;

    private IDisposable? CurrentLock;
    private long m_IsDisposed;
    
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

    /// <inheritdoc />
    public IMediaComponent<TMedia> Component { get; }

    /// <inheritdoc />
    public int GroupIndex => Component.GroupIndex;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Dispose()"/> method has been called.
    /// </summary>
    private bool IsDisposed => Interlocked.Read(ref m_IsDisposed) != 0;

    /// <summary>
    /// Locks this <see cref="FrameStore{TMedia}"/> and returns
    /// the stored frames as a <see cref="IFrameGraph{TMedia}"/>
    /// for exclusive reading and writing. This is a tight blovking operation
    /// and the only way this returns null is if <see cref="Dispose()"/>
    /// is called by someone else.
    /// </summary>
    /// <returns>The locked frame graph.</returns>
    public IFrameGraph<TMedia>? Lock()
    {
        IDisposable? currentLock;
        while (!IsDisposed)
        {
            currentLock = SyncLocker.TryLock();
            if (currentLock is not null)
            {
                CurrentLock = currentLock;
                return PrepareFrameGraph();
            }
        }

        return null;
    }

    private FrameGraph PrepareFrameGraph()
    {
        Frames.GroupIndex = GroupIndex;

        // Remove frames not belonging to the current group index
        for (var i = Frames.Count - 1; i >= 0; i--)
        {
            if (Frames[i].GroupIndex != GroupIndex)
                Frames.RemoveAt(i);
        }

        return Frames;
    }

    private void Unlock()
    {
        CurrentLock?.Dispose();
        CurrentLock = null;
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
        Frames.Clear();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}
