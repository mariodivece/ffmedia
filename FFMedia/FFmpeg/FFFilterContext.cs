using FFmpeg.AutoGen.Abstractions;

namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for <see cref="AVFilterContext"/>.
/// These represent 'live' instances of <see cref="FFFilter"/> prototypes.
/// </summary>
public unsafe sealed class FFFilterContext :
    NativeReferenceBase<AVFilterContext>,
    IFFOptionsEnabled
{
    /// <summary>
    /// Creates a new instance of the <see cref="FFFilterContext"/> class.
    /// </summary>
    /// <param name="target"></param>
    public FFFilterContext(AVFilterContext* target)
        : base(target)
    {
        // placeholder
    }

    /// <inheritdoc />
    public FFMediaClass MediaClass => OptionsWrapper.MediaClass;

    /// <inheritdoc />
    public IReadOnlyList<FFOption> CurrentOptions => OptionsWrapper.CurrentOptions;

    /// <inheritdoc />
    public IReadOnlyList<IFFOptionsEnabled> CurrentChildren => OptionsWrapper.CurrentChildren;

    /// <summary>
    /// Gets the media type of the buffer sink.
    /// </summary>
    public AVMediaType MediaType => ffmpeg.av_buffersink_get_type(Target);

    /// <summary>
    /// Gets the video frame rate from the buffer sink of the filter.
    /// </summary>
    public AVRational FrameRate => ffmpeg.av_buffersink_get_frame_rate(Target);

    /// <summary>
    /// Gets the video pixel format from the buffer sink of the filter.
    /// </summary>
    public AVPixelFormat PixelFormat => (AVPixelFormat)ffmpeg.av_buffersink_get_format(Target);

    /// <summary>
    /// Gets the video pixel width from the buffer sink of the filter.
    /// </summary>
    public int PixelWidth => ffmpeg.av_buffersink_get_w(Target);

    /// <summary>
    /// Gets the video pixel height from the buffer sink of the filter.
    /// </summary>
    public int PixelHeight => ffmpeg.av_buffersink_get_h(Target);

    /// <summary>
    /// Gets the audio sample rate from the buffer sink of the filter.
    /// </summary>
    public int SampleRate => ffmpeg.av_buffersink_get_sample_rate(Target);

    /// <summary>
    /// Gets the audio sample format from the buffer sink of the filter.
    /// </summary>
    public AVSampleFormat SampleFormat => (AVSampleFormat)ffmpeg.av_buffersink_get_format(Target);

    /// <summary>
    /// Gets the audio channel count from the buffer sink of the filter.
    /// </summary>
    public int ChannelCount => ChannelLayout.nb_channels;

    /// <summary>
    /// Gets the audio channel layout.
    /// </summary>
    public AVChannelLayout ChannelLayout
    {
        get
        {
            AVChannelLayout layout = default;
            ffmpeg.av_buffersink_get_ch_layout(Target, &layout);
            return layout;
        }

    }

    /// <summary>
    /// Gets the time base from the buffer sink of the filter.
    /// </summary>
    public AVRational TimeBase => ffmpeg.av_buffersink_get_time_base(Target);

    /// <summary>
    /// Gets a wrapper for implementing <see cref="IFFOptionsEnabled"/>.
    /// </summary>
    private IFFOptionsEnabled OptionsWrapper => FFOptionsStore.TryWrap(this, out var options)
        ? options
        : FFOptionsStore.Empty;

    /// <summary>
    /// Attempts to read the filtered frame off the filter into
    /// an already allocated target frame.
    /// </summary>
    /// <param name="outputFrame">The frame to write to.</param>
    /// <param name="resultCode">The result code. Will be negative on failure.</param>
    /// <returns>True on success. Flase on failure.</returns>
    public bool TryReadSinkFrame(FFFrameBase outputFrame, out int resultCode)
    {
        const int DefaultFlags = default;

        resultCode = ffmpeg.AVERROR_UNKNOWN;
        if (outputFrame is null || outputFrame.IsNull)
            return false;

        resultCode = ffmpeg.av_buffersink_get_frame_flags(
            Target, outputFrame.Target, DefaultFlags);

        return resultCode >= 0;
    }

    /// <summary>
    /// Attempts to write a frame to the filter input.
    /// </summary>
    /// <param name="inputFrame">The frame to write into the context input.</param>
    /// <param name="resultCode">The result code. Will be negative on error.</param>
    /// <returns>True on success. Flase on failure.</returns>
    public bool TryWriteSourceFrame(FFFrameBase inputFrame, out int resultCode)
    {
        resultCode = ffmpeg.AVERROR_UNKNOWN;
        if (inputFrame is null || inputFrame.IsNull)
            return false;

        resultCode = ffmpeg.av_buffersrc_add_frame(Target, inputFrame.Target);
        return resultCode >= 0;
    }

    /// <summary>
    /// Links 2 filter instances together.
    /// Will throw on error.
    /// </summary>
    /// <param name="input">The input filter instance.</param>
    /// <param name="output">The output filter instance.</param>
    /// <param name="destinationPadIndex">The index of the input pad on the destination filter</param>
    public static void Link(FFFilterContext input, FFFilterContext output, int destinationPadIndex = default)
    {
        if (input is null || input.IsNull)
            throw new ArgumentNullException(nameof(input));

        if (output is null || output.IsNull)
            throw new ArgumentNullException(nameof(output));

        var resultCode = ffmpeg.avfilter_link(
            input.Target, 0, output.Target, Convert.ToUInt32(destinationPadIndex));
        
        if (resultCode != 0)
            throw new FFException(resultCode, "Failed to link filters.");
    }
}
