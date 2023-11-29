namespace FFmpeg;

/// <summary>
/// Represents a wrapper for <see cref="AVCodecContext"/>.
/// </summary>
public unsafe class FFCodecContext :
    NativeTrackedReferenceBase<AVCodecContext>,
    IFFOptionsEnabled
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFCodecContext"/> class
    /// without any particular codec.
    /// </summary>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFCodecContext(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(filePath, lineNumber)
    {
        Update(ffmpeg.avcodec_alloc_context3(codec: null));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FFCodecContext"/> class
    /// with a specific codec.
    /// </summary>
    /// <param name="codec">The codec reference.</param>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFCodecContext(
        FFCodec codec,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(filePath, lineNumber)
    {
        ArgumentNullException.ThrowIfNull(codec);
        Update(ffmpeg.avcodec_alloc_context3(codec: codec.Target));
    }

    /// <inheritdoc />
    public FFMediaClass MediaClass => OptionsWrapper.MediaClass;

    /// <inheritdoc />
    public IReadOnlyList<FFOption> CurrentOptions => OptionsWrapper.CurrentOptions;

    /// <inheritdoc />
    public IReadOnlyList<IFFOptionsEnabled> CurrentChildren => OptionsWrapper.CurrentChildren;

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVCodecContext* target) =>
        ffmpeg.avcodec_free_context(&target);

    /// <summary>
    /// Gets a wrapper for implementing <see cref="IFFOptionsEnabled"/>.
    /// </summary>
    private IFFOptionsEnabled OptionsWrapper => FFOptionsStore.TryWrap(this, out var options)
        ? options
        : FFOptionsStore.Empty;
}
