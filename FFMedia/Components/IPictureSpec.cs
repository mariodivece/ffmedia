namespace FFMedia.Components;

/// <summary>
/// Defines the members participating in a picture specification.
/// </summary>
public unsafe interface IPictureSpec
{
    /// <summary>
    /// Defines the default DPI in the X direction (96 dpi).
    /// </summary>
    public const double DefaultDpiX = 96.0;

    /// <summary>
    /// Defines the default DPI in the Y direction (96 dpi).
    /// </summary>
    public const double DefaultDpiY = 96.0;

    /// <summary>
    /// Defines the default Pixel Aspect Ratio (1 / 1).
    /// </summary>
    public static readonly AVRational DefaultPixelAspectRatio = FFRational.OneValue;

    /// <summary>
    /// Defines the default pixel format.
    /// </summary>
    public const AVPixelFormat DefaultPixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;

    /// <summary>
    /// Gets the number of bytes per pixel.
    /// Returns null if <see cref="PixelFormat"/> is invalid or unkown.
    /// </summary>
    int? BytesPerPixel
    {
        get
        {
            const int BitsPerByte = 8;
            var descriptor = ffmpeg.av_pix_fmt_desc_get(PixelFormat);
            if (descriptor is null) return null;

            var bitsPerPixel = ffmpeg.av_get_padded_bits_per_pixel(descriptor);
            return bitsPerPixel / BitsPerByte;
        }
    }

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
    AVRational PixelAspectRatio => DefaultPixelAspectRatio;

    /// <summary>
    /// Gets the dots per inch in the X direction.
    /// </summary>
    double DpiX => DefaultDpiX;

    /// <summary>
    /// Gets the dots per inch in the Y direction. 
    /// </summary>
    double DpiY => DefaultDpiY;
}
