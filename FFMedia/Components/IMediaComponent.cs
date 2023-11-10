namespace FFMedia.Components;

internal interface IMediaComponent : ISerialGroupable
{
    PacketStore Packets { get; }

    FrameStore Frames { get; }

}
