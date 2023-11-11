namespace FFMedia.Engine;

/// <summary>
/// PRovides a base implementation of a media component.
/// </summary>
/// <typeparam name="TMedia">The media frame type.</typeparam>
public abstract class MediaComponentBase<TMedia> : IMediaComponent<TMedia>
    where TMedia : class, IMediaFrame
{
    /// <summary>
    /// Port of VIDEO_PICTURE_QUEUE_SIZE.
    /// </summary>
    protected const int VideoQueueCapacity = 3;

    /// <summary>
    /// Port of SAMPLE_QUEUE_SIZE.
    /// </summary>
    protected const int AudioQueueCapacity = 9;

    /// <summary>
    /// Port of SUBPICTURE_QUEUE_SIZE.
    /// </summary>
    protected const int SubtitleQueueCapacity = 16;

    /// <summary>
    /// Port of FRAME_QUEUE_SIZE.
    /// </summary>
    protected static readonly int MaxQueueCapacity = Math.Max(Math.Max(VideoQueueCapacity, AudioQueueCapacity), SubtitleQueueCapacity);

    private long m_IsDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaComponentBase{TMedia}"/> class.
    /// </summary>
    /// <param name="component">The associated component.</param>
    protected MediaComponentBase(IMediaComponent<TMedia> component)
    {
        Component = component;

        MediaType = typeof(TMedia).IsAssignableTo(typeof(IVideoFrame))
            ? AVMediaType.AVMEDIA_TYPE_VIDEO
            : typeof(TMedia).IsAssignableTo(typeof(IAudioFrame))
            ? AVMediaType.AVMEDIA_TYPE_AUDIO
            : typeof(TMedia).IsAssignableTo(typeof(ISubtitleFrame))
            ? AVMediaType.AVMEDIA_TYPE_SUBTITLE
            : AVMediaType.AVMEDIA_TYPE_UNKNOWN;

        var capacity = MediaType switch
        {
            AVMediaType.AVMEDIA_TYPE_VIDEO => VideoQueueCapacity,
            AVMediaType.AVMEDIA_TYPE_AUDIO => AudioQueueCapacity,
            AVMediaType.AVMEDIA_TYPE_SUBTITLE => SubtitleQueueCapacity,
            _ => throw new NotImplementedException()
        };

        Frames = new(this, capacity);
        Packets = new();
    }

    /// <inheritdoc />
    public virtual IMediaComponent<TMedia> Component { get; }

    /// <inheritdoc />
    public virtual AVMediaType MediaType { get; }

    /// <inheritdoc />
    public virtual PacketStore Packets { get; }

    /// <inheritdoc />
    public virtual FrameStore<TMedia> Frames { get; }

    /// <inheritdoc />
    public virtual int GroupIndex => Packets.GroupIndex;

    /// <summary>
    /// Releases unmanaged and optionaly unmanaged resources.
    /// </summary>
    /// <param name="alsoManaged">Includes managed resources.</param>
    protected virtual void Dispose(bool alsoManaged)
    {
        if (Interlocked.Read(ref m_IsDisposed) > 1 || !alsoManaged)
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
