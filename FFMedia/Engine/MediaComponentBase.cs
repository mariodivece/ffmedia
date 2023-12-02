
namespace FFMedia.Engine;

/// <summary>
/// PRovides a base implementation of a media component.
/// </summary>
/// <typeparam name="TMedia">The media frame type.</typeparam>
public abstract class MediaComponentBase<TMedia> : IMediaComponent<TMedia>
    where TMedia : class, IMediaFrame
{
    private long m_IsDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaComponentBase{TMedia}"/> class.
    /// </summary>
    /// <param name="container">The associated container.</param>
    /// <param name="framePool">The frame pool service.</param>
    protected MediaComponentBase(MediaContainer container, IFramePool<TMedia> framePool)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(framePool);

        Container = container;
        FramePool = framePool;
        Options = container.Options;

        MediaType = typeof(TMedia).IsAssignableTo(typeof(IVideoFrame))
            ? AVMediaType.AVMEDIA_TYPE_VIDEO
            : typeof(TMedia).IsAssignableTo(typeof(IAudioFrame))
            ? AVMediaType.AVMEDIA_TYPE_AUDIO
            : typeof(TMedia).IsAssignableTo(typeof(ISubtitleFrame))
            ? AVMediaType.AVMEDIA_TYPE_SUBTITLE
            : AVMediaType.AVMEDIA_TYPE_UNKNOWN;

        var capacity = MediaType switch
        {
            AVMediaType.AVMEDIA_TYPE_VIDEO => Options.VideoFramesCapacity,
            AVMediaType.AVMEDIA_TYPE_AUDIO => Options.AudioFramesCapacity,
            AVMediaType.AVMEDIA_TYPE_SUBTITLE => Options.SubtitleFramesCapacity,
            _ => throw new NotImplementedException()
        };

        Frames = new(this, Math.Min(capacity, Options.FramesMaxCapacity));
        Packets = new();
    }

    protected MediaContainerOptions Options { get; }

    /// <inheritdoc />
    public virtual MediaContainer Container { get; }

    /// <inheritdoc />
    public virtual AVMediaType MediaType { get; }

    /// <inheritdoc />
    public virtual PacketStore Packets { get; }

    /// <inheritdoc />
    public virtual FrameStore<TMedia> Frames { get; }

    /// <inheritdoc />
    public virtual int GroupIndex => Packets.GroupIndex;

    /// <inheritdoc />
    public IFramePool<TMedia> FramePool { get; }

    /// <summary>
    /// Releases unmanaged and optionaly unmanaged resources.
    /// </summary>
    /// <param name="alsoManaged">Includes managed resources.</param>
    protected virtual void Dispose(bool alsoManaged)
    {
        if (Interlocked.Increment(ref m_IsDisposed) > 1 || !alsoManaged)
            return;

        Frames.Dispose();
        Packets.Dispose();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}
