namespace FFMedia.Components;

internal interface IPictureSpec
{
    /// <summary>
    /// Gets the address of the first pixel.
    /// </summary>
    nint BufferAddress { get; }

    /// <summary>
    /// Gets the pixel format.
    /// </summary>
    AVPixelFormat PixelFormat { get; }

    /// <summary>
    /// Gets the number of pixel columns.
    /// </summary>
    int PixelWidth { get; }

    /// <summary>
    /// Gets the number of pixel rows.
    /// </summary>
    int PixelHeight { get; }

    /// <summary>
    /// Gets the number of bytes per row.
    /// </summary>
    int RowBytes { get; }

    /// <summary>
    /// Gets the pixel aspect ratio.
    /// </summary>
    AVRational PixelAspectRatio { get; }

    /// <summary>
    /// Gets the dots per inch in the X direction.
    /// </summary>
    public double DpiX { get; }

    /// <summary>
    /// Gets the dots per inch in the Y direction. 
    /// </summary>
    public double DpiY { get; }
}
