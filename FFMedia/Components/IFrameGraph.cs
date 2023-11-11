namespace FFMedia.Components;

/// <summary>
/// Represents a set of media frames in its locked, read/write state.
/// </summary>
/// <typeparam name="TMedia"></typeparam>
public interface IFrameGraph<TMedia> :
    IDisposable,
    ISerialGroupable
    where TMedia : class, IMediaFrame
{
    IMediaComponent<TMedia> Component { get; }
}