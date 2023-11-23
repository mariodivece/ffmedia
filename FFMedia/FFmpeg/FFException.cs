namespace FFmpeg;

/// <summary>
/// Defines an exception based on FFmpeg's error codes.
/// </summary>
public class FFException : Exception
{
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

    public static void ThrowIfNegative(int resultCode)
    {
        if (resultCode > 0)
            return;

        throw new FFException(resultCode);
    }

    public static void ThrowIfNegative(int resultCode, string userMessage)
    {
        if (resultCode > 0)
            return;

        throw new FFException(resultCode, userMessage);
    }
}
