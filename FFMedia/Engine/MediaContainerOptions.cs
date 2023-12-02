namespace FFMedia.Engine;

public class MediaContainerOptions : MediaOptionsBase
{
    /// <summary>
    /// Port of VIDEO_PICTURE_QUEUE_SIZE.
    /// </summary>
    public int VideoFramesCapacity
    {
        get => GetValueOrDefault(3);
        set => SetOptionValue(value);
    }

    /// <summary>
    /// Port of SAMPLE_QUEUE_SIZE.
    /// </summary>
    public int AudioFramesCapacity
    {
        get => GetValueOrDefault(9);
        set => SetOptionValue(value);
    }

    /// <summary>
    /// Port of SUBPICTURE_QUEUE_SIZE.
    /// </summary>
    public int SubtitleFramesCapacity
    {
        get => GetValueOrDefault(16);
        set => SetOptionValue(value);
    }

    /// <summary>
    /// Port of FRAME_QUEUE_SIZE.
    /// </summary>
    public int FramesMaxCapacity => Math.Max(Math.Max(VideoFramesCapacity, AudioFramesCapacity), SubtitleFramesCapacity);

}
