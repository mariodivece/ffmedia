namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for a set of <see cref="FFChapter"/>.
/// </summary>
public unsafe sealed class FFChapterSet :
    NativeChildSet<FFFormatContext, FFChapter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFChapterSet"/> class.
    /// </summary>
    /// <param name="parent">The owning format context</param>
    public FFChapterSet(FFFormatContext parent)
        : base(parent)
    {
        // placeholder
    }

    /// <inheritdoc />
    public override FFChapter this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            if (Parent.IsNull ||
                Parent.Target is null || 
                Parent.Target->chapters is null || 
                Parent.Target->chapters[index] is null)
                throw new InvalidOperationException("Child object is not addressable.");

            return new(Parent.Target->chapters[index]);
        }
        set => throw new NotSupportedException("The objects in this set cannot be relocated or replaced.");
    }

    /// <inheritdoc />
    public override int Count => Parent is not null && Parent.Target is not null
        ? Convert.ToInt32(Parent.Target->nb_chapters)
        : default;
}
