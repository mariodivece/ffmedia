namespace FFMedia.Extensions;

internal static class RationalExtensions
{
    /// <summary>
    /// Defines the maximum integer value to represent either a numerator or denominator when
    /// convertirn from a double to a rational number.
    /// </summary>
    public const int MaxRationalComponentMagnitude = 1000000000;

    /// <summary>
    /// Defines a <see cref="AVRational"/> whose numerator is 0 and denominator is 1.
    /// </summary>
    public static readonly AVRational UndefinedValue = new() { num = 0, den = 1 };

    /// <summary>
    /// Defines a <see cref="AVRational"/> whose numerator is 1 and denominator is 1.
    /// </summary>
    public static readonly AVRational OneValue = new() { num = 1, den = 1 };

    /// <summary>
    /// Checks if the denominator is not 0.
    /// </summary>
    /// <param name="r">The value to check.</param>
    /// <returns>True if the rational number is valid. False otherwise.</returns>
    public static bool IsValid(this AVRational r) => r.den != 0;

    /// <summary>
    /// Checks if both the numerator and the denominator is not 0.
    /// </summary>
    /// <param name="r">The value to check.</param>
    /// <returns>True if the rational number is valid. False otherwise.</returns>
    public static bool HasValue(this AVRational r) => r.den != 0 && r.num != 0;

    /// <summary>
    /// Converts a rational to a double.
    /// </summary>
    /// <remarks>
    /// See <see cref="ffmpeg.av_q2d(AVRational)"/>.
    /// </remarks>
    /// <param name="r">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static double ToDouble(this AVRational r) => ffmpeg.av_q2d(r);

    /// <summary>
    /// Converts a double to a rational number.
    /// Non-finite numbers will be converted to <see cref="UndefinedValue"/>.
    /// </summary>
    /// <param name="d">The value to convert.</param>
    /// <param name="maxMagnitude"></param>
    /// <returns>The rational number.</returns>
    public static AVRational ToRational(this double d, int maxMagnitude = MaxRationalComponentMagnitude) => double.IsFinite(d)
        ? ffmpeg.av_d2q(d, maxMagnitude)
        : UndefinedValue;

    /// <summary>
    /// Reduces the rational number to a normalized value by converting it to a double
    /// and then convertin that double back to a rational number.
    /// </summary>
    /// <param name="r">The rational to normalize.</param>
    /// <returns>The rational number.</returns>
    public static AVRational Normalize(this AVRational r) => r.IsValid()
        ? r.ToDouble().ToRational()
        : UndefinedValue;

    /// <summary>
    /// Converts the rational number components to their absolute values.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    public static AVRational Abs(this AVRational value) =>
        new() { num = Math.Abs(value.num), den = Math.Abs(value.den) };
}
