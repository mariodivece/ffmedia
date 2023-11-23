namespace FFmpeg;

/// <summary>
/// Defines the members of a <see cref="AVOption"/>s-enabled object.
/// </summary>
public interface IFFOptionsDefined : INativeReference
{
    /// <summary>
    /// Gets the child <see cref="IFFOptionsDefined"/> objects.
    /// </summary>
    public IReadOnlyList<IFFOptionsDefined> Children { get; }

    /// <summary>
    /// Enumerates all the options for this <see cref="IFFOptionsDefined"/> class.
    /// </summary>
    IReadOnlyList<FFOption> Options { get; }

    /// <summary>
    /// Attempts to find an option with the specified name.
    /// </summary>
    /// <param name="optionName">The name of the option to search for.</param>
    /// <param name="searchChildren">Whether to search for the option in child objects.</param>
    /// <returns>The the option if it is found. Null otherwise.</returns>
    /// <remarks>Port of cmdutils.c/opt_find and based on libavutil/opt.c.</remarks>
    FFOption? FindOption(string optionName, bool searchChildren);

    /// <summary>
    /// Checks whether an option with the specified name exists.
    /// </summary>
    /// <param name="optionName">The name of the option to search for.</param>
    /// <param name="searchChildren">Whether to search for the option in child objects.</param>
    /// <returns>True if the option is found. False otherwise.</returns>
    bool HasOption(string optionName, bool searchChildren);
}
