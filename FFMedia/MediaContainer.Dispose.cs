namespace FFMedia;

public partial class MediaContainer : IDisposable
{
    private long _IsDisposed;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Dispose()"/>
    /// method has already been called.
    /// </summary>
    public bool IsDsiposed => Interlocked.Read(ref _IsDisposed) > 0;

    /// <summary>
    /// Overridable implementation of dispose logic.
    /// </summary>
    /// <param name="alsoManaged"></param>
    protected virtual void Dispose(bool alsoManaged)
    {
        if (Interlocked.Increment(ref _IsDisposed) > 1)
            return;

        if (alsoManaged)
        {
            // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MediaContainer()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}
