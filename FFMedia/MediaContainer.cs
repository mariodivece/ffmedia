namespace FFMedia;

/// <summary>
/// Provides encapsulation of a multimedia steram and its services
/// which together, allow the end user to extract and render the contents
/// of such stream and issue standard playback commands.
/// </summary>
public partial class MediaContainer : IDisposable
{
    internal MediaContainer(MediaOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Provides access to this container's configuration options.
    /// </summary>
    public MediaOptions Options { get; }

    public void Open()
    {
        // TODO: Validate Services
        // TODO: Validate options


    }

}
