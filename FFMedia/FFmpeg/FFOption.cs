namespace FFmpeg;

/// <summary>
/// Represents a wrapper for the <see cref="AVOption"/> structure.
/// </summary>
/// <param name="target"></param>
public unsafe sealed class FFOption(AVOption* target) :
    NativeReferenceBase<AVOption>(target)
{
    /// <summary>
    /// Gets the option type.
    /// </summary>
    public AVOptionType Type => Target->type;

    /// <summary>
    /// Gets the name of this option.
    /// </summary>
    public string Name => NativeExtensions.ReadString(Target->name) ?? string.Empty;

    /// <summary>
    /// Gets the help text in English for this option.
    /// </summary>
    public string Help => NativeExtensions.ReadString(Target->help) ?? string.Empty;

    /// <summary>
    /// Gets the logical unit to which this option belongs.
    /// Non-constant options and corresponding named constants share the same unit.
    /// </summary>
    public string Unit => NativeExtensions.ReadString(Target->unit) ?? string.Empty;

    /// <summary>
    /// Gets the option flags.
    /// </summary>
    public int Flags => Target->flags;

    /// <summary>
    /// Gets the minimum valid value for the option
    /// </summary>
    public double MinValue => Target->min;

    /// <summary>
    /// Gets the maximum valid value for the option
    /// </summary>
    public double MaxValue => Target->min;

}
