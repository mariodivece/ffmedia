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
    public AVOptionType Type => Target is not null
        ? Target->type : AVOptionType.AV_OPT_TYPE_FLAGS;

    /// <summary>
    /// Gets the name of this option.
    /// </summary>
    public string Name => Target is not null
        ? NativeExtensions.ReadString(Target->name) ?? string.Empty : string.Empty;

    /// <summary>
    /// Gets the help text in English for this option.
    /// </summary>
    public string Help => Target is not null
        ? NativeExtensions.ReadString(Target->help) ?? string.Empty : string.Empty;

    /// <summary>
    /// Gets the logical unit to which this option belongs.
    /// Non-constant options and corresponding named constants share the same unit.
    /// </summary>
    public string Unit => Target is not null
        ? NativeExtensions.ReadString(Target->unit) ?? string.Empty : string.Empty;

    /// <summary>
    /// Gets the option flags.
    /// </summary>
    public int Flags => Target is not null ? Target->flags : 0;

    /// <summary>
    /// Gets the minimum valid value for the option
    /// </summary>
    public double MinValue => Target is not null ? Target->min : default;

    /// <summary>
    /// Gets the maximum valid value for the option
    /// </summary>
    public double MaxValue => Target is not null ? Target->min : default;

    /// <summary>
    /// Gets the default value defined for this option.
    /// </summary>
    public string? DefaultStringValue => Target is not null
        ? NativeExtensions.ReadString(Target->default_val.str) : default;

    /// <summary>
    /// Gets the default value defined for this option.
    /// </summary>
    public long DefaultLongValue => Target is not null ? Target->default_val.i64 : default;

    /// <summary>
    /// Gets the default value defined for this option.
    /// </summary>
    public AVRational DefaultRationalValue => Target is not null ? Target->default_val.q : default;

    /// <summary>
    /// Gets the default value defined for this option.
    /// </summary>
    public double DefaultDoubleValue => Target is not null ? Target->default_val.dbl : default;

    /// <inheritdoc />
    public override string ToString()
    {
        return IsNull
            ? $"{nameof(AVOption)}* null"
            : $"{Name} ({Type.ToString().Replace("AV_OPT_TYPE_", string.Empty, StringComparison.Ordinal)}, {Unit}): {Help}";
    }
}
