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
}
