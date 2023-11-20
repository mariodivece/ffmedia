namespace FFmpeg;

/// <summary>
/// Provides a wrapper for the <see cref="AVClass"/> structure.
/// </summary>
/// <param name="target">The target to wrap.</param>
public unsafe sealed class FFMediaClass(AVClass* target) :
    NativeReferenceBase<AVClass>(target)
{
    /// <summary>
    /// Gets the <see cref="AVClass"/> for <see cref="AVCodecContext"/>.
    /// It can be used in combination with <see cref="ffmpeg.AV_OPT_SEARCH_FAKE_OBJ"/>
    /// for examining options.
    /// </summary>
    public static FFMediaClass Codec { get; } = new(ffmpeg.avcodec_get_class());

    /// <summary>
    /// Gets the <see cref="AVClass"/> for <see cref="AVFormatContext"/>.
    /// It can be used in combination with <see cref="ffmpeg.AV_OPT_SEARCH_FAKE_OBJ"/>
    /// for examining options.
    /// </summary>
    public static FFMediaClass Format { get; } = new(ffmpeg.avformat_get_class());

    /// <summary>
    /// Gets the <see cref="AVClass"/> for <see cref="SwsContext"/>.
    /// It can be used in combination with <see cref="ffmpeg.AV_OPT_SEARCH_FAKE_OBJ"/>
    /// for examining options.
    /// </summary>
    public static FFMediaClass Scaler { get; } = new(ffmpeg.sws_get_class());

    /// <summary>
    /// Gets the <see cref="AVClass"/> for <see cref="SwrContext"/>.
    /// It can be used in combination with <see cref="ffmpeg.AV_OPT_SEARCH_FAKE_OBJ"/>
    /// for examining options.
    /// </summary>
    public static FFMediaClass Resampler { get; } = new(ffmpeg.swr_get_class());


    public FFOption? FindOption(string optionName, bool searchChildren)
    {
        if (Target is null)
            return default;

        var flags = ffmpeg.AV_OPT_SEARCH_FAKE_OBJ | (searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0);
        var t = Target;
        var option = ffmpeg.av_opt_find(&t, optionName, null, 0, flags);
        return option is not null && option->flags != (int)AVOptionType.AV_OPT_TYPE_FLAGS
            ? new(option)
            : default;
    }

    public bool HasOption(string optionName, int optionFlags = default, int searchFlags = ffmpeg.AV_OPT_SEARCH_FAKE_OBJ) =>
        FindOption(optionName, optionFlags, searchFlags) is not null;

}
