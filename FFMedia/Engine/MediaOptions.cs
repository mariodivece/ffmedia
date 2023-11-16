namespace FFMedia.Engine;

/// <summary>
/// Provides configuration options for a <see cref="MediaContainer"/>.
/// </summary>
public record MediaOptions
{
    /// <summary>
    /// Port of VIDEO_PICTURE_QUEUE_SIZE.
    /// </summary>
    public int VideoFramesCapacity { get; init; } = 3;

    /// <summary>
    /// Port of SAMPLE_QUEUE_SIZE.
    /// </summary>
    public int AudioFramesCapacity { get; init; } = 9;

    /// <summary>
    /// Port of SUBPICTURE_QUEUE_SIZE.
    /// </summary>
    public int SubtitleFramesCapacity { get; init; } = 16;

    /// <summary>
    /// Port of FRAME_QUEUE_SIZE.
    /// </summary>
    public int FramesMaxCapacity => Math.Max(Math.Max(VideoFramesCapacity, AudioFramesCapacity), SubtitleFramesCapacity);



}
