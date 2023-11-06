using System.Text;

namespace FFMedia.Extensions;

internal static class NativeExtensions
{
    /// <summary>
    /// Given a buffer address, read a series of UTF8 characters
    /// unitl a 0 byte value is encountered.
    /// </summary>
    /// <param name="address">The pointer address of the first character position.</param>
    /// <returns>The decoded string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string? ReadString(this nint address)
    {
        if (address == default)
            return default;

        var source = (byte*)address.ToPointer();
        var length = 0;
        while (source[length] != 0)
            ++length;

        if (length == 0)
            return string.Empty;

        var byteSpan = new Span<byte>(source, length);
        return Encoding.UTF8.GetString(byteSpan);
    }

    public static bool ToBool(this int value) => value != 0;
}
