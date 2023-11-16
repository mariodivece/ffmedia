namespace FFMedia.Components;

/// <summary>
/// Defines the members of a native FFmpeg media frame.
/// </summary>
public unsafe interface INativeFrame :
    INativeTrackedReference,
    INativeReference
{
    /// <summary>
    /// Gets the media type of this frame.
    /// </summary>
    AVMediaType MediaType { get; }

    /// <summary>
    /// Gets the reordered position from the last <see cref="AVPacket"/>
    /// that has been input into the decoder.
    /// </summary>
    long PacketPosition { get; }

    /// <summary>Gets
    /// Gets a pointer to the picture/channel planes. This might be different from the first allocated byte.
    /// For video, it could even point to the end of the image data.
    /// </summary>
    byte_ptr8 Data { get; }

    /// <summary>
    /// Gets the Presentation timestamp in
    /// time base units (time when frame should be shown to user).
    /// </summary>
    long? Pts { get; }

    /// <summary>
    /// Gets the DTS copied from the AVPacket that triggered returning this frame. (if frame threading isn't used)
    /// This is also the Presentation time of this <see cref="AVFrame"/> calculated from only <see cref="AVPacket.dts"/>
    /// values without pts values.
    /// </summary>
    long? PacketDts { get; }

    /// <summary>
    /// Gets the frame timestamp estimated using various heuristics, in stream time base units.
    /// </summary>
    long? BestEffortPts { get; }

    /// <summary>
    /// Gets the pointers to the data planes/channels.
    /// </summary>
    byte** ExtendedData { get; }
}
