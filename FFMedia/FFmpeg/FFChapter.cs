namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for <see cref="AVChapter"/>.
/// </summary>
public unsafe sealed class FFChapter : 
    NativeReferenceBase<AVChapter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFChapter"/> class.
    /// </summary>
    /// <param name="target">The data structure to wrap.</param>
    public FFChapter(AVChapter* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>
    /// Gets the unique identifier of this chapter.
    /// </summary>
    public long Id => Target is not null ? Target->id : default;

    /// <summary>
    /// Gets the metadata for this chapter.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata => Target is not null
        ? FFDictionary.ToDictionary(Target->metadata)
        : Constants.EmptyDictionary;

    /// <summary>
    /// Gets the start time as determined by <see cref="StartTimeUnits"/> and <see cref="TimeBase"/>.
    /// </summary>
    public TimeExtent StartTime => StartTimeUnits.ToSeconds(TimeBase);

    /// <summary>
    /// Gets the end time as determined by <see cref="StartTimeUnits"/> and <see cref="TimeBase"/>.
    /// </summary>
    public TimeExtent EndTime => EndTimeUnits.ToSeconds(TimeBase);

    /// <summary>
    /// Gets the duration of this chapter based on <see cref="EndTime"/> and <see cref="StartTime"/>.
    /// </summary>
    public TimeExtent Duration => EndTime - StartTime;

    /// <summary>
    /// Gets the start time in <see cref="TimeBase"/> units.
    /// </summary>
    public long StartTimeUnits => Target is not null ? Target->start : ffmpeg.AV_NOPTS_VALUE;

    /// <summary>
    /// Gets the end time in <see cref="TimeBase"/> units.
    /// </summary>
    public long EndTimeUnits => Target is not null ? Target->end : ffmpeg.AV_NOPTS_VALUE;

    /// <summary>
    /// Gets the time base for timestamps used in this <see cref="FFChapter"/>.
    /// </summary>
    public AVRational TimeBase => Target is not null ? Target->time_base : RationalExtensions.UndefinedValue;
}
