namespace FFMedia.Components;

/// <summary>
/// Defines the members of a multimedia frame that aew relevant
/// for presentation purposes.
/// </summary>
public interface IMediaFrame : ISerialGroupable, IDisposable
{
    /// <summary>
    /// Gets the thype of media of this frame.
    /// </summary>
    AVMediaType MediaType { get; }

    /// <summary>
    /// Gets the presentation start time of this frame.
    /// </summary>
    TimeExtent StartTime { get; }

    /// <summary>
    /// Gets the duration of this frame.
    /// </summary>
    TimeExtent Duration { get; }
}
