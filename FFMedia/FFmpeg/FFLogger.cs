using Microsoft.Extensions.Logging;
namespace FFmpeg;

/// <summary>
/// Singleton that implements FFmpeg logging.
/// </summary>
public sealed unsafe class FFLogger : ILogger
{
    private readonly object SyncRoot = new();
    private readonly av_log_set_callback_callback NativeLogCallback;

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
    /// Gets or sets event handlers for <see cref="FFLoggerEventArgs"/>.
    /// If not handlers exists, the dedefault <see cref="ffmpeg.av_log_default_callback(void*, int, string, byte*)"/>
    /// is called when messages are logged to the ffmpeg library.
    /// </summary>
    public event EventHandler<FFLoggerEventArgs>? OnMessageLogged;

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
                return ffmpeg.av_log_get_level().ToManagedLogLevel();
        }
        set
        {
            lock (SyncRoot)
                ffmpeg.av_log_set_level(value.ToNativeLogLevel());
        }
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (SyncRoot)
        {
            var message = formatter?.Invoke(state, exception);
            var nativeLevel = logLevel.ToNativeLogLevel();
            ffmpeg.av_log(null, nativeLevel, message);
        }
    }

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

    /// <summary>
    /// Called when <see cref="ffmpeg.av_log"/> is called.
    /// </summary>
    /// <param name="avClass">The options-enable dobject.</param>
    /// <param name="nativeLevel">The native log level.</param>
    /// <param name="messageFormat">The message to format.</param>
    /// <param name="variableArgs">The native variable arguments.</param>
    private void NativeLogHandler(void* avClass, int nativeLevel, string messageFormat, byte* variableArgs)
    {
        const int LineSize = 1024;
        
        lock (SyncRoot)
        {
            var externalHandler = OnMessageLogged;
            if (externalHandler is null)
            {
                ffmpeg.av_log_default_callback(avClass, nativeLevel, messageFormat, variableArgs);
                return;
            }

            var messageOutput = stackalloc byte[LineSize];
            int printPrefix;

            ffmpeg.av_log_format_line(avClass, nativeLevel, messageFormat, variableArgs, messageOutput, LineSize, &printPrefix);
            var message = NativeExtensions.ReadString(messageOutput) ?? string.Empty;
            var optionsEnabled = avClass is not null
                ? new FFOptionsStore(avClass)
                : FFOptionsStore.Empty;

            externalHandler.Invoke(this, new(optionsEnabled, nativeLevel.ToManagedLogLevel(), message));
        }
    }
}