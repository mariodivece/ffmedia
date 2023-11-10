
using FFMedia.ServiceModel;
using System.Threading.Channels;
using FFmpegBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace FFMedia.ScrapBook;

internal class Program
{
    static void Main(string[] args)
    {
        FFmpegBindings.LibrariesPath = @"C:\ffmpeg\x64\";
        FFmpegBindings.Initialize();

        var c = new MediaContainer();
        c.AddService<TimingService>();
        //c.AddService<NonService>();


        var channel = Channel.CreateUnbounded<string>(new()
        {
            AllowSynchronousContinuations = true,
            SingleReader = false,
            SingleWriter = false,
        });

        var t = new Thread(() => {
            var waitTask = channel.Reader.WaitToReadAsync(CancellationToken.None);
            if (!waitTask.IsCompleted)
                _ = waitTask.AsTask().GetAwaiter().GetResult();

            channel.Reader.TryRead(out var v);
            Console.WriteLine(v);
            
        })
        {
            IsBackground = true,
        };

        t.Start();
        Console.ReadKey();
        channel.Writer.Complete();
    }

    private class TimingService : ITimingService
    {
        public MediaContainer Container { get; private set; }

        public void Initialize(MediaContainer container)
        {
            Container = container;
        }
    }

    private class NonService :INonService 
    {
        public MediaContainer Container { get; private set; }

        public void Initialize(MediaContainer container)
        {
            Container = container;
        }
    }

    private interface INonService : IMediaContainerService { }
}