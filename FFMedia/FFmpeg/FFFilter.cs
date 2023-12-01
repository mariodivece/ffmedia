using System.IO.MemoryMappedFiles;

namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for the <see cref="AVFilter"/> data structure.
/// These filters serve as a kind of prototype for <see cref="FFFilterContext"/>.
/// </summary>
public unsafe sealed class FFFilter :
    NativeReferenceBase<AVFilter>
{
    private FFFilter(AVFilter* pointer)
        : base(pointer)
    {
        // placeholder
    }

    /// <summary>
    /// Gets the filter name.
    /// </summary>
    public string Name => NativeExtensions.ReadString(Target->name) ?? string.Empty;

    /// <summary>
    /// Gets the filter description.
    /// </summary>
    public string Description => NativeExtensions.ReadString(Target->description) ?? string.Empty;

    /// <summary>
    /// Gets the AVFILTER_FLAG_* flags.
    /// </summary>
    public int Flags => Target->flags;

    /// <summary>
    /// Gets the private media class for this filter.
    /// </summary>
    public FFMediaClass MediaClass => new(Target->priv_class);

    /// <summary>
    /// Attempts to create a filter reference from the given filter name.
    /// </summary>
    /// <param name="filterName">The filter name.</param>
    /// <param name="filter">The filter, if found.</param>
    /// <returns>True on success. False on failure.</returns>
    public static bool TryFind(string filterName, [MaybeNullWhen(false)] out FFFilter filter)
    {
        filter = null;
        if (string.IsNullOrWhiteSpace(filterName))
            return false;

        var pointer = ffmpeg.avfilter_get_by_name(filterName);
        filter = pointer is not null
            ? new FFFilter(pointer)
            : default;

        return filter is not null;
    }
}
