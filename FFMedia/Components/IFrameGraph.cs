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

    /// <summary>
    /// Gets the frame at the given index position.
    /// </summary>
    /// <param name="index">The index position.</param>
    /// <returns>The frame at the given index position.</returns>
    TMedia this[int index] { get; }

    /// <summary>
    /// Gets the number of elements currently stored in the frame graph.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets a value indicating whether the frame graph is at full
    /// capacity.
    /// </summary>
    bool IsFull { get; }

    /// <summary>
    /// Gets a value indicating whether the frame graph contains no
    /// elements.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Gets a value indicating the first index of the frame graph.
    /// Returns -1 if no elements are found.
    /// </summary>
    int MinIndex { get; }

    /// <summary>
    /// Gets a value indicating the last index of the frame graph.
    /// Returns -1 if no elements are found.
    /// </summary>
    int MaxIndex { get; }

    /// <summary>
    /// Gets the start time of the last frame in the graph.
    /// Returns <see cref="TimeExtent.NaN"/>
    /// if the graph <see cref="IsEmpty"/>.
    /// </summary>
    TimeExtent MaxStartTime { get; }

    /// <summary>
    /// Gets the start time of the first frame in the graph.
    /// Returns <see cref="TimeExtent.NaN"/>
    /// if the graph <see cref="IsEmpty"/>.
    /// </summary>
    TimeExtent RangeStartTime { get; }

    /// <summary>
    /// Gets the start time of the last frame in the graph
    /// plus its duration. Returns <see cref="TimeExtent.NaN"/>
    /// if the graph <see cref="IsEmpty"/>.
    /// </summary>
    TimeExtent RangeEndTime { get; }

    /// <summary>
    /// Gets the total duration of the frame graph.
    /// Returns <see cref="TimeExtent.Zero"/>
    /// if the graph <see cref="IsEmpty"/>.
    /// </summary>
    TimeExtent TotalDuration { get; }

    /// <summary>
    /// Adds a frame to the graph.
    /// If the graph <see cref="IsFull"/>
    /// makes room for the new frame by removing a frame in either the
    /// last or the first position depending on whether the newly added frame
    /// is added in the first or second half of the graph respectively.
    /// All added frames must specify a valid and finite start time
    /// and diration.
    /// </summary>
    /// <param name="item">The frame to add.</param>
    void Add(TMedia item);

    /// <summary>
    /// Gets a value indicating whether the specified
    /// frame reference already exists in the graph.
    /// </summary>
    /// <param name="item">The frame to search for.</param>
    /// <returns>The result.</returns>
    bool Contains(TMedia item);

    /// <summary>
    /// Gets a value indicating whether the specified time
    /// occurs between <see cref="RangeStartTime"/> and
    /// <see cref="RangeEndTime"/>.
    /// </summary>
    /// <param name="time">The time to search for.</param>
    /// <returns>The result.</returns>
    bool Contains(TimeExtent time);

    /// <summary>
    /// Gets the index within the graph of given frame reference.
    /// If the frame reference is not found then -1 is returned.
    /// </summary>
    /// <param name="item">The frame to search for.</param>
    /// <returns>The frame index.</returns>
    int IndexOf(TMedia item);

    /// <summary>
    /// Finds the first frame occurring on or after the given time.
    /// If the graph is empty, null is returned.
    /// If the time occurs before the <see cref="RangeStartTime"/>
    /// then the first frame is returned.
    /// If the time occurs after the <see cref="RangeEndTime"/>
    /// then the last frame is returned.
    /// If none of those edge cases match, then it returns the
    /// first occurrence of a frame that has its start time on or after the given time.
    /// </summary>
    /// <param name="startTime">The time to search for.</param>
    /// <returns>The media frame if found.</returns>
    TMedia? FindFrame(TimeExtent startTime);

    /// <summary>
    /// Finds the first frame index occurring on or after the given time.
    /// The logic is identical to the <see cref="FindFrame(TimeExtent)"/>
    /// method call, except it returns -1 if the graph <see cref="IsEmpty"/>.
    /// </summary>
    /// <param name="startTime">The time to search for.</param>
    /// <returns>The media frame index if found.</returns>
    int FindFrameIndex(TimeExtent startTime);

    /// <summary>
    /// Finds the relative position of the specified time within
    /// <see cref="RangeStartTime"/> and <see cref="RangeEndTime"/>.
    /// Negative numbers: the time occurs before the first position.
    /// Numbers greater than 1: the time occurs after the last position.
    /// Numbers 0.0 to 1.0: the time occurs at the given percent of the
    /// <see cref="TotalDuration"/>. If <see cref="IsEmpty"/> evaluates to true,
    /// then the output will be 0.0.
    /// </summary>
    /// <param name="time">The time to compute the relative position for.</param>
    /// <returns>The relative position of the given time</returns>
    double FindRelativePosition(TimeExtent time);

    /// <summary>
    /// Finds the relative position of the specified time within
    /// <see cref="RangeStartTime"/> and <see cref="RangeEndTime"/>.
    /// The same logic is applied as <see cref="FindRelativePosition(TimeExtent)"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    double FindRelativePosition(TMedia item);
}