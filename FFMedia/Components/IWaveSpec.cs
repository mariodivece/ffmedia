namespace FFMedia.Components;

/// <summary>
/// Represents fields that contain audio sample specifications.
/// </summary>
public interface IWaveSpec
{
    AVChannelLayout ChannelLayout { get; set; }

    AVSampleFormat SampleFormat { get; set; }

    int ChannelCount { get; set; }


}
