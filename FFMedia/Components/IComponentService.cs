namespace FFMedia.Components;

/// <summary>
/// Provides member definitions for classes that are part of a <see cref="IMediaComponent{TMedia}"/>
/// </summary>
public interface IComponentService<TMedia>
    where TMedia : class, IMediaFrame
{
    /// <summary>
    /// Provides access to the media component this service belongs to.
    /// </summary>
    IMediaComponent<TMedia> Component { get; }
}
