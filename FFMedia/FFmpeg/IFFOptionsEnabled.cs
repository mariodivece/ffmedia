namespace FFMedia.FFmpeg;

/// <summary>
/// Provides member definitions for data structures that
/// can store options and change options. The <see cref="MediaClass"/>
/// defines all possible options and children, while the <see cref="Options"/>
/// allow the user to access and change option data for this instance.
/// It is recommended that you use the <see cref="FFOptionsWrapper"/>
/// to quickly implement <see cref="AVOption"/>-enabled structures.
/// Only those structures which define their first members to be
/// <see cref="AVClass"/> are supported.
/// </summary>
public interface IFFOptionsEnabled
{
    /// <summary>
    /// Gets the media class that defines all possible options and
    /// child option objects.
    /// </summary>
    public FFMediaClass MediaClass { get; }

    /// <summary>
    /// Gets a proxy to read and write instance options
    /// for this opbject.
    /// </summary>
    public FFOptionsWrapper Options { get; }
}
