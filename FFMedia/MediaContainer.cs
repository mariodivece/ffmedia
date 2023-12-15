using Microsoft.Extensions.DependencyInjection;

namespace FFMedia;

/// <summary>
/// Provides encapsulation of a multimedia steram and its services
/// which together, allow the end user to extract and render the contents
/// of such stream and issue standard playback commands.
/// </summary>
public partial class MediaContainer : IDisposable
{
    internal MediaContainer(IServiceCollection services)
    {
        var importedServices = services is not null && services.Count > 0
            ? [.. services]
            : Array.Empty<ServiceDescriptor>();
        
        IServiceCollection localServices = new ServiceCollection();
        foreach (var service in importedServices)
            localServices.Add(service);

        // Ensure this media container is available as a service to child classes.
        localServices.AddSingleton((p) => this);

        var providerFactory = new DefaultServiceProviderFactory();
        ServiceProvider = providerFactory.CreateServiceProvider(localServices);
        
        // TODO: also can be: Options = ServiceProvider.GetRequiredService<IMediaOptions>();
        Options = ActivatorUtilities.CreateInstance<IMediaOptions>(ServiceProvider) as MediaContainerOptions ??
            throw new InvalidCastException($"{nameof(IMediaOptions)} must be of type {nameof(MediaContainerOptions)}");
    }

    public async Task OpenAsync(string url)
    {
        await Task.Delay(1000).ConfigureAwait(false);
    }

    private IServiceProvider ServiceProvider { get; }

    internal MediaContainerOptions Options { get; }
}
