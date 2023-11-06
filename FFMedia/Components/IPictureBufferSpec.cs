namespace FFMedia.Components;

/// <summary>
/// Extends the <see cref="IPictureSpec"/> to also contain picture data buffers.
/// </summary>
public interface IPictureBufferSpec : IPictureSpec
{
    /// <summary>
    /// Gets the address of the first pixel.
    /// </summary>
    nint BufferAddress { get; }

    /// <summary>
    /// Gets the number of bytes for this buffer.
    /// </summary>
    int BufferLength {  get; }
}
