using Microsoft.Extensions.Logging;

namespace FFmpeg;

/// <summary>
/// Event arguments for custom message FFmpeg handling.
/// </summary>
public sealed class FFLoggerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFLoggerEventArgs"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="optionsObject">The options object.</param>
    /// <param name="logLevel">The log level.</param>
    public FFLoggerEventArgs(IFFOptionsEnabled optionsObject, LogLevel logLevel, string message)
        : base()
    {
        Message = message ?? string.Empty;
        OptionsObject = optionsObject ?? FFOptionsStore.Empty;
        LogLevel = logLevel;
    }

    /// <summary>
    /// Gets the log level for the logger event.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets the options-enabled object this message belongs to.
    /// </summary>
    public IFFOptionsEnabled OptionsObject { get; }

    /// <summary>
    /// Gets the logging message.
    /// </summary>
    public string Message { get; }
}
