namespace FFMedia.Extensions;

internal static class TimestampExtensions
{
    /// <summary>
    /// Gets a value indicating whether a timestamp does not have a value of
    /// <see cref="ffmpeg.AV_NOPTS_VALUE"/>.
    /// </summary>
    /// <param name="value">The timestamp value.</param>
    /// <returns>The result.</returns>
    public static bool IsValidTimestamp(this long value) => value != ffmpeg.AV_NOPTS_VALUE;

    /// <summary>
    /// Converts a <see cref="ffmpeg.AV_NOPTS_VALUE"/> to a null value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The nullable value.</returns>
    public static long? ToNullable(this long value) => value == ffmpeg.AV_NOPTS_VALUE ? null : value;

}
