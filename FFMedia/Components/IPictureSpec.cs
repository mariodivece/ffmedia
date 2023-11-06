namespace FFMedia.Components;

/// <summary>
/// Defines the members participating in a picture specification.
/// </summary>
public unsafe interface IPictureSpec
{
    /// <summary>
    /// Gets the number of bytes per pixel.
    /// Returns null if <see cref="PixelFormat"/> is invalid or unkown.
    /// </summary>
    int? BytesPerPixel { get; }

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
    /// Often times this is called stride and line size.
    /// </summary>
    int RowBytes { get; }

    /// <summary>
    /// Gets the pixel aspect ratio.
    /// </summary>
    AVRational PixelAspectRatio { get; }

    /// <summary>
    /// Gets the dots per inch in the X direction.
    /// </summary>
    double DpiX { get; }

    /// <summary>
    /// Gets the dots per inch in the Y direction. 
    /// </summary>
    double DpiY { get; }
}
