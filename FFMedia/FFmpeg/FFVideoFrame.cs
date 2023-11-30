namespace FFmpeg;

/// <summary>
/// Provides a wrapper for a <see cref="AVFrame"/> of video.
/// </summary>
public unsafe class FFVideoFrame :
    FFFrameBase,
    IPictureBufferSpec
{
    /// <summary>
    /// Initializes and allocates a new instance of the <see cref="FFVideoFrame"/> class.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFVideoFrame(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(ffmpeg.av_frame_alloc(), filePath, lineNumber)
    {
        // placeholder
    }

    /// <summary>
    /// Creates an instance of the <see cref="FFVideoFrame"/> class, from an already allocated
    /// <see cref="AVFrame"/>
    /// </summary>
    /// <param name="target">The allocated frame pointer.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFVideoFrame(AVFrame* target,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(target, filePath, lineNumber)
    {
        // placeholder
    }

    /// <inheritdoc />
    public override AVMediaType MediaType => AVMediaType.AVMEDIA_TYPE_VIDEO;

    /// <inheritdoc />
    public nint BufferAddress => new(Target->data[0]);

    /// <inheritdoc />
    public AVPixelFormat PixelFormat => ((AVPixelFormat)Target->format).Normalize();

    /// <inheritdoc />
    public int PixelWidth => Target->width;

    /// <inheritdoc />
    public int PixelHeight => Target->height;

    /// <inheritdoc />
    public int RowBytes => Target->linesize[0];

    /// <inheritdoc />
    public int? BytesPerPixel => PixelFormat.BytesPerPixel();

    /// <inheritdoc />
    public double DpiX => IVideoFrame.DefaultDpiX;

    /// <inheritdoc />
    public double DpiY => IVideoFrame.DefaultDpiY;

    /// <inheritdoc />
    public AVRational PixelAspectRatio => !Target->sample_aspect_ratio.HasValue()
        ? IVideoFrame.DefaultPixelAspectRatio
        : Target->sample_aspect_ratio.Normalize();

    /// <summary>
    /// Gets a value indicating whether this frame is a keyframe.
    /// </summary>
    public bool IsKeyFrame => Target->key_frame.ToBool();

    /// <summary>
    /// Gets the picture type.
    /// </summary>
    public AVPictureType PictureType => Target->pict_type;

    /// <summary>
    /// Gets the picture number im bitstream order.
    /// </summary>
    public int CodedPictureNumber => Target->coded_picture_number;

    /// <summary>
    /// Gets the picture number in display order.
    /// </summary>
    public int DisplayPictureNumber => Target->display_picture_number;

    /// <summary>
    /// Gets a value indicating how much the picture must be delayed.
    /// extra_delay = repeat_pict / (2*fps)
    /// </summary>
    public int RepeatCount => Target->repeat_pict;

    /// <summary>
    /// Gets a value indicating whether the picture is interlaced.
    /// </summary>
    public bool IsInterlaced => Target->interlaced_frame.ToBool();

    /// <summary>
    /// Gets a value indicating whether the picture is interlaced
    /// and the top frame is displayed first.
    /// </summary>
    public bool IsTopFieldFirst => IsInterlaced && Target->top_field_first.ToBool();

    /// <inheritdoc />
    public int BufferLength => RowBytes * PixelHeight;

    /// <summary>
    /// Makes a newly allocated copy of this frame that references the same <see cref="FFFrameBase.Data"/>.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_frame_clone"/>.</remarks>
    /// <returns>The cloned frame.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FFVideoFrame Clone(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default) =>
        new(ffmpeg.av_frame_clone(Target), filePath, lineNumber);
}
