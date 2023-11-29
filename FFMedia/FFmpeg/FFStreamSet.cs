namespace FFmpeg;

/// <summary>
/// Wraps a set of streams.
/// </summary>
public unsafe sealed class FFStreamSet :
    NativeChildSet<FFFormatContext, FFStream>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFStreamSet"/> class.
    /// </summary>
    /// <param name="parent"></param>
    public FFStreamSet(FFFormatContext parent)
        : base(parent)
    {
        // placeholder
    }

    /// <inheritdoc />
    public override FFStream this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            if (Parent.IsNull || Parent.Target is null || Parent.Target->streams is null || Parent.Target->streams[index] is null)
                throw new InvalidOperationException("Child object is not addressable.");

            return new(Parent.Target->streams[index], Parent);
        }
        set => throw new NotSupportedException("The objects in this set cannot be relocated or replaced.");
    }

    /// <inheritdoc />
    public override int Count => Parent is not null && Parent.Target is not null
        ? Convert.ToInt32(Parent.Target->nb_streams)
        : default;
}
