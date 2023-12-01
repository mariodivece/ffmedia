namespace FFmpeg;

/// <summary>
/// Serves as a wrapper to the <see cref="AVBufferRef"/> data structure.
/// </summary>
public unsafe class FFBufferRef :
    NativeReferenceBase<AVBufferRef>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFBufferRef"/> class.
    /// </summary>
    /// <param name="target">The data structure to wrap.</param>
    public FFBufferRef(AVBufferRef* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>
    /// Retrieves the contents 
    /// </summary>
    /// <returns></returns>
    public Span<T> AsSpan<T>()
        where T : unmanaged
    {
        var elementSize = (ulong)sizeof(T);
        var elementCount = Convert.ToInt32(Target->size / elementSize);
        return new Span<T>(Target->data, elementCount);
    }
}
