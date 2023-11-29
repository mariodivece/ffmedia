namespace FFmpeg;

/// <summary>
/// Represents a wrapper for the <see cref="AVPacket"/> structure.
/// </summary>
public unsafe sealed class FFPacket : NativeTrackedReferenceBase<AVPacket>, ISerialGroupable
{
    /// <summary>
    /// Creates an instance of the <see cref="FFPacket"/> class, automatically allocating the <see cref="AVPacket"/>
    /// in native memory.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFPacket(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : this(ffmpeg.av_packet_alloc(), filePath, lineNumber)
    {
        // placeholder
    }

    /// <summary>
    /// Creates an instance of the <see cref="FFPacket"/> class, from an already allocated
    /// <see cref="AVPacket"/>
    /// </summary>
    /// <param name="target">The allocated packet pointer.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    public FFPacket(AVPacket* target,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
        : base(target, filePath, lineNumber)
    {
        // placeholder
    }

    /// <inheritdoc />
    public int GroupIndex { get; set; }

    /// <summary>
    /// Gets a value indicating whether this packet serves as a flush packet.
    /// Flush packets are created by calling the <see cref="CreateFlushPacket"/> method.
    /// </summary>
    public bool IsFlushPacket { get; private set; }

    /// <summary>
    /// Gets the stream index this packet belongs to.
    /// </summary>
    public int StreamIndex
    {
        get => Target is not null ? Target->stream_index : default;
        private set => Target->stream_index = value;
    }

    /// <summary>
    /// Gets the duration of this packet in <see cref="AVStream.time_base"/> units.
    /// Returns 0 if unkown.
    /// </summary>
    public long DurationUnits
    {
        get => Target is not null ? Target->duration : default;
        set => Target->duration = value;
    }

    /// <summary>
    /// Gets the number of bytes in the packet's <see cref="Data"/>.
    /// </summary>
    public int DataSize
    {
        get => Target is not null ? Target->size : default;
        set => Target->size = value;
    }

    /// <summary>
    /// Gets a pointer to the contests of the <see cref="AVPacket"/>.
    /// </summary>
    public byte* Data
    {
        get => Target is not null ? Target->data : default;
        set => Target->data = value;
    }

    /// <summary>
    /// Gesta  value indicating whether 
    /// </summary>
    public bool HasData => !IsNull && Data is not null;

    /// <summary>
    /// Gets the presentation timestamp in <see cref="AVStream.time_base"/> units.
    /// Returns null if unknown.
    /// </summary>
    public long? PtsUnits => Target is not null
        ? Target->pts.IsValidTimestamp() ? Target->pts : Target->dts.ToNullable()
        : default;

    /// <summary>
    /// Gets the decompression timestamp in <see cref="AVStream.time_base"/> units.
    /// Returns null if not stored in the file.
    /// </summary>
    public long? DtsUnits => Target is not null ? Target->dts.ToNullable() : default;

    /// <summary>
    /// Creates and allocates a special packet that forces the stream to be flushed.
    /// </summary>
    /// <returns>The newly created and allocated packet.</returns>
    public static FFPacket CreateFlushPacket(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
    {
        var packet = new FFPacket(filePath, lineNumber)
        {
            DataSize = 0,
            IsFlushPacket = true
        };

        packet.Data = (byte*)packet.Target;
        return packet;
    }

    /// <summary>
    /// Creates and allocates a packet with no data for the specified
    /// stream index.
    /// </summary>
    /// <param name="streamIndex">The stream index for which this empty packet was allocated.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    /// <returns>The newly created and allocated packet.</returns>
    public static FFPacket CreateNullPacket(int streamIndex,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default) => new(filePath, lineNumber)
        {
            Data = default,
            DataSize = 0,
            StreamIndex = streamIndex,
            DurationUnits = 0
        };

    /// <summary>
    /// Makes a newly allocated copy of this packet that references the same <see cref="Data"/>.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_packet_clone"/>.</remarks>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    /// <returns>The cloned packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public FFPacket Clone(
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default) =>
        Clone(Target, filePath, lineNumber);

    /// <summary>
    /// Makes a newly allocated copy of the specified packet that
    /// references the same <see cref="Data"/>.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.av_packet_clone"/>.</remarks>
    /// <param name="source">The packet to clone.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="lineNumber">The source line number.</param>
    /// <returns>The cloned packet.</returns>
    public static FFPacket Clone(AVPacket* source,
        [CallerFilePath] string? filePath = default,
        [CallerLineNumber] int? lineNumber = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new(ffmpeg.av_packet_clone(source), filePath, lineNumber);
    }

    /// <inheritdoc />
    protected override void ReleaseInternal(AVPacket* target) =>
        ffmpeg.av_packet_free(&target);
}
