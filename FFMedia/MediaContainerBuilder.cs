﻿using Microsoft.Extensions.DependencyInjection;
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
        var container = new MediaContainer(Services);
        await container.OpenAsync(uri.ToString()).ConfigureAwait(false);
        return container;
    }


    /// <summary>
    /// An implementation for <see cref="ILoggingBuilder"/>.
    /// </summary>
    private sealed class LoggingBuilder(IServiceCollection services) : ILoggingBuilder
    {
        public IServiceCollection Services { get; } = services;
    }

}
