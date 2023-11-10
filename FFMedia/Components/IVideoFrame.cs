namespace FFMedia.Components;

/// <summary>
/// Defines the members for a working video frame.
/// </summary>
public interface IVideoFrame : IPictureBufferSpec, IMediaFrame
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
    /// Defines the default pixel format.
    /// </summary>
    public const AVPixelFormat DefaultPixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;

    /// <summary>
    /// Defines the default Pixel Aspect Ratio (1 / 1).
    /// </summary>
    public static readonly AVRational DefaultPixelAspectRatio = RationalExtensions.OneValue;
}
