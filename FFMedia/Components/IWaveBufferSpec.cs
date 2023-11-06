namespace FFMedia.Components;

/// <summary>
/// Extends the <see cref="IWaveSpec"/> to also contain a wave buffer.
/// </summary>
public unsafe interface IWaveBufferSpec : IWaveSpec
{
    /// <summary>
    /// Gets the address of the buffer.
    /// </summary>
    nint BufferAddress { get; }

    /// <summary>
    /// Gets the number of bytes for this buffer.
    /// </summary>
    int BufferLength { get; }

    /// <summary>
    /// Gets the duration of the buffer in seconds.
    /// </summary>
    TimeExtent BufferDuration {  get; }

    /// <summary>
    /// Gets the number of samples contained in this buffer (per channel).
    /// </summary>
    int SampleCount { get; }
}
