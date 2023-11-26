namespace FFMedia.FFmpeg;

public unsafe class FFCodecContext :
    NativeTrackedReferenceBase<AVCodecContext>,
    IFFOptionsEnabled
{
    public FFCodecContext(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(filePath, lineNumber)
    {
        Update(ffmpeg.avcodec_alloc_context3(codec: null));
    }

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
    public FFMediaClass MediaClass => Options.GetMediaClass();

    /// <inheritdoc />
    public FFOptionsWrapper Options => FFOptionsWrapper.TryWrap(this, out var options) ? options : FFOptionsWrapper.Empty;

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVCodecContext* target)
    {
        ffmpeg.avcodec_free_context(&target);
    }
}
