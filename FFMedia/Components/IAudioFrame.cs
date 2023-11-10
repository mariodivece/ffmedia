namespace FFMedia.Components;

/// <summary>
/// Defines the members for a working audio frame.
/// </summary>
public interface IAudioFrame : IWaveBufferSpec, IMediaFrame
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
}
