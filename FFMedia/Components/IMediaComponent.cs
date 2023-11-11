namespace FFMedia.Components;

public interface IMediaComponent<TMedia> :
    ISerialGroupable,
    IDisposable
    where TMedia : class, IMediaFrame
{
    AVMediaType MediaType { get; }

    PacketStore Packets { get; }

    FrameStore<TMedia> Frames { get; }
}
