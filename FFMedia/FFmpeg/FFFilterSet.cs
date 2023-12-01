namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for a set of <see cref="FFFilterContext"/>.
/// </summary>
public unsafe sealed class FFFilterSet :
    NativeChildSet<FFFilterGraph, FFFilterContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFFilterContext"/> class.
    /// </summary>
    /// <param name="parent">The owning filter graph.</param>
    public FFFilterSet(FFFilterGraph parent)
        : base(parent)
    {
        // placeholder
    }

    /// <inheritdoc />
    public override FFFilterContext this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            if (Parent.IsNull ||
                Parent.Target is null ||
                Parent.Target->filters is null ||
                Parent.Target->filters[index] is null)
                throw new InvalidOperationException("Child object is not addressable.");

            return new(Parent.Target->filters[index]);
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            ArgumentNullException.ThrowIfNull(value);

            Parent.Target->filters[index] = value.Target;
        }
    }

    /// <inheritdoc />
    public override int Count => Parent is not null && Parent.Target is not null
        ? Convert.ToInt32(Parent.Target->nb_filters)
        : default;
}
