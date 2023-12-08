using FFMedia.Components;
using FFMedia.Engine;
using FFMedia.Extensions;
using FFMedia.Primitives;
using FFmpeg;
using FFmpeg.AutoGen.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using FFmpegBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace FFMedia.ScrapBook;

internal unsafe class Program
{
    private static int Result = 0;

    static void Main(string[] args)
    {
        TestBinaryLookup();
        TestFFLogger();
        Console.WriteLine($"Result: {Result}");
    }

    private static void InitFFmpeg()
    {
        FFmpegBindings.LibrariesPath = @"C:\ffmpeg\x64\";
        FFmpegBindings.Initialize();
    }

    private static void TestFFLogger()
    {
        InitFFmpeg();
        var logger = FFLogger.Instance;
        logger.OnMessageLogged += (s, e) =>
        {
            Console.WriteLine($"{e.OptionsObject.Address}: ({e.LogLevel}) {e.Message}");
        };

        ffmpeg.av_log(null, ffmpeg.AV_LOG_INFO, $"And this is some maeesage from the direct API call");
        logger.LogInformation("This is some cool stuff: {myMessage}", "Says Mario!");
    }

    private static void TestMediaOptions()
    {
        var options = new MediaContainerOptions() as IMediaOptions;
        dynamic dynamicOptions = options.AsDynamic();

        dynamicOptions.FrameSizes = new
        {
            Video = 16,
            Audio = 32,
            Subtitle = 64
        };

        var audio = (double)dynamicOptions.FrameSizes.Audio;
        var nonExistent = dynamicOptions.FrameSizes.NonExistent;
    }

    private static void TestDictionaries()
    {
        InitFFmpeg();

        using var dict = new FFDictionary
        {
            ["hello"] = "empty",
            ["world"] = string.Empty,
            [""] = "page"
        };

        dict.Remove(string.Empty);
        dict.Remove("hello");
        dict.Remove("world");

        dict.Add("Mexico", "Again");
        dict["Mexico"] = "Changed String";
        dict.Clear();
        dict["Mario"] = "Di Vece";
        dict.Remove("Mario");

        var n = FFMediaClass.Format;

        using var context = new FFCodecContext();
        var optionValue = context.GetOptionValue("bt", true);
        context.SetOptionValue("bt", true, "50000");
        optionValue = context.GetOptionValue("bt", true);
        Debug.Assert("50000".Equals(optionValue));
    }

    private static void TestStandalondeDI()
    {
        var builder = new ContainerBuilder();
        builder.Services.AddLogging();
        builder.Logging.ClearProviders();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ColorLoggerProvider<GreenLogger>>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ColorLoggerProvider<BlueLogger>>());

        var container = builder.Build();

        var logger = container.GetRequiredService<ILogger<LoggerInjected>>();
        var injectedInstance = ActivatorUtilities.CreateInstance<LoggerInjected>(container, @"c:\ffmpeg\x64\");
    }

    private static void TestBinaryLookup()
    {
        const int ItemCount = 4000;
        var itemList = new List<int>(ItemCount);
        while (itemList.Count < ItemCount)
        {
            var nextItem = Random.Shared.Next(0, ItemCount) * 5;
            if (itemList.Contains(nextItem))
                continue;

            itemList.Add(nextItem);
        }

        var items = itemList.ToArray();
        Array.Sort(items);

        var lookupValue = 57;

        var t2 = new TimeExtent[] { 1, 2, 3, 4 };
        t2.TrySlidingValueSearch(2, out var cv);

        var success = items.TrySlidingValueSearch(lookupValue, out var result);

        lookupValue = 4049;
        success = items.TrySlidingValueSearch(lookupValue, out result);

        lookupValue = 0;
        success = items.TrySlidingValueSearch(lookupValue, out result);

        lookupValue = 1;
        success = items.TrySlidingValueSearch(lookupValue, out result);

        lookupValue = -1;
        success = items.TrySlidingValueSearch(lookupValue, out result);

        lookupValue = items.Last();
        success = items.TrySlidingValueSearch(lookupValue, out result);

        lookupValue = items.Last() + 1;
        success = items.TrySlidingValueSearch(lookupValue, out result);
    }

    private static void TaskBody(ExclusiveLock exclusive, string name)
    {
        var startTime = Stopwatch.GetTimestamp();
        var entryCount = 0;
        var denyCount = 0;

        while (Stopwatch.GetElapsedTime(startTime).TotalSeconds < 3)
        {
            using var locker = exclusive.TryLock();
            entryCount += locker is not null ? 1 : 0;
            denyCount += locker is null ? 1 : 0;

            if (locker is not null)
                Thread.Sleep(1);
        }

        Console.WriteLine($"{name,-16} Entry: {entryCount,8}, Deny: {denyCount,8} ");
    }

    private static void TaskBody2(ExclusiveLock exclusive, string name)
    {
        var entryCount = 0;
        var denyCount = 0;

        while (entryCount < 10)
        {
            using var locker = exclusive.TryLock();
            if (locker is not null)
            {
                Result++;
                entryCount++;
            }
        }

        Console.WriteLine($"{name,-16} Entry: {entryCount,8}, Deny: {denyCount,8} ");
    }
}

public class ContainerBuilder
{
    public ContainerBuilder()
    {
        Services = new ServiceCollection();
        Logging = new LoggingBuilder(Services);
    }

    public IServiceCollection Services { get; }

    public ILoggingBuilder Logging { get; }

    public IServiceProvider Build()
    {
        var factory = new DefaultServiceProviderFactory();
        return factory.CreateServiceProvider(Services);
    }
}

public class LoggerInjected
{
    public LoggerInjected(string filePath, ILogger<LoggerInjected>? logger)
    {
        Logger = logger;
        Logger?.Log(LogLevel.Information, "Hello {date} - The File path is {filePath}", DateTime.UtcNow, filePath);
    }

    public ILogger? Logger { get; }
}

public class LoggingBuilder : ILoggingBuilder
{
    public static readonly object SyncRoot = new();

    public LoggingBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}

public sealed class ColorLoggerProvider<T> : ILoggerProvider
    where T : ColorLoggerBase, new()
{
    private readonly ConcurrentDictionary<Type, ILogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(typeof(T), new T());

    public void Dispose()
    {
        // do nothing.
    }
}

public abstract class ColorLoggerBase : ILogger
{
    public abstract ConsoleColor Color { get; }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (LoggingBuilder.SyncRoot)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}] - {formatter(state, exception)}");
            Console.ForegroundColor = originalColor;
        }
    }
}

public class GreenLogger : ColorLoggerBase
{
    public override ConsoleColor Color => ConsoleColor.Green;
}

public class BlueLogger : ColorLoggerBase
{
    public override ConsoleColor Color => ConsoleColor.Blue;
}