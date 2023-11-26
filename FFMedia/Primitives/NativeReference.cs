﻿namespace FFMedia.Primitives;

/// <summary>
/// Represents a basic native reference wrapper class that wraps a pointer.
/// </summary>
public unsafe class NativeReference : INativeReference
{
    /// <summary>
    /// Creates a new instance of the <see cref="NativeReference"/> class.
    /// </summary>
    /// <param name="address">The pointer address this object points to.</param>
    public NativeReference(nint address)
    {
        Update(address);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="NativeReference"/> class.
    /// </summary>
    /// <param name="target">The pointer this object points to.</param>
    public NativeReference(void* target)
    {
        Update(new nint(target));
    }

    /// <inheritdoc />
    public nint Address { get; private set; }

    /// <inheritdoc />
    public bool IsNull => Address == IntPtr.Zero;

    /// <inheritdoc />
    public void Update(nint address) => Address = address;
}