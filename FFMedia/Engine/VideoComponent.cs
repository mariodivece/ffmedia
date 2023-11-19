namespace FFMedia.Engine;

/// <summary>
/// Implements a base class for a video component.
/// </summary>
/// <typeparam name="TMedia"></typeparam>
public class VideoComponentBase<TMedia>
    : MediaComponentBase<TMedia>
    where TMedia : class, IVideoFrame
{
    /// <summary>
    /// Create an instance of the <see cref="VideoComponentBase{TMedia}"/> class.
    /// </summary>
    /// <param name="container">The associated container.</param>
    /// <param name="framePool">The frame pool service.</param>
    protected VideoComponentBase(MediaContainer container, IFramePool<TMedia> framePool)
        : base(container, framePool)
    {
        // placeholder
    }
}
