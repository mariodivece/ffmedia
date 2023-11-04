namespace FFMedia.ServiceModel;

public interface IMediaContainerService
{
    void Initialize(MediaContainer container);
    
    MediaContainer Container { get; }
}
