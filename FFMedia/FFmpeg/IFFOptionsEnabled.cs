namespace FFmpeg;

/// <summary>
/// Provides member definitions for data structures that
/// can store options and change options. The <see cref="MediaClass"/>
/// defines all possible options and children, while the <see cref="CurrentOptions"/>
/// allow the user to access and change option data for this instance.
/// It is recommended that you use the <see cref="FFOptionsStore"/> class
/// to quickly implement <see cref="AVOption"/>-enabled structures.
/// Only those structures which define their first members to be
/// <see cref="AVClass"/> are supported.
/// </summary>
public interface IFFOptionsEnabled :
    INativeReference
{
    /// <summary>
    /// Gets the media class that defines all possible options and
    /// child option objects.
    /// </summary>
    FFMediaClass MediaClass { get; }

    /// <summary>
    /// Gets the currently stored options in the object.
    /// Does not include options stored in child objects.
    /// </summary>
    IReadOnlyList<FFOption> CurrentOptions { get; }

    /// <summary>
    /// Gets the child options-enabled object currently associated with
    /// this object.
    /// </summary>
    IReadOnlyList<IFFOptionsEnabled> CurrentChildren { get; }
}
