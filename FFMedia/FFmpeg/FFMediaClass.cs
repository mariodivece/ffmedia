﻿namespace FFmpeg;

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

    /// <summary>
    /// Gets the child <see cref="FFMediaClass"/> objects.
    /// </summary>
    /// <inheritdoc />
    public IReadOnlyList<FFMediaClass> Children
    {
        get
        {
            if (Target is null)
                return Array.Empty<FFMediaClass>();

            var result = new List<FFMediaClass>();
            void* iterator = null;
            AVClass* currentChild;
            do
            {
                currentChild = ffmpeg.av_opt_child_class_iterate(Target, &iterator);

                if (currentChild is not null)
                    result.Add(new FFMediaClass(currentChild));

            } while (currentChild is not null);

            return result;
        }
    }

    /// <summary>
    /// Gets an iteration of all the options for this <see cref="FFMediaClass"/>.
    /// </summary>
    /// <inheritdoc />
    public IReadOnlyList<FFOption> Options
    {
        get
        {
            if (Target is null)
                return Array.Empty<FFOption>();

            var options = new List<FFOption>();

            // since we are searching for a 'fake' object (i.e. static struct, we need to pass a double pointer
            // per the documentation of the av_opt_find method call.
            var target = Target;

            AVOption* currentOption = null;
            do
            {
                currentOption = ffmpeg.av_opt_next(&target, currentOption);
                if (currentOption is not null && currentOption->name is not null)
                    options.Add(new(currentOption));

            } while (currentOption is not null);
            return options;
        }
    }

    /// <summary>
    /// Attempts to find an option with the specified name.
    /// </summary>
    /// <param name="optionName">The name of the option to search for.</param>
    /// <param name="searchChildren">Whether to search for the option in child objects.</param>
    /// <returns>The the option if it is found. Null otherwise.</returns>
    /// <remarks>Port of cmdutils.c/opt_find and based on libavutil/opt.c.</remarks>
    public FFOption? FindOption(string optionName, bool searchChildren)
    {
        if (Target is null)
            return default;

        var searchFlags =
            ffmpeg.AV_OPT_SEARCH_FAKE_OBJ |
            (searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0);

        // since we are searching for a 'fake' object (i.e. static struct), we need to pass a double pointer
        // per the documentation of the av_opt_find method call.
        var target = Target;
        var option = ffmpeg.av_opt_find(&target, optionName, null, 0, searchFlags);

        return option is not null
            ? new(option)
            : default;
    }

    /// <summary>
    /// Checks whether an option with the specified name exists.
    /// </summary>
    /// <param name="optionName">The name of the option to search for.</param>
    /// <param name="searchChildren">Whether to search for the option in child objects.</param>
    /// <returns>True if the option is found. False otherwise.</returns>
    public bool HasOption(string optionName, bool searchChildren) =>
        FindOption(optionName, searchChildren) is not null;

}
