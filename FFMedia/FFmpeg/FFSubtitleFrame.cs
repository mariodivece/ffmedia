namespace FFmpeg;

public unsafe class FFSubtitleFrame :
    NativeTrackedReferenceBase<AVSubtitle>,
    INativeFrame
{
    public FFSubtitleFrame([CallerFilePath] string? filePath = default, [CallerLineNumber] int? lineNumber = default)
    : base(filePath, lineNumber)
    {
        Update((AVSubtitle*)ffmpeg.av_mallocz((ulong)sizeof(AVSubtitle)));
    }

    #region INativeFrame (Hide most of the implementation because it is useless)

    /// <summary>
    /// Gets the packet pts, in <see cref="ffmpeg.AV_TIME_BASE" />
    /// </summary>
    public long? Pts => Target->pts.ToNullable();

    /// <summary>
    /// Gets the packet pts, in <see cref="ffmpeg.AV_TIME_BASE" />
    /// </summary>
    public long? BestEffortPts => Target->pts.ToNullable();

#pragma warning disable CA1033

    long INativeFrame.PacketPosition => -1;

    byte_ptr8 INativeFrame.Data => default;

    long? INativeFrame.PacketDts => Target->pts.ToNullable();

    byte** INativeFrame.ExtendedData => null;

#pragma warning restore CA1033

    #endregion

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVSubtitle* target) =>
        ffmpeg.avsubtitle_free(target);
}
