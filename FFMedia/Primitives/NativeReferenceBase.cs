namespace FFMedia.Primitives;

/// <summary>
/// Serves as a base implementation of the <see cref="INativeReference{T}"/>
/// This allowa a class to wrap an unmanaged data structure.
/// interface.
/// </summary>
/// <typeparam name="T">The unmanaged type to wrap.</typeparam>
public abstract unsafe class NativeReferenceBase<T> :
    NativeReference,
    INativeReference<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeReferenceBase{T}"/> class.
    /// </summary>
    /// <param name="target">The data structure to wrap.</param>
    protected NativeReferenceBase(T* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="NativeReferenceBase{T}"/> class.
    /// You can the call the <see cref="INativeReference.Update(nint)"/> method to provide access to the reference.
    /// </summary>
    protected NativeReferenceBase()
        : base(IntPtr.Zero)
    {
        // placeholder
    }

    /// <inheritdoc />
    public T* Target => IsNull ? null : (T*)Address;

    /// <inheritdoc />
    public T Value => IsNull ? default : *Target;

    /// <inheritdoc />
    public int StructureSize => sizeof(T);

    /// <inheritdoc />
    public void Update(T* target) => Update(target is null
        ? default
        : new nint(target));
}
