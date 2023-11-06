namespace FFMedia.Extensions;

internal static unsafe class PictureExtensions
{
    /// <summary>
    /// Obtains a <see cref="AVPixFmtDescriptor"/> for the provided format
    /// and extracts the number of bytes per pixel including padding.
    /// </summary>
    /// <param name="format">The pixel format.</param>
    /// <returns>The number of bytes to store each pixel for the specified format.</returns>
    public static int? BytesPerPixel(this AVPixelFormat format)
    {
        const int BitsPerByte = 8;
        var descriptor = ffmpeg.av_pix_fmt_desc_get(format);
        if (descriptor is null) return null;

        var bitsPerPixel = ffmpeg.av_get_padded_bits_per_pixel(descriptor);
        return bitsPerPixel / BitsPerByte;
    }

    /// <summary>
    /// Gets the pixel format replacing deprecated pixel formats.
    /// AV_PIX_FMT_YUVJXXXX are replaced with their AV_PIX_FMT_YUVXXXX equivalents.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>A normalized pixel format.</returns>
    public static AVPixelFormat Normalize(this AVPixelFormat format)
    {
        return format switch
        {
            AVPixelFormat.AV_PIX_FMT_YUVJ411P => AVPixelFormat.AV_PIX_FMT_YUV411P,
            AVPixelFormat.AV_PIX_FMT_YUVJ420P => AVPixelFormat.AV_PIX_FMT_YUV420P,
            AVPixelFormat.AV_PIX_FMT_YUVJ422P => AVPixelFormat.AV_PIX_FMT_YUV422P,
            AVPixelFormat.AV_PIX_FMT_YUVJ440P => AVPixelFormat.AV_PIX_FMT_YUV440P,
            AVPixelFormat.AV_PIX_FMT_YUVJ444P => AVPixelFormat.AV_PIX_FMT_YUV444P,
            _ => format,
        };
    }
}
