namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for a <see cref="AVSubtitle"/>
/// </summary>
public unsafe class FFSubtitleFrame :
    NativeTrackedReferenceBase<AVSubtitle>,
    INativeFrame
{
    

    /// <summary>
    /// Initializes a new instance of the <see cref="FFSubtitleFrame"/> class.
    /// Allocates a new <see cref="AVFrame"/>.
    /// </summary>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFSubtitleFrame([CallerFilePath] string? filePath = default, [CallerLineNumber] int? lineNumber = default)
    : base(filePath, lineNumber)
    {
        Update((AVSubtitle*)ffmpeg.av_mallocz((ulong)sizeof(AVSubtitle)));
    }

    /// <inheritdoc />
    public AVMediaType MediaType => AVMediaType.AVMEDIA_TYPE_SUBTITLE;

    /// <summary>
    /// Gets the subtitle start display time.
    /// </summary>
    public TimeExtent StartDisplayTime => ((long)Target->start_display_time).ToSeconds(Constants.SubtitlesTimeBase);

    /// <summary>
    /// Gets the subtitle end display time.
    /// </summary>
    public TimeExtent EndDisplayTime => ((long)Target->end_display_time).ToSeconds(Constants.SubtitlesTimeBase);

    /// <summary>
    /// Gest the subtitle display duration, derived from <see cref="StartDisplayTime"/> and
    /// <see cref="EndDisplayTime"/>.
    /// </summary>
    public TimeExtent DisplayDuration => EndDisplayTime - StartDisplayTime;

    /// <summary>
    /// Gets a list of rects contained by the subtitle frame.
    /// </summary>
    public IReadOnlyList<FFSubtitleRect> Rects
    {
        get
        {
            if (IsNull || Target->num_rects <= 0)
                return Array.Empty<FFSubtitleRect>();

            var result = new List<FFSubtitleRect>();
            for (var i = 0; i < Target->num_rects; i++)
                result.Add(new(Target->rects[i]));

            return result;
        }
    }

    #region INativeFrame (Hide most of the implementation because it is useless)

    /// <inheritdoc />
    public long? Pts => Target->pts.ToNullable();

    /// <inheritdoc />
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
