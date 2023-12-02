using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using System.Xml.Linq;

namespace FFmpeg;

public sealed class FFLoggerConfiguration
{
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

public sealed class FFLogger : ILogger
{
    private static FrozenDictionary<int, LogLevel> NativeToManaged = new Dictionary<int, LogLevel>
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

    private static FrozenDictionary<LogLevel, int> ManagedToNative = new Dictionary<LogLevel, int>
    {
        { LogLevel.Trace, ffmpeg.AV_LOG_TRACE },
        { LogLevel.Debug, ffmpeg.AV_LOG_VERBOSE },
        { LogLevel.Information, ffmpeg.AV_LOG_INFO },
        { LogLevel.Warning, ffmpeg.AV_LOG_WARNING },
        { LogLevel.Error, ffmpeg.AV_LOG_ERROR },
        { LogLevel.Critical, ffmpeg.AV_LOG_PANIC },
    }.ToFrozenDictionary();

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }

    public FFLoggerConfiguration Configuration { get; } = new();

    public static FFLogger Instance { get; } = new FFLogger();
}