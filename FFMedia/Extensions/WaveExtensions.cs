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
    /// <returns>The string descriptor.</returns>
    public static string? Describe(this AVChannelLayout layout)
    {
#pragma warning disable CA2014 // Do not use stackalloc in loops: This is fine because the loop only retries once.

        const int BufferSize = 2048;
        var currentAlloc = BufferSize;
        var requestedAlloc = BufferSize;

        while (currentAlloc <= requestedAlloc)
        {
            var output = stackalloc byte[currentAlloc];
            requestedAlloc = ffmpeg.av_channel_layout_describe(&layout, output, (ulong)currentAlloc);

            if (requestedAlloc < 0)
                break;

            if (requestedAlloc <= currentAlloc)
                return ((nint)output).ReadString();

            currentAlloc = requestedAlloc;
        }

        return null;
#pragma warning restore CA2014 // Do not use stackalloc in loops
    }

    public static string? ToName(this AVSampleFormat sampleFormat)
    {
        var formatName = ffmpeg.av_get_sample_fmt_name(sampleFormat);
        return string.IsNullOrWhiteSpace(formatName) ? null : formatName;
    }
}
