namespace FFmpeg;

/// <summary>
/// Represents a wrapper for the <see cref="AVFrame"/> structure.
/// </summary>
public unsafe abstract class FFFrameBase :
    NativeTrackedReferenceBase<AVFrame>,
    INativeFrame
{
    /// <summary>
    /// Initializes and allocates a new instance of the <see cref="FFFrameBase"/> class.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    protected FFFrameBase(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(ffmpeg.av_frame_alloc(), filePath, lineNumber)
    {
        // placeholder
    }

    /// <summary>
    /// Creates an instance of the <see cref="FFFrameBase"/> class, from an already allocated
    /// <see cref="AVFrame"/>
    /// </summary>
    /// <param name="target">The allocated frame pointer.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    protected FFFrameBase(AVFrame* target,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(target, filePath, lineNumber)
    {
        // placeholder
    }

    /// <inheritdoc />
    public abstract AVMediaType MediaType { get; }

    /// <inheritdoc />
    public long PacketPosition => Target is not null ? Target->pkt_pos : default;

    /// <inheritdoc />
    public byte_ptr8 Data => Target is not null ? Target->data : default;

    /// <inheritdoc />
    public long? PtsUnits => Target is not null ? Target->pts.ToNullable() : default;

    /// <inheritdoc />
    public long? PacketDtsUnits => Target is not null ? Target->pkt_dts.ToNullable() : default;

    /// <inheritdoc />
    public long? BestEffortPtsUnits => Target is not null ? Target->best_effort_timestamp.ToNullable() : default;

    /// <inheritdoc />
    public byte** ExtendedData => Target is not null ? Target->extended_data : default;

    /// <summary>
    /// Unreference all the buffers referenced by frame and reset the frame fields.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_frame_unref(AVFrame*)"/>.</remarks>
    public void Reset()
    {
        if (Target is null)
            return;

        ffmpeg.av_frame_unref(Target);
    }

    /// <summary>
    /// Move everything contained in this frame to a destination frame and reset this frame.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_frame_move_ref"/>.</remarks>
    /// <param name="destination">The destination frame.</param>
    public void MoveTo<T>(T destination)
        where T : INativeReference<AVFrame>
    {
        if (Target is null)
            throw new InvalidOperationException("Current frame has to point to a non-zero address.");

        ArgumentNullException.ThrowIfNull(destination);
        if (destination.IsNull) throw new ArgumentNullException(nameof(destination));

        ffmpeg.av_frame_move_ref(destination.Target, Target);
    }

    /// <inheritdoc />
    protected override void ReleaseInternal(AVFrame* target) =>
        ffmpeg.av_frame_free(&target);
}
