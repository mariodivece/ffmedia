using Microsoft.Extensions.DependencyInjection;

namespace FFMedia;

/// <summary>
/// Provides encapsulation of a multimedia steram and its services
/// which together, allow the end user to extract and render the contents
/// of such stream and issue standard playback commands.
/// </summary>
public partial class MediaContainer : IDisposable
{
    public MediaContainer(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Options = ActivatorUtilities.CreateInstance<IMediaOptions>(serviceProvider) as MediaContainerOptions ??
            throw new InvalidCastException($"{nameof(IMediaOptions)} must be of type {nameof(MediaContainerOptions)}");

        // TODO: also can be:
        // Options = ServiceProvider.GetRequiredService<IMediaOptions>();

    }

    public async Task OpenAsync(string url)
    {
        await Task.Delay(1000).ConfigureAwait(false);
    }

    private IServiceProvider ServiceProvider { get; }

    internal MediaContainerOptions Options { get; }
}
