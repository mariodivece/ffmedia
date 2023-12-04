using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace FFMedia.Extensions;

public static class LoggingExtensions
{
    private static readonly FrozenDictionary<int, LogLevel> NativeToManaged = new Dictionary<int, LogLevel>
    {
        { ffmpeg.AV_LOG_TRACE, LogLevel.Trace },
        { ffmpeg.AV_LOG_DEBUG, LogLevel.Debug },
        { ffmpeg.AV_LOG_VERBOSE, LogLevel.Debug },
        { ffmpeg.AV_LOG_INFO, LogLevel.Information },
        { ffmpeg.AV_LOG_WARNING, LogLevel.Warning },
        { ffmpeg.AV_LOG_ERROR, LogLevel.Error },
        { ffmpeg.AV_LOG_FATAL, LogLevel.Critical },
        { ffmpeg.AV_LOG_PANIC, LogLevel.Critical }
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<LogLevel, int> ManagedToNative = new Dictionary<LogLevel, int>
    {
        { LogLevel.Trace, ffmpeg.AV_LOG_TRACE },
        { LogLevel.Debug, ffmpeg.AV_LOG_VERBOSE },
        { LogLevel.Information, ffmpeg.AV_LOG_INFO },
        { LogLevel.Warning, ffmpeg.AV_LOG_WARNING },
        { LogLevel.Error, ffmpeg.AV_LOG_ERROR },
        { LogLevel.Critical, ffmpeg.AV_LOG_PANIC },
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a native FFmpeg log level to a managed <see cref="LogLevel"/>.
    /// See AV_LOG_* values.
    /// </summary>
    /// <param name="nativeLogLevel">The integer representing the log level.</param>
    /// <returns>The managed log level.</returns>
    public static LogLevel ToManagedLogLevel(this int nativeLogLevel) =>
        !NativeToManaged.TryGetValue(nativeLogLevel, out var level)
            ? LogLevel.Information
            : level;

    /// <summary>
    /// Converts a managed <see cref="LogLevel"/> to a native log level.
    /// See AV_LOG_* values.
    /// </summary>
    /// <param name="managedLogLevel">The managed log level.</param>
    /// <returns>The native log level.</returns>
    public static int ToNativeLogLevel(this LogLevel managedLogLevel) =>
        !ManagedToNative.TryGetValue(managedLogLevel, out var level)
            ? ffmpeg.AV_LOG_INFO
            : level;
}
