namespace FFmpeg;


public unsafe class FFVideoFrame : FFFrame, IPictureSpec
{
    /// <summary>
    /// Initializes and allocates a new instance of the <see cref="FFFrame"/> class.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    public FFVideoFrame(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(filePath, lineNumber)
    {
        // placeholder
    }

    public nint BufferAddress => new(Target->data[0]);

    public AVPixelFormat PixelFormat => (AVPixelFormat)Target->format;

    public int PixelWidth => Target->width;

    public int PixelHeight => Target->height;

    public int RowBytes => Target->linesize[0];

    public AVRational PixelAspectRatio => Target->sample_aspect_ratio.num == 0 && Target->sample_aspect_ratio.den == 1
        ? new() { num = 1, den = 1}
        : ffmpeg.av_d2q(ffmpeg.av_q2d(Target->sample_aspect_ratio), int.MaxValue - 1);

    public double DpiX => 96.0 * PixelAspectRatio.num;

    public double DpiY => 96.0 * PixelAspectRatio.den;
}

/// <summary>
/// Represents a wrapper for the <see cref="AVFrame"/> structure.
/// </summary>
public unsafe class FFFrame :
    NativeTrackedReferenceBase<AVFrame>
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
    /// Gets the reordered position from the last <see cref="AVPacket"/>
    /// that has been input into the decoder.
    /// </summary>
    public long PacketPosition => Target->pkt_pos;

    /// <summary>
    /// Gets the audio sample format of the frame.
    /// </summary>
    public AVSampleFormat SampleFormat => (AVSampleFormat)Target->format;

    /// <summary>
    /// Gets the name of the <see cref="SampleFormat"/>.
    /// </summary>
    public string SampleFormatName => ffmpeg.av_get_sample_fmt_name(SampleFormat);

    /// <summary>Gets
    /// Gets a pointer to the picture/channel planes. This might be different from the first allocated byte.
    /// For video, it could even point to the end of the image data.
    /// </summary>
    public byte_ptr8 Data => Target->data;


    public int SampleCount => Target->nb_samples;

    public int Channels => Target->ch_layout.nb_channels;

    public int SampleRate => Target->sample_rate;

    public double AudioComputedDuration => (double)SampleCount / SampleRate;

    public long Pts
    {
        get => Target->pts;
        set => Target->pts = value;
    }

    public long? PacketDts => Target->pkt_dts.ToNullable();

    /// <summary>
    /// Gets the frame timestamp estimated using various heuristics, in stream time base.
    /// </summary>
    public long? BestEffortPts => Target->best_effort_timestamp.ToNullable();

    public byte** ExtendedData
    {
        get => Target->extended_data;
        set => Target->extended_data = value;
    }

    public AVChannelLayout ChannelLayout =>
        Target->ch_layout;

    public int SamplesBufferSize =>
        AudioParams.ComputeSamplesBufferSize(Channels, SampleCount, SampleFormat, true);

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
    public void MoveTo(FFFrame destination) =>
        ffmpeg.av_frame_move_ref(destination!.Target, Target);

    /// <inheritdoc />
    protected override void ReleaseInternal(AVFrame* target) =>
        ffmpeg.av_frame_free(&target);
}
