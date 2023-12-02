namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for the <see cref="AVStream"/> data structure.
/// </summary>
public unsafe sealed class FFStream : NativeReferenceBase<AVStream>
{
    /// <summary>
    /// Initializes as new instance of the <see cref="FFStream"/> class.
    /// </summary>
    /// <param name="target">The data structure to wrap.</param>
    /// <param name="formatContext">The format context this stream belongs to.</param>
    public FFStream(AVStream* target, FFFormatContext formatContext)
        : base(target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (formatContext is null || formatContext.IsNull)
            throw new ArgumentNullException(nameof(formatContext));

        FormatContext = formatContext;
    }

    /// <summary>
    /// Gets the format context this stream belongs to.
    /// </summary>
    public FFFormatContext FormatContext { get; }

    /// <summary>
    /// Gets this stream's codec media type.
    /// </summary>
    public AVMediaType MediaType => CodecParameters.MediaType;

    /// <summary>
    /// Gets or sets via a set of which packets can be discarded at will and do not need to be demuxed.
    /// </summary>
    public AVDiscard DiscardFlags
    {
        get => Target->discard;
        set => Target->discard = value;
    }

    /// <summary>
    /// Gets the default codec drived from <see cref="CodecParameters"/>.
    /// </summary>
    public FFCodec? DefaultCodec
    {
        get
        {
            var codecId = CodecParameters.CodecId;
            if (codecId == AVCodecID.AV_CODEC_ID_NONE)
                return null;
            
            var result = FFCodec.FromDecoderId(codecId);
            result ??= FFCodec.FromEncoderId(codecId);

            return result;
        }
    }
    /// <summary>
    /// Gets the codec parameters for this stream.
    /// </summary>
    public FFCodecParameters CodecParameters => new(Target->codecpar);


    /// <summary>
    /// Gets a list of currently available programs in this stream.
    /// </summary>
    public IReadOnlyList<FFProgram> Programs
    {
        get
        {
            var result = new List<FFProgram>(16);
            AVProgram* program = default;
            while ((program = ffmpeg.av_find_program_from_stream(FormatContext.Target, program, StreamIndex)) is not null)
                result.Add(new(program));

            return result;
        }
    }


    /// <summary>
    /// Gets the stream index in the format context.
    /// </summary>
    public int StreamIndex => Target->index;

    /// <summary>
    /// Gets the stream time base.
    /// </summary>
    public AVRational TimeBase => Target->time_base;

    /// <summary>
    /// Gets the pts of the first frame of the stream in presentation order, in stream time base.
    /// Only set this if you are absolutely 100% sure that the value you set it to really is the
    /// pts of the first frame. This may be undefined (AV_NOPTS_VALUE).</summary>
    public long StartTimeUnits => Target->start_time;

    /// <summary>
    /// Gets the start time in seconds derived from <see cref="TimeBase"/> and
    /// <see cref="StartTimeUnits"/>.
    /// </summary>
    public TimeExtent StartTime => StartTimeUnits.ToSeconds(TimeBase);

    /// <summary>
    /// Gets the stream disposition - a combination of AV_DISPOSITION_* flags. - this is set by
    /// libavformat when creating the stream or in avformat_find_stream_info(). -
    /// muxing: may be set by the caller before avformat_write_header().
    /// </summary>
    public int DispositionFlags
    {
        get => Target->disposition;
        set => Target->disposition = value;
    }

    /// <summary>
    /// Gets a value indicating whether the stream currenty holds
    /// a packet that can be decoded into a picture.
    /// </summary>
    public bool HasAttchedPicture =>
        DispositionFlags.HasFlag(ffmpeg.AV_DISPOSITION_ATTACHED_PIC) &&
        Target->attached_pic.data is not null;

    /// <summary>
    /// Extracts a newly-allocated packet that references the same data
    /// as the attached picture.
    /// </summary>
    /// <returns>True if the packet was allocated. False otherwise.</returns>
    public bool TryCloneAttachedPicture([MaybeNullWhen(false)] out FFPacket packet)
    {
        packet = null;
        if (!HasAttchedPicture)
            return false;

        var currentPicture = &Target->attached_pic;
        packet = FFPacket.Clone(currentPicture);
        return true;
    }

    /// <summary>
    /// Guesses the frame rate of this stream.
    /// Optionally provide a frame for better guesses.
    /// Will return a 0/1 value if not determined.
    /// </summary>
    /// <param name="frame">The optional sample frame.</param>
    /// <returns>The frame rate.</returns>
    public AVRational GuessFrameRate(FFVideoFrame? frame = default) => ffmpeg.av_guess_frame_rate(
        FormatContext.Target, Target, frame is not null && !frame.IsNull ? frame.Target : null);

    /// <summary>
    /// Guesses the pixel aspect ratio of this stream.
    /// Optionally provide a frame for better guesses.
    /// Will return a 0/1 value if not determined.
    /// </summary>
    /// <param name="frame">The optional sample frame.</param>
    /// <returns>The aspect ratio.</returns>
    public AVRational GuessAspectRatio(FFVideoFrame? frame = default) => ffmpeg.av_guess_sample_aspect_ratio(
        FormatContext.Target, Target, frame is not null && !frame.IsNull ? frame.Target : null);
}
