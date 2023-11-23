namespace FFmpeg;

/// <summary>
/// A wapper for the <see cref="AVInputFormat"/>.
/// </summary>
/// <remarks>
/// Creates a new isntance of the <see cref="FFInputFormat"/> class.
/// </remarks>
/// <param name="target">The target to wrap.</param>
public unsafe sealed class FFInputFormat(AVInputFormat* target) :
    NativeReferenceBase<AVInputFormat>(target),
    IFFOptionsStorage
{
    private const StringSplitOptions SplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    public void x()
    {
        this.Target->cl
    }

    /// <summary>
    /// Gets the flags. Also see <see cref="AVInputFormat.flags"/>.
    /// </summary>
    public int Flags => Target->flags;

    /// <summary>
    /// Gets a comma separated list of short names for the format.
    /// New names may be appended with a minor bump in ffmpeg.
    /// </summary>
    public IReadOnlyList<string> ShortNames
    {
        get
        {
            if (Target is null)
                return [];

            var names = NativeExtensions.ReadString(Target->name);
            if (string.IsNullOrWhiteSpace(names))
                return [];

            return names.Split(',', SplitOptions);
        }

    }

    /// <summary>
    /// Tries to find an input format by its short name.
    /// </summary>
    /// <param name="shortName">The short name to search for.</param>
    /// <param name="format">The matching input format.</param>
    /// <returns>True if the search succeeds. False otherwise.</returns>
    public static bool TryFind(string shortName, [MaybeNullWhen(false)] out FFInputFormat format)
    {
        format = null;
        var pointer = ffmpeg.av_find_input_format(shortName);

        if (pointer is null)
            return false;

        format = new(pointer);
        return true;
    }
}
