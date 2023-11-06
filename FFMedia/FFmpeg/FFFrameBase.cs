namespace FFmpeg;

/// <summary>
/// Represents a wrapper for the <see cref="AVFrame"/> structure.
/// </summary>
public unsafe abstract class FFFrameBase :
    NativeTrackedReferenceBase<AVFrame>
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

    /// <summary>
    /// Gets the reordered position from the last <see cref="AVPacket"/>
    /// that has been input into the decoder.
    /// </summary>
    public long PacketPosition => Target->pkt_pos;

    /// <summary>Gets
    /// Gets a pointer to the picture/channel planes. This might be different from the first allocated byte.
    /// For video, it could even point to the end of the image data.
    /// </summary>
    public byte_ptr8 Data => Target->data;

    /// <summary>
    /// Gets the Presentation timestamp in
    /// time base units (time when frame should be shown to user).
    /// </summary>
    public long? Pts => Target->pts.ToNullable();

    /// <summary>
    /// Gets the DTS copied from the AVPacket that triggered returning this frame. (if frame threading isn't used)
    /// This is also the Presentation time of this <see cref="AVFrame"/> calculated from only <see cref="AVPacket.dts"/>
    /// values without pts values.
    /// </summary>
    public long? PacketDts => Target->pkt_dts.ToNullable();

    /// <summary>
    /// Gets the frame timestamp estimated using various heuristics, in stream time base units.
    /// </summary>
    public long? BestEffortPts => Target->best_effort_timestamp.ToNullable();

    /// <summary>Gets the pointers to the data planes/channels.</summary>
    public byte** ExtendedData => Target->extended_data;

    /// <summary>
    /// Unreference all the buffers referenced by frame and reset the frame fields.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_frame_unref(AVFrame*)"/>.</remarks>
    public void Reset() => ffmpeg.av_frame_unref(Target);

    /// <summary>
    /// Move everything contained in this frame to a destination frame and reset this frame.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_frame_move_ref"/>.</remarks>
    /// <param name="destination">The destination frame.</param>
    public void MoveTo<T>(T destination)
        where T : FFFrameBase
    {
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        ffmpeg.av_frame_move_ref(destination.Target, Target);
    }

    /// <inheritdoc />
    protected override void ReleaseInternal(AVFrame* target) =>
        ffmpeg.av_frame_free(&target);
}
