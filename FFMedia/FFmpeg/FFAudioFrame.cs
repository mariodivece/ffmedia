namespace FFmpeg;

/// <summary>
/// Provides a wrapper for a <see cref="AVFrame"/> of audio.
/// </summary>
public unsafe class FFAudioFrame :
    FFFrameBase,
    IWaveBufferSpec
{

    /// <summary>
    /// Initializes and allocates a new instance of the <see cref="FFAudioFrame"/> class.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFAudioFrame(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(ffmpeg.av_frame_alloc(), filePath, lineNumber)
    {
        // placeholder
    }

    /// <summary>
    /// Creates an instance of the <see cref="FFAudioFrame"/> class, from an already allocated
    /// <see cref="AVFrame"/>
    /// </summary>
    /// <param name="target">The allocated frame pointer.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFAudioFrame(AVFrame* target,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(target, filePath, lineNumber)
    {
        // placeholder
    }

    /// <inheritdoc />
    public AVSampleFormat SampleFormat => (AVSampleFormat)Target->format;

    /// <inheritdoc />
    public int ChannelCount => Target->ch_layout.nb_channels;

    /// <inheritdoc />
    public int SampleRate => Target->sample_rate;

    /// <inheritdoc />
    public nint BufferAddress => new(Target->extended_data[0]);

    /// <inheritdoc />
    public int SampleCount => Target->nb_samples;

    /// <inheritdoc />
    public int BufferLength => ffmpeg.av_samples_get_buffer_size(null, ChannelCount, SampleCount, SampleFormat, 1);

    /// <inheritdoc />
    public int BytesPerFrame => ffmpeg.av_samples_get_buffer_size(null, ChannelCount, 1, SampleFormat, 1);

    /// <inheritdoc />
    public int BytesPerSecond => ffmpeg.av_samples_get_buffer_size(null, ChannelCount, SampleRate, SampleFormat, 1);

    /// <inheritdoc />
    public TimeExtent BufferDuration => SampleRate > 0 ? (double)SampleCount / SampleRate : TimeExtent.NaN;

    /// <inheritdoc />
    public int BytesPerSample => ffmpeg.av_get_bytes_per_sample(SampleFormat);

    /// <inheritdoc />
    public string? SampleFormatName => SampleFormat.ToName();

    /// <inheritdoc />
    public AVChannelLayout ChannelLayout => Target->ch_layout;

    /// <inheritdoc />
    public string? ChannelLayoutName => ChannelLayout.Describe();
}
