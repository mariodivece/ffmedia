namespace FFMedia.Components;

public interface IMediaFrame : ISerialGroupable, IDisposable
{
    AVMediaType MediaType { get; }

    TimeExtent StartTime { get; }

    TimeExtent Duration { get; }
}
