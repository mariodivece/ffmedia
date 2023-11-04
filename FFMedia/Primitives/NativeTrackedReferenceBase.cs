namespace FFMedia.Primitives;

/// <summary>
/// Serves as a base implementation for a <see cref="INativeTrackedReference"/>.
/// </summary>
/// <typeparam name="T">The unmanaged data structure type that this class wraps.</typeparam>
public abstract unsafe class NativeTrackedReferenceBase<T> : NativeReferenceBase<T>, INativeTrackedReference
    where T : unmanaged
{
    private long m_IsDsiposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeTrackedReferenceBase{T}"/> class.
    /// Automatically assigns a <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="filePath">Use sttribute [CallerFilePath] in derived class to track the source.</param>
    /// <param name="lineNumber">Use attribute [CallerLineNumber] in dervice classes to track the source.</param>
    protected NativeTrackedReferenceBase(string? filePath, int? lineNumber)
        : base()
    {
        filePath ??= "(No file)";
        lineNumber ??= 0;
        var source = $"{Path.GetFileName(filePath)}: {lineNumber}";
        ObjectId = NativeReferenceTracker.Instance.Add(this, source);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeTrackedReferenceBase{T}"/> class.
    /// Automatically assigns a <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="filePath">Use sttribute [CallerFilePath] in derived class to track the source.</param>
    /// <param name="lineNumber">Use attribute [CallerLineNumber] in dervice classes to track the source.</param>
    protected NativeTrackedReferenceBase(T* target, string? filePath, int? lineNumber)
        : this(filePath, lineNumber)
    {
        Update(target);
    }

    /// <inheritdoc />
    public ulong ObjectId { get; protected set; }

    /// <inheritdoc />
    public bool IsDisposed => Interlocked.Read(ref m_IsDsiposed) != 0;

    /// <summary>
    /// Implements the necessary steps to free the memory when
    /// <see cref="IDisposable.Dispose"/> is called.
    /// There is no need to call <see cref="INativeReference.Update(nint)"/>.
    /// or overrid the <see cref="Dispose(bool)"/> method.
    /// </summary>
    /// <param name="target">The pointer addess to the data structure.</param>
    protected abstract void ReleaseInternal(T* target);

    /// <summary>
    /// Releases unmanaged and optionally managed resources held by this instance.
    /// It is recommended that you implement <see cref="ReleaseInternal(T*)"/>
    /// instead of overriding this method.
    /// </summary>
    /// <param name="alsoManaged"></param>
    protected virtual void Dispose(bool alsoManaged)
    {
        if (Interlocked.Increment(ref m_IsDsiposed) > 1)
            return;

        if (!alsoManaged)
            return;

        if (!IsEmpty)
            ReleaseInternal(Target);

        Update(IntPtr.Zero);
        NativeReferenceTracker.Instance.Remove(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}