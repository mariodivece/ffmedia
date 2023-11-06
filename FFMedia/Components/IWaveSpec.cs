namespace FFMedia.Components;

/// <summary>
/// Represents fields that contain audio sample specifications.
/// </summary>
public unsafe interface IWaveSpec
{
    /// <summary>
    /// Gets the channel layout.
    /// </summary>
    AVChannelLayout ChannelLayout { get; }

    /// <summary>
    /// Gets a description of the <see cref="ChannelLayout"/>.
    /// </summary>
    string? ChannelLayoutName { get; }

    /// <summary>
    /// Gets the audio sample format.
    /// </summary>
    AVSampleFormat SampleFormat { get; }

    /// <summary>
    /// Gets the number of bytes per individual sample (per channel).
    /// </summary>
    int BytesPerSample {  get; }

    /// <summary>
    /// Gets the number of bytes per audio frame. Unlike <see cref="BytesPerSample"/>, this
    /// measure includes of 1 sample fro all audio channels.
    /// </summary>
    int BytesPerFrame { get; }

    /// <summary>
    /// Gets the number of bytes required to store 1 second of audio.
    /// </summary>
    int BytesPerSecond {  get; }

    /// <summary>
    /// Gets the string representation of the <see cref="SampleFormat"/>
    /// </summary>
    string? SampleFormatName { get; }

    /// <summary>
    /// Gets the audio channel count.
    /// </summary>
    int ChannelCount { get; }

    /// <summary>
    /// Gets the audio sampling rate per audio channel.
    /// </summary>
    int SampleRate { get; }
}
