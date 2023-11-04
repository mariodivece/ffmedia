namespace FFMedia;

/// <summary>
/// Provides encapsulation of a multimedia steram and its services
/// which together, allow the end user to extract and render the contents
/// of such stream and issue standard playback commands.
/// </summary>
public partial class MediaContainer : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaContainer"/> class.
    /// </summary>
    public MediaContainer()
    {
        ServiceProperties = new(BuildServiceProperties, false);

    }

    public void Open()
    {
        // TODO: Validate Services
        // TODO: Validate options


    }

}
