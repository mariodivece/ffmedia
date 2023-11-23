namespace FFMedia.Primitives;

/// <summary>
/// Serves as a base implementation of the <see cref="INativeReference{T}"/>
/// This allowa a class to wrap an unmanaged data structure.
/// interface.
/// </summary>
/// <typeparam name="T">The unmanaged type to wrap.</typeparam>
public abstract unsafe class NativeReferenceBase<T> : INativeReference<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeReferenceBase{T}"/> class.
    /// </summary>
    /// <param name="target">The data structure to wrap.</param>
    protected NativeReferenceBase(T* target)
    {
        Update(target);
    }

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="NativeReferenceBase{T}"/> class.
    /// You can the call the <see cref="Update(nint)"/> method to provide access to the reference.
    /// </summary>
    protected NativeReferenceBase()
    {
        // placeholder
    }

    /// <inheritdoc />
    public nint Address { get; protected set; }

    /// <inheritdoc />
    public T* Target => IsNull ? null : (T*)Address;

    /// <inheritdoc />
    public T Value => IsNull ? default : *Target;

    /// <inheritdoc />
    public bool IsNull => Address == 0;

    /// <inheritdoc />
    public int StructureSize => sizeof(T);

    /// <inheritdoc />
    public void Update(nint address) => Address = address;

    /// <inheritdoc />
    public void Update(T* target) => Address = target is null
        ? default
        : new(target);
}
