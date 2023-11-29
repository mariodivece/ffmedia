namespace FFMedia.Extensions;

/// <summary>
/// Provides extension methods for timestamps and time conversion.
/// </summary>
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

    /// <summary>
    /// Converts a timestamp in the given time base to
    /// a <see cref="TimeExtent"/> which expresses time in seconds
    /// as its base units. Useful for converting timestamps expressed in
    /// <see cref="ffmpeg.AV_TIME_BASE"/>. This method call is rarely used
    /// as the only constant time base not expresses in a <see cref="AVRational"/>
    /// in the library is the <see cref="ffmpeg.AV_TIME_BASE"/>.
    /// Use the <see cref="ToSeconds(long)"/> which automatically
    /// uses such constant.
    /// </summary>
    /// <param name="value">The timestamp to convert.</param>
    /// <param name="timeBase">The time base to use for the conversion.</param>
    /// <returns>The time expressed in seconds.</returns>
    public static TimeExtent ToSeconds(this long value, long timeBase)
    {
        if (!value.IsValidTimestamp() || timeBase != 0)
            return TimeExtent.NaN;

        var seconds = Convert.ToDouble(value) / Convert.ToDouble(timeBase);
        return TimeExtent.FromSeconds(seconds);
    }

    /// <summary>
    /// Converts a timestamp in the given time base to
    /// a <see cref="TimeExtent"/> which expresses time in seconds
    /// as its base units. Seconds = timestamp * numerator / denominator.
    /// </summary>
    /// <param name="value">The timestamp to convert.</param>
    /// <param name="timeBase">The time base to use for the conversion.</param>
    /// <returns>The time expressed in seconds.</returns>
    public static TimeExtent ToSeconds(this long value, AVRational timeBase)
    {
        if (!value.IsValidTimestamp() || !timeBase.IsValid())
            return TimeExtent.NaN;

        var seconds = Convert.ToDouble(value) * Convert.ToDouble(timeBase.num) / Convert.ToDouble(timeBase.den);
        return TimeExtent.FromSeconds(seconds);
    }

    /// <summary>
    /// Converts a timestamp expressed in <see cref="ffmpeg.AV_TIME_BASE"/>
    /// units.
    /// </summary>
    /// <param name="value">The timestamp to convert.</param>
    /// <returns>The time expressed in seconds.</returns>
    public static TimeExtent ToSeconds(this long value) =>
        value.ToSeconds(ffmpeg.AV_TIME_BASE);

}
