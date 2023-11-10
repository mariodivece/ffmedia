namespace FFMedia.Components;

internal interface IMediaComponent<TNative, TMedia> :
    ISerialGroupable
    where TNative : INativeFrame
    where TMedia : IMediaFrame
{
    AVMediaType MediaType { get; }

    PacketStore Packets { get; }

    FrameStore<TNative, TMedia> Frames { get; }

}
