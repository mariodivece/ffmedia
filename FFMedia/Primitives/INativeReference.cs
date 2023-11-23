namespace FFMedia.Primitives;

/// <summary>
/// Represents the basic members of a class that
/// wraps a data structure that resides in unmanaged
/// memory.
/// </summary>
public interface INativeReference
{
    /// <summary>
    /// Gets the pointer address of the unmanaged data structure.
    /// </summary>
    nint Address { get; }

    /// <summary>
    /// Updates the pointer address of the unmanaged data structure.
    /// </summary>
    /// <param name="address">The new address assigned.</param>
    void Update(nint address);

    /// <summary>
    /// Gets a value indicating whether this instance no
    /// points to a non-zero <see cref="Address"/>
    /// </summary>
    bool IsNull { get; }
}

/// <summary>
/// Represents a strongly-typed version of the <see cref="INativeReference"/>
/// interface with additional capabilities.
/// </summary>
/// <typeparam name="T">The unmanaged type to wrap.</typeparam>
public unsafe interface INativeReference<T> : INativeReference
    where T : unmanaged
{
    /// <summary>
    /// Gets a pointer reference to the wrapped data structure.
    /// </summary>
    T* Target { get; }

    /// <summary>
    /// Updates the pointer to the specified target.
    /// </summary>
    /// <param name="target">The pointer target.</param>
    void Update(T* target);

    /// <summary>
    /// Gets the de-referenced value of the wrapped data structure.
    /// </summary>
    T Value { get; }

    /// <summary>
    /// Gets the size in bytes of the wrapped data structure.
    /// </summary>
    int StructureSize { get; }
}
