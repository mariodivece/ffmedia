using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FFMedia;

/// <summary>
/// 
/// </summary>
public class MediaContainerBuilder
{
    public MediaContainerBuilder()
    {
        Services = new ServiceCollection();
        Logging = new LoggingBuilder(Services);
    }

    public IServiceCollection Services { get; }

    public ILoggingBuilder Logging { get; }

    public async Task<MediaContainer> OpenAsync(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        var providerFactory = new DefaultServiceProviderFactory();
        var provider = providerFactory.CreateServiceProvider(Services);
        var container = new MediaContainer(provider);
        await container.OpenAsync(uri.ToString()).ConfigureAwait(false);
        return container;
    }


    /// <summary>
    /// An implementation for <see cref="ILoggingBuilder"/>.
    /// </summary>
    private sealed class LoggingBuilder : ILoggingBuilder
    {
        public static readonly object SyncRoot = new();

        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

}
