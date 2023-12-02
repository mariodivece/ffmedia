namespace FFMedia.Components;

/// <summary>
/// Defines members for pooling media frames.
/// Implementors are responsible for allocation and cleanup of
/// media frames. There must not be any currently leased frames
/// by the pool at the time when <see cref="IDisposable.Dispose"/> is
/// called as such call is intended to release all reasources of allocated frames.
/// </summary>
/// <typeparam name="TMedia"></typeparam>
public interface IFramePool<TMedia> :
    IDisposable
    where TMedia: class, IMediaFrame
{
    /// <summary>
    /// Tries to acquire a frame lease for writing to it.
    /// User is free to reallocate, read and write the frame
    /// as long as the reference is kept the same and such reference is
    /// always returned using <see cref="ReturnFrameLease(TMedia)"/>.
    /// </summary>
    /// <param name="frame">The leased frame.</param>
    /// <returns>True if the operation succeeds. False otherwise.</returns>
    bool TryAcquireFrameLease([MaybeNullWhen(false)] out TMedia? frame);

    /// <summary>
    /// Returns a frame reference that was previously leased.
    /// </summary>
    /// <param name="frame">The frame reference to return to the poool.</param>
    void ReturnFrameLease(TMedia frame);
}
