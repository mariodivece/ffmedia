using FFMedia.Primitives;
using FFmpeg;
using FFmpeg.AutoGen.Abstractions;
using System.Diagnostics;
using FFmpegBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace FFMedia.ScrapBook;

internal unsafe class Program
{
    private static int Result = 0;

    static void Main(string[] args)
    {
        FFmpegBindings.LibrariesPath = @"C:\ffmpeg\x64\";
        FFmpegBindings.Initialize();

        var options = FFMediaClass.Format.Options;
        var fp = FFMediaClass.Format.FindOption("max_ts_probe", true);
        var fp2 = options.FirstOrDefault(o => o.Name == "max_ts_probe");

        var format = ffmpeg.avformat_alloc_context();

        var mediaClass = new FFMediaClass(format->av_class);

        var setResult = ffmpeg.av_opt_set(format->priv_data, "max_ts_probe", "1", 0);

        var mcOpts = mediaClass.Options;
        var mcChildren = mediaClass.Children;


        Console.WriteLine($"Result: {Result}");
        ffmpeg.avformat_free_context(format);
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