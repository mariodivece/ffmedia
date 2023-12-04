using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
namespace FFmpeg;

/// <summary>
/// Singleton that implements FFmpeg logging.
/// </summary>
public sealed unsafe class FFLogger : ILogger
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

    private readonly object SyncRoot = new();
    private readonly av_log_set_callback_callback NativeLogCallback;
    private LogCallback? _OnMessageLogged;

    /// <summary>
    /// Gets the signleton instance of the FFmpeg logging subsystem.
    /// </summary>
    public static FFLogger Instance { get; } = new FFLogger();

    /// <summary>
    /// Prevents external initialization.
    /// </summary>
    private FFLogger()
    {
        lock (SyncRoot)
        {
            SkipRepeated = true;
            Level = LogLevel.Debug;
            NativeLogCallback = new(NativeLogHandler);
            ffmpeg.av_log_set_callback(NativeLogCallback);
        }
    }

    /// <summary>
    /// Gets or sets a vaqlue indicating whether this logger
    /// will skip repeated messages.
    /// </summary>
    public bool SkipRepeated
    {
        get
        {
            lock (SyncRoot)
                return ffmpeg.av_log_get_flags().HasFlag(ffmpeg.AV_LOG_SKIP_REPEATED);
        }
        set
        {
            lock (SyncRoot)
                ffmpeg.av_log_set_flags(ffmpeg.av_log_get_flags().SetFlag(value ? ffmpeg.AV_LOG_SKIP_REPEATED : default));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the logging level.
    /// </summary>
    public LogLevel Level
    {
        get
        {
            lock (SyncRoot)
                return ToManagedLogLevel(ffmpeg.av_log_get_level());
        }
        set
        {
            lock (SyncRoot)
                ffmpeg.av_log_set_level(ToNativeLogLevel(value));
        }
    }

    public LogCallback? OnMessageLogged
    {
        get
        {
            lock (SyncRoot)
                return _OnMessageLogged;
        }
        set
        {
            lock (SyncRoot)
                _OnMessageLogged = value;
        }
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
            return false;

        lock (SyncRoot)
        {
            var requestedLevel = (int)logLevel;
            var currentLevel = (int)Level;

            return currentLevel >= requestedLevel;
        }
    }

    private static int ToNativeLogLevel(LogLevel managedLogLevel) =>
        !ManagedToNative.TryGetValue(managedLogLevel, out var level)
            ? ffmpeg.AV_LOG_INFO
            : level;

    private static LogLevel ToManagedLogLevel(int nativeLogLevel) =>
        !NativeToManaged.TryGetValue(nativeLogLevel, out var level)
            ? LogLevel.Information
            : level;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (SyncRoot)
        {
            var message = formatter?.Invoke(state, exception);
            var level = ToNativeLogLevel(logLevel);
            ffmpeg.av_log(null, level, message);
        }
    }

    private void NativeLogHandler(void* avClass, int level, string messageFormat, byte* variableArgs)
    {
        const int LineSize = 1024;

        lock (SyncRoot)
        {
            var externalHandler = OnMessageLogged;
            if (externalHandler is null)
            {
                ffmpeg.av_log_default_callback(avClass, level, messageFormat, variableArgs);
                return;
            }

            var messageOutput = stackalloc byte[LineSize];
            int printPrefix;

            ffmpeg.av_log_format_line(avClass, level, messageFormat, variableArgs, messageOutput, LineSize, &printPrefix);
            var message = NativeExtensions.ReadString(messageOutput) ?? string.Empty;
            var optionsEnabled = avClass is not null
                ? new FFOptionsStore(avClass)
                : FFOptionsStore.Empty;

            externalHandler(optionsEnabled, ToManagedLogLevel(level), message);
        }
    }
}