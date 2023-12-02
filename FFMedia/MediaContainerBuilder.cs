using Microsoft.Extensions.DependencyInjection;

namespace FFMedia;

public class MediaContainerBuilder
{
    public IServiceCollection Services { get; } = new ServiceCollection();

    public async Task<MediaContainer> OpenAsync(Uri uri)
    {
        var providerFactory = new DefaultServiceProviderFactory();
        var provider = providerFactory.CreateServiceProvider(Services);
        var container = new MediaContainer(provider);
        await container.OpenAsync(uri.ToString()).ConfigureAwait(false);
        return container;
    }

}
