namespace FFmpeg;

using FFmpeg.AutoGen.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Serves as a wrapper for the <see cref="AVFormatContext"/> data structure.
/// </summary>
public unsafe sealed class FFFormatContext :
    NativeTrackedReferenceBase<AVFormatContext>
{
    private readonly AVIOInterruptCB_callback NativeInterruptCallback;

    /// <summary>
    /// Initializes a new instance fo the <see cref="FFFormatContext"/> class.
    /// </summary>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFFormatContext([CallerFilePath] string? filePath = default, [CallerLineNumber] int? lineNumber = default)
        : base(ffmpeg.avformat_alloc_context(), filePath, lineNumber)
    {
        NativeInterruptCallback = new(InputInterrupt);
        Target->interrupt_callback.callback = NativeInterruptCallback;
    }

    /// <summary>
    /// Gets or sets the IO interrupt callback delegate.
    /// </summary>
    public FormatContextInterruptCallback? InterruptCallback { get; set; }

    /// <summary>
    /// Gets a list of streams.
    /// </summary>
    public FFStreamSet Streams => new(this);

    /// <summary>
    /// Gets a list of chapters.
    /// </summary>
    public FFChapterSet Chapters => new(this);

    /// <summary>
    /// Gets the input format.
    /// </summary>
    public FFInputFormat InputFormat => Target is not null && Target->iformat is not null
        ? new(Target->iformat)
        : FFInputFormat.Empty;

    /// <summary>
    /// Gets the IO context.
    /// </summary>
    public FFIOContext IO => Target is not null && Target->pb is not null
        ? new(Target->pb)
        : FFIOContext.Empty;

    /// <summary>
    /// Gets the format context metadata.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata => Target is not null
        ? FFDictionary.ToDictionary(Target->metadata)
        : Constants.EmptyDictionary;

    /// <summary>
    /// Gets or sets a combination of AV_FMT_FLAG_*
    /// before opening an input stream.
    /// </summary>
    public int Flags
    {
        get => Target->flags;
        set => Target->flags = value;
    }

    /// <summary>
    /// Gets the duration.
    /// </summary>
    public TimeExtent Duration => Target is not null
        ? Target->duration.ToSeconds()
        : TimeExtent.NaN;

    /// <summary>
    /// Gets the start time.
    /// </summary>
    public TimeExtent StartTime => Target is not null
        ? Target->start_time.ToSeconds()
        : TimeExtent.NaN;

    /// <summary>
    /// Gets the URL for this format context.
    /// </summary>
    public string? Url => Target is not null && Target->url is not null
        ? NativeExtensions.ReadString(Target->url)
        : default;

    public int FindBestStream(AVMediaType mediaType, int wantedStreamIndex, int relatedStreamIndex) =>
        ffmpeg.av_find_best_stream(Target, mediaType, wantedStreamIndex, relatedStreamIndex, null, 0);

    public int FindBestVideoStream(int wantedStreamIndex) =>
        FindBestStream(AVMediaType.AVMEDIA_TYPE_VIDEO, wantedStreamIndex, -1);

    public int FindBestAudioStream(int wantedStreamIndex, int relatedStreamIndex) =>
        FindBestStream(AVMediaType.AVMEDIA_TYPE_AUDIO, wantedStreamIndex, relatedStreamIndex);

    public int FindBestSubtitleStream(int wantedStreamIndex, int relatedStreamIndex) =>
        FindBestStream(AVMediaType.AVMEDIA_TYPE_SUBTITLE, wantedStreamIndex, relatedStreamIndex);

    public int ReadPlay() =>
        ffmpeg.av_read_play(Target);

    public int ReadPause() =>
        ffmpeg.av_read_pause(Target);

    /// <summary>
    /// Gets a value indicating whether the seek method for the input is not known.
    /// </summary>
    public bool IsSeekMethodUnknown =>
        Target is not null &&
        InputFormat is not null &&
        InputFormat.Target is not null &&
        InputFormat.Flags.HasFlag(Constants.SeekMethodUnknownFlags) &&
        InputFormat.Target->read_seek.Pointer == default;

    /// <summary>
    /// Gets a value indicating whether the input is real-time.
    /// </summary>
    public bool IsRealTime
    {
        get
        {
            var formatNames = InputFormat?.ShortNames?
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.ToUpperInvariant()) ?? Array.Empty<string>();

            if (formatNames.Any(c => c == "RTP" || c == "RTSP" || c == "SDP"))
                return true;

            var url = Url?.ToUpperInvariant() ?? string.Empty;
            var isRealtimeProtocol =
                url.StartsWith("RTP:", StringComparison.Ordinal) ||
                url.StartsWith("UDP:", StringComparison.Ordinal);
            
            return IO is not null && !IO.IsNull && isRealtimeProtocol;
        }
    }

    public int SeekFile(long seekTargetMin, long seekTarget, long seekTargetMax, int seekFlags = 0) =>
        ffmpeg.avformat_seek_file(Target, -1, seekTargetMin, seekTarget, seekTargetMax, seekFlags);

    public int ReadFrame(out FFPacket packet)
    {
        packet = new FFPacket();
        return ffmpeg.av_read_frame(Target, packet.Target);
    }

    public void OpenInput(string filePath, FFInputFormat format, FFDictionary formatOptions)
    {
        const string ScanAllPmtsKey = "scan_all_pmts";

        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(format);
        ArgumentNullException.ThrowIfNull(formatOptions);

        var isScanAllPmtsSet = false;
        if (!formatOptions.ContainsKey(ScanAllPmtsKey))
        {
            formatOptions[ScanAllPmtsKey] = "1";
            isScanAllPmtsSet = true;
        }

        var context = Target;
        var formatOptionsPtr = formatOptions.Target;
        var resultCode = ffmpeg.avformat_open_input(&context, filePath, format.Target, &formatOptionsPtr);
        Update(context);
        formatOptions.Update(formatOptionsPtr);

        if (context is not null)
            format.Update(context->iformat);

        if (isScanAllPmtsSet)
            formatOptions.Remove(ScanAllPmtsKey);

        FFException.ThrowIfNegative(resultCode, $"Unable to open input '{filePath}'");
    }

    /// <summary>
    /// Causes global side data to be injected in the next packet
    /// of each stream as well as after any subsequent seek.
    /// </summary>
    public void InjectGlobalSideData()
    {
        if (Target is null) return;
        ffmpeg.av_format_inject_global_side_data(Target);
    }

    /// <summary>
    /// Prints detailed information about the input format, such as duration, bitrate,
    /// streams, container, programs, metadata, side data, codec and time base, and
    /// posts such information to the log.
    /// </summary>
    public void LogInputFormatDump() => ffmpeg.av_dump_format(Target, 0, Url, 0);

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVFormatContext* target) =>
        ffmpeg.avformat_close_input(&target);

    /// <summary>
    /// A managed wrapper for <see cref="AVIOInterruptCB"/>.
    /// </summary>
    /// <param name="opaque">An opaque reference that is set when a function is blocking.</param>
    /// <returns>0 to continue. Anything non-zero to break.</returns>
    private int InputInterrupt(void* opaque)
    {
        var callback = InterruptCallback;
        if (callback is null)
            return 0;

        return callback(new NativeReference(opaque));
    }
}
