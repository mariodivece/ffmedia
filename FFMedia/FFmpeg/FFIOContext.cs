namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for <see cref="AVIOContext"/>.
/// </summary>
public unsafe sealed class FFIOContext : 
    NativeReferenceBase<AVIOContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFIOContext"/> class.
    /// </summary>
    /// <param name="target">The target data structure to wrap.</param>
    public FFIOContext(AVIOContext* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>
    /// Represents an IO context with no target.
    /// </summary>
    public static FFIOContext Empty { get; } = new FFIOContext(null);

    /// <summary>
    /// Gets the current error code. Returns 0 if no error is detected.
    /// </summary>
    public int Error => Target is null ? 0 : Target->error;

    /// <summary>
    /// Gets the current byte position being read.
    /// Returns an error code (negative) for failure.
    /// </summary>
    public long BytePosition => Target is null
        ? ffmpeg.AVERROR(ffmpeg.EINVAL) 
        : ffmpeg.avio_tell(Target);

    /// <summary>
    /// Gets the file size in bytes.
    /// Returns an error code (negative) for failure.
    /// </summary>
    public long Size => Target is null
        ? ffmpeg.AVERROR(ffmpeg.EINVAL)
        : ffmpeg.avio_size(Target);


    /// <summary>
    /// Checks if the stream is at the end of the file.
    /// </summary>
    /// <returns>True if the stream has reached an EOF condition. False otherwise.</returns>
    public bool CheckEndOfStream() =>
        Target is not null && ffmpeg.avio_feof(Target) != 0;

    /// <summary>
    /// Gets or sets whether the end of stream has been reached.
    /// </summary>
    public bool EndOfStream
    {
        get => Target is not null && Target->eof_reached != 0;
        set
        {
            if (Target is null)
                return;

            Target->eof_reached = (value) ? 1 : 0;
        }
    }
}
