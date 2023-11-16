namespace FFMedia.Components;

public interface IFramePool<TMedia> :
    IDisposable
    where TMedia: class, IMediaFrame
{
    bool TryLease([MaybeNullWhen(false)]out TMedia? frame);


}
