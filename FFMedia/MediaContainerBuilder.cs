namespace FFMedia;

public class MediaContainerBuilder
{
    private MediaOptions? Options;

    public MediaContainerBuilder WithOptions(MediaOptions options)
    {
        Options = options;
        return this;
    }

    public MediaContainer Open(Uri uri)
    {
        var container = new MediaContainer(Options ?? new());
        container.Open();

        return container;
    }

}
