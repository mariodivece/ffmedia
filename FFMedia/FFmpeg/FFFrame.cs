namespace FFmpeg;

/// <summary>
/// Represents a wrapper for the <see cref="AVFrame"/> structure.
/// </summary>
public unsafe sealed class FFFrame :
    NativeTrackedReferenceBase<AVFrame>,
    IVideoFrame,
    IWaveBufferSpec
{
    /// <summary>
    /// Initializes and allocates a new instance of the <see cref="FFFrame"/> class.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    public FFFrame(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(ffmpeg.av_frame_alloc(), filePath, lineNumber)
    {
        // placeholder
    }

    /// <summary>
    /// Creates an instance of the <see cref="FFPacket"/> class, from an already allocated
    /// <see cref="AVFrame"/>
    /// </summary>
    /// <param name="target">The allocated frame pointer.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFFrame(AVFrame* target,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(target, filePath, lineNumber)
    {
        // placeholder
    }

    #region Video Frame Spec

    nint IPictureBufferSpec.BufferAddress => new(Target->data[0]);

    /// <inheritdoc />
    AVPixelFormat IPictureSpec.PixelFormat => (AVPixelFormat)Target->format;

    /// <inheritdoc />
    int IPictureSpec.PixelWidth => Target->width;

    /// <inheritdoc />
    int IPictureSpec.PixelHeight => Target->height;

    /// <inheritdoc />
    int IPictureSpec.RowBytes => Target->linesize[0];

    /// <inheritdoc />
    AVRational IPictureSpec.PixelAspectRatio => !Target->sample_aspect_ratio.IsValid()
        ? IPictureSpec.DefaultPixelAspectRatio
        : Target->sample_aspect_ratio.Normalize();

    #endregion

    #region Video Frame

    /// <inheritdoc />
    bool IVideoFrame.IsKeyFrame => Target->key_frame != 0;

    /// <inheritdoc />
    AVPictureType IVideoFrame.PictureType => Target->pict_type;

    /// <inheritdoc />
    int IVideoFrame.CodedPictureNumber => Target->coded_picture_number;

    /// <inheritdoc />
    int IVideoFrame.DisplayPictureNumber => Target->display_picture_number;

    /// <inheritdoc />
    int IVideoFrame.RepeatCount => Target->repeat_pict;

    /// <inheritdoc />
    bool IVideoFrame.IsInterlaced => Target->interlaced_frame != 0;

    /// <inheritdoc />
    bool IVideoFrame.IsTopFieldFirst => Target->top_field_first != 0;

    #endregion

    #region Audio Frame Spec

    /// <inheritdoc />
    AVSampleFormat IWaveSpec.SampleFormat => (AVSampleFormat)Target->format;

    /// <inheritdoc />
    int IWaveSpec.ChannelCount => Target->ch_layout.nb_channels;

    /// <inheritdoc />
    int IWaveSpec.SampleRate => Target->sample_rate;

    /// <inheritdoc />
    nint IWaveBufferSpec.BufferAddress => new(Target->extended_data[0]);

    /// <inheritdoc />
    int IWaveBufferSpec.SampleCount => Target->nb_samples;

    #endregion

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
    public void MoveTo(FFFrame destination)
    {
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        ffmpeg.av_frame_move_ref(destination.Target, Target);
    }

    /// <summary>
    /// Makes a newly allocated copy of this frame that references the same <see cref="Data"/>.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_frame_clone"/>.</remarks>
    /// <returns>The cloned frame.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FFFrame Clone(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default) =>
        new(ffmpeg.av_frame_clone(Target), filePath, lineNumber);

    /// <inheritdoc />
    protected override void ReleaseInternal(AVFrame* target) =>
        ffmpeg.av_frame_free(&target);
}
