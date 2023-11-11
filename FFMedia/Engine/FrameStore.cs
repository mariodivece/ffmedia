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
    private readonly SortedFrameList Frames;

    private int m_Count;
    private long m_IsDisposed;
    private IDisposable? CurrentLock;

    public FrameStore(IMediaComponent<TMedia> component, int capacity)
    {
        Frames = new(capacity);
        Capacity = capacity;
        Component = component;
    }

    public int Count
    {
        get => Interlocked.CompareExchange(ref m_Count, 0, 0);
        set => Interlocked.Exchange(ref m_Count, value);
    }

    public int Capacity { get; }

    public bool IsFull => Count >= Capacity;

    public IMediaComponent<TMedia> Component { get; }

    private bool IsDisposed => Interlocked.Read(ref m_IsDisposed) != 0;

    public int GroupIndex => Component.GroupIndex;

    public IFrameGraph<TMedia>? Lock(int timeoutMilliseconds = Timeout.Infinite)
    {
        if (IsDisposed)
            return null;

        CurrentLock = SyncLocker.Lock(timeoutMilliseconds);
        return CurrentLock is not null ? new FrameGraph(this) : null;
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

    private class FrameSorter : IComparer<TMedia>
    {
        public int Compare(TMedia? x, TMedia? y)
        {
            var tx = x?.StartTime ?? TimeExtent.NaN;
            var ty = y?.StartTime ?? TimeExtent.NaN;
            return tx.CompareTo(ty);
        }
    }
}
