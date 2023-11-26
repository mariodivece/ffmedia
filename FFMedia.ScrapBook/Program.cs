using FFMedia.FFmpeg;
using FFMedia.Primitives;
using FFmpeg;
using FFmpeg.AutoGen.Abstractions;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
using FFmpegBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace FFMedia.ScrapBook;

internal unsafe class Program
{
    private static int Result = 0;

    static void Main(string[] args)
    {
        FFmpegBindings.LibrariesPath = @"C:\ffmpeg\x64\";
        FFmpegBindings.Initialize();

        var n = FFMediaClass.Format;

        using var context = new DummyContext();


        Console.WriteLine($"Result: {Result}");
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

public unsafe class DummyContext : NativeTrackedReferenceBase<AVCodecContext>
{
    public DummyContext()
        : base(null, null)
    {
        var h264Codecs = FFCodec.Codecs.Where(c => c.Name.Contains("x264")).ToArray();
        var codec = h264Codecs.First();
        var inputContext = ffmpeg.avcodec_alloc_context3(codec!.Target);

        Update(inputContext);
    }

    public FFOptionsWrapper? Options => FFOptionsWrapper.TryWrap(this, out var options) ? options : null;

    protected override unsafe void ReleaseInternal(AVCodecContext* target)
    {
        ffmpeg.avcodec_free_context(&target);
    }
}