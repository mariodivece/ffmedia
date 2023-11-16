namespace FFMedia.Components;

/// <summary>
/// Represents a set of media frames in its locked, read-only state.
/// </summary>
/// <typeparam name="TMedia"></typeparam>
public interface IFrameGraph<TMedia> :
    IDisposable,
    ISerialGroupable,
    IComponentService<TMedia>
    where TMedia : class, IMediaFrame
{
    TMedia this[int index] { get; }

    /// <summary>
    /// GEts the number of elements currently stored in the frame graph.
    /// </summary>
    int Count { get; }

    bool IsFull { get; }

    bool IsEmpty { get; }

    int MinIndex { get; }

    int MaxIndex { get; }

    TimeExtent MinStartTime { get; }

    TimeExtent MaxStartTime { get; }

    TimeExtent RangeStartTime { get; }

    TimeExtent RangeEndTime { get; }

    TimeExtent TotalDuration { get; }

    bool Contains(TMedia item);

    bool Contains(TimeExtent time);

    int IndexOf(TMedia item);

    TMedia? FindFrame(TimeExtent startTime);

    int FindFrameIndex(TimeExtent startTime);

    /// <summary>
    /// Finds the relative position of the specified time within
    /// <see cref="MinStartTime"/> and <see cref="RangeEndTime"/>.
    /// Negative numbers: the time occurs before the first position.
    /// Numbers greater than 1: the time occurs after the last position.
    /// Numbers 0.0 to 1.0: the time occurs at the given percent of the
    /// <see cref="TotalDuration"/>. If <see cref="IsEmpty"/> evaluates to true,
    /// then the output will be 0.0.
    /// </summary>
    /// <param name="time">The time to compute the relative position for.</param>
    /// <returns>The relative position of the given time</returns>
    double FindRelativePosition(TimeExtent time);

    double FindRelativePosition(TMedia item);
}