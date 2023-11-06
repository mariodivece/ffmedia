namespace FFmpeg;

/// <summary>
/// Provides a wrapper for a <see cref="AVFrame"/> of video.
/// </summary>
public unsafe class FFVideoFrame :
    FFFrameBase,
    IVideoFrame
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
    public nint BufferAddress => new(Target->data[0]);

    /// <inheritdoc />
    public AVPixelFormat PixelFormat => (AVPixelFormat)Target->format;

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

    /// <inheritdoc />
    public bool IsKeyFrame => Target->key_frame.ToBool();

    /// <inheritdoc />
    public AVPictureType PictureType => Target->pict_type;

    /// <inheritdoc />
    public int CodedPictureNumber => Target->coded_picture_number;

    /// <inheritdoc />
    public int DisplayPictureNumber => Target->display_picture_number;

    /// <inheritdoc />
    public int RepeatCount => Target->repeat_pict;

    /// <inheritdoc />
    public bool IsInterlaced => Target->interlaced_frame.ToBool();

    /// <inheritdoc />
    public bool IsTopFieldFirst => Target->top_field_first.ToBool();

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
