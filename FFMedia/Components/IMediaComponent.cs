namespace FFMedia.Components;

/// <summary>
/// Provides member definitions for a media component of the specified
/// media type.
/// </summary>
/// <typeparam name="TMedia">The component's media type.</typeparam>
public interface IMediaComponent<TMedia> :
    ISerialGroupable,
    IDisposable
    where TMedia : class, IMediaFrame
{
    /// <summary>
    /// Provides access to the media container that owns this <see cref="IMediaComponent{TMedia}"/>
    /// </summary>
    MediaContainer Container { get; }

    /// <summary>
    /// Gets the media type of this component.
    /// </summary>
    AVMediaType MediaType { get; }

    /// <summary>
    /// Provides access to the underlying <see cref="PacketStore"/>.
    /// </summary>
    PacketStore Packets { get; }

    /// <summary>
    /// Provides access to the underlying <see cref="FrameStore{TMedia}"/>.
    /// </summary>
    FrameStore<TMedia> Frames { get; }

    /// <summary>
    /// Provides access to the underlying <see cref="IFramePool{TMedia}"/>.
    /// </summary>
    IFramePool<TMedia> FramePool { get; }
}
