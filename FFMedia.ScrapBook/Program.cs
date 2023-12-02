using FFMedia.Extensions;
using FFMedia.Primitives;
using FFmpeg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using FFmpegBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace FFMedia.ScrapBook;

internal unsafe class Program
{
    private static int Result = 0;

    static void Main(string[] args)
    {
        TestStandalondeDI();
        return;

        FFmpegBindings.LibrariesPath = @"C:\ffmpeg\x64\";
        FFmpegBindings.Initialize();
        
        using var dict = new FFDictionary();

        dict["hello"] = "empty";
        dict["world"] = string.Empty;
        dict[""] = "page";

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

        Console.WriteLine($"Result: {Result}");
    }

    private static void TestStandalondeDI()
    {
        var services = new ServiceCollection();

        // TODO: not sure how to call the configure actions
        // see: https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting/src/HostBuilder.cs#L286
        services.AddLogging(configure =>
        {
            // Logging provider not yet working
            // See: https://learn.microsoft.com/en-us/dotnet/core/extensions/custom-logging-provider
            configure.ClearProviders();
            configure.AddProvider(FFLoggerProvider.Instance);
        });

        // Instead of providers, we just injet directly.
        services.AddSingleton<ILogger, FFLogger>((s) => FFLogger.Instance);

        var fac = new DefaultServiceProviderFactory();
        var provider = fac.CreateServiceProvider(services);
        
        var instance = ActivatorUtilities.CreateInstance<LoggerEnabled>(provider);

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

public class LoggerEnabled
{
    public LoggerEnabled(ILogger logger)
    {
        Logger = logger;
    }

    public ILogger Logger { get; }
}