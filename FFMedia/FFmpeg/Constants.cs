namespace FFmpeg;

/// <summary>
/// Stores vrious constants for the FFmpeg library.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the <see cref="ffmpeg.AV_TIME_BASE"/> constant as a <see cref="AVRational"/>
    /// express as 1 / TIME_BASE.
    /// </summary>
    public static readonly AVRational AV_TIME_BASE_Q = ffmpeg.av_make_q(1, ffmpeg.AV_TIME_BASE);

    /// <summary>
    /// For the <see cref="AVFormatContext"/>, in order to determine if the input is seekable.
    /// </summary>
    public const int SeekMethodUnknownFlags = ffmpeg.AVFMT_NOBINSEARCH | ffmpeg.AVFMT_NOGENSEARCH | ffmpeg.AVFMT_NO_BYTE_SEEK;

    /// <summary>
    /// Represents an empty dictionary of string key-value pairs.
    /// </summary>
    public static IReadOnlyDictionary<string, string> EmptyDictionary { get; } = new Dictionary<string, string>(0);

    /// <summary>
    /// Gets the standard timebase for subtitle frames. Start and End display times
    /// for subtitle frames are expressed in milliseconds.
    /// </summary>
    public static AVRational SubtitlesTimeBase { get; } = new AVRational { num = 1, den = 1000 };
}
