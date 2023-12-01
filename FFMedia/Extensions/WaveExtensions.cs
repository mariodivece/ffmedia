using FFmpeg.AutoGen.Abstractions;

namespace FFMedia.Extensions;

internal static unsafe class WaveExtensions
{
    public static AVChannelLayout DefaultChannelLayout(this IWaveSpec spec)
    {
        var target = default(AVChannelLayout);
        ffmpeg.av_channel_layout_default(&target, spec.ChannelCount);
        return target;
    }

    /// <summary>
    /// Gets a human-readable string describing the channel layout properties.
    /// The string will be in the same format that is accepted by
    /// <see cref="ffmpeg.av_channel_layout_from_string(AVChannelLayout*, string)"/>,
    /// allowing to rebuild the same channel layout, except for opaque pointers.
    /// </summary>
    /// <param name="layout">The channel layout.</param>
    /// <returns>The string description.</returns>
    public static string Describe(this AVChannelLayout layout)
    {
        using var bp = new FFBPrint();
        var resultCode = ffmpeg.av_channel_layout_describe_bprint(&layout, bp.Target);
        
        FFException.ThrowIfNegative(resultCode);
        return bp.ToString();
    }

    public static string? ToName(this AVSampleFormat sampleFormat)
    {
        var formatName = ffmpeg.av_get_sample_fmt_name(sampleFormat);
        return string.IsNullOrWhiteSpace(formatName) ? null : formatName;
    }
}
