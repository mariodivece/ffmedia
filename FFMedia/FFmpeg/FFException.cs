namespace FFmpeg;

/// <summary>
/// Defines an exception based on FFmpeg's error codes.
/// </summary>
public class FFException : Exception
{
    private static readonly int DefaultErrorCode = ffmpeg.AVERROR(ffmpeg.EINVAL);

    /// <summary>
    /// Creates a new instance of the <see cref="FFException"/> class.
    /// </summary>
    public FFException()
        : this(DefaultErrorCode)
    {
        // placeholder
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FFException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FFException(string message)
        : this(DefaultErrorCode, message)
    {
        // placeholder
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FFException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public FFException(string message, Exception innerException)
        : this(DefaultErrorCode, message, innerException)
    {
        // placeholder
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FFException"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    public FFException(int errorCode)
       : base(DescribeError(errorCode))
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="FFException"/> class.
    /// </summary>
    /// <param name="errorCode">The rror code.</param>
    /// <param name="userMessage">The custom error message.</param>
    /// <param name="innerException">The optional inner exception.</param>
    public FFException(int errorCode, string userMessage, Exception? innerException = default)
        : base($"{userMessage}\r\n{nameof(FFmpeg)} error {errorCode}: {DescribeError(errorCode)}", innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// The FFmpeg error code.
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// Port of print_error. Gets a string representation of an FFmpeg error code.
    /// </summary>
    /// <param name="errorCode">The FFmpeg error code.</param>
    /// <returns>The text representation of the error code.</returns>
    public static unsafe string DescribeError(int errorCode)
    {
        const int BufferSize = 2048;
        var buffer = stackalloc byte[BufferSize];
        var foundError = ffmpeg.av_strerror(errorCode, buffer, BufferSize) == 0;
        var description = foundError ? NativeExtensions.ReadString(buffer) : string.Empty;
        return description ?? $"{nameof(FFmpeg)} error code ({errorCode})";
    }

    /// <summary>
    /// Throsw an exception if the result code is negative.
    /// </summary>
    /// <param name="resultCode">The ffmpeg error code.</param>
    /// <exception cref="FFException">The exception thrown.</exception>
    public static void ThrowIfNegative(int resultCode)
    {
        if (resultCode >= 0)
            return;

        throw new FFException(resultCode);
    }

    /// <summary>
    /// Throsw an exception if the result code is negative.
    /// </summary>
    /// <param name="resultCode">The ffmpeg error code.</param>
    /// <param name="userMessage">A custom error message.</param>
    /// <exception cref="FFException">The exception thrown.</exception>
    public static void ThrowIfNegative(int resultCode, string userMessage)
    {
        if (resultCode >= 0)
            return;

        throw new FFException(resultCode, userMessage);
    }
}
