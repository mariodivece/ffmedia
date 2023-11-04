namespace FFMedia.Primitives;

/// <summary>
/// Represents the members of a <see cref="INativeReference"/>
/// that are required for it to be trackable and disposable.
/// This is required to track and manage the lifecycle of data
/// structures residing in native memory and avoid potential
/// memory leaks.
/// </summary>
public interface INativeTrackedReference : INativeReference, IDisposable
{
    /// <summary>
    /// Gets the unique identifier that was assigned by the current process.
    /// </summary>
    ulong ObjectId { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="IDisposable.Dispose"/>
    /// method has already been called.
    /// </summary>
    bool IsDisposed { get; }
}
