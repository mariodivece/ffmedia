using FFmpeg.AutoGen.Abstractions;

namespace FFMedia.Components;

/// <summary>
/// Represents fields that contain audio sample specifications.
/// </summary>
public unsafe interface IWaveSpec
{
    /// <summary>
    /// Defines the default audio sample format (S16).
    /// </summary>
    public const AVSampleFormat DefaultSampleFormat = AVSampleFormat.AV_SAMPLE_FMT_S16;

    /// <summary>
    /// Defines the default channel count (2).
    /// </summary>
    public const int DefaultChannelCount = 2;

    /// <summary>
    /// Defines the default sample rate.
    /// </summary>
    public const int DefaultSampleRate = 44100;

    /// <summary>
    /// Gets the channel layout.
    /// </summary>
    AVChannelLayout ChannelLayout
    {
        get
        {
            var target = default(AVChannelLayout);
            ffmpeg.av_channel_layout_default(&target, ChannelCount);
            return target;
        }
    }

    /// <summary>
    /// Gets a description of the <see cref="ChannelLayout"/>.
    /// </summary>
    string? ChannelLayoutName
    {
        get
        {
            const int StringBufferLength = 2048;
            var channelLayout = ChannelLayout;
            var filterLayoutString = stackalloc byte[StringBufferLength];
            ffmpeg.av_channel_layout_describe(&channelLayout, filterLayoutString, StringBufferLength);
            return ((nint)filterLayoutString).ReadString();
        }
    }

    /// <summary>
    /// Gets the audio sample format.
    /// </summary>
    AVSampleFormat SampleFormat { get; }

    /// <summary>
    /// Gets the number of bytes per individual sample.
    /// </summary>
    int BytesPerSample => ffmpeg.av_get_bytes_per_sample(SampleFormat);

    /// <summary>
    /// Gets the number of bytes per audio frame. Unlike <see cref="BytesPerSample"/>, this
    /// measure is inclusive of 1 sample per each audio channel.
    /// </summary>
    int BytesPerFrame => ffmpeg.av_samples_get_buffer_size(null, ChannelCount, 1, SampleFormat, 1);

    /// <summary>
    /// Gets the number of bytes required to store 1 second of audio.
    /// </summary>
    int BytesPerSecond => ffmpeg.av_samples_get_buffer_size(null, ChannelCount, SampleRate, SampleFormat, 1);

    /// <summary>
    /// Gets the string representation of the <see cref="SampleFormat"/>
    /// </summary>
    string? SampleFormatName
    {
        get
        {
            var formatName = ffmpeg.av_get_sample_fmt_name(SampleFormat);
            return string.IsNullOrWhiteSpace(formatName) ? null : formatName;
        }
    }

    /// <summary>
    /// Gets the audio channel count.
    /// </summary>
    int ChannelCount { get; }

    /// <summary>
    /// Gets the audio sampling rate per audio channel.
    /// </summary>
    int SampleRate { get; }
}
