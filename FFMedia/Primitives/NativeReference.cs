
namespace FFMedia.Primitives;

/// <summary>
/// Represents a basic native reference wrapper class that wraps a pointer.
/// </summary>
public unsafe class NativeReference : INativeReference
{
    /// <summary>
    /// Creates a new instance of the <see cref="NativeReference"/> class.
    /// No null pointer checks are performed for this constructor.
    /// </summary>
    /// <param name="address">The pointer address this object points to.</param>
    public NativeReference(nint address) => Update(address);

    /// <summary>
    /// Creates a new instance of the <see cref="NativeReference"/> class.
    /// No null pointer checks are performed for this constructor.
    /// </summary>
    /// <param name="target">The pointer this object points to.</param>
    public NativeReference(void* target) => Update(new nint(target));

    /// <inheritdoc />
    public nint Address { get; private set; }

    /// <inheritdoc />
    public bool IsNull => Address == default;

    /// <inheritdoc />
    public void* ToPointer() => Address.ToPointer();

    /// <inheritdoc />
    public void Update(nint address) => Address = address;
}
