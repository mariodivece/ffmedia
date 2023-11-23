using System.Text;

namespace FFMedia.Extensions;

internal static unsafe class NativeExtensions
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe string? ReadString(void* target) =>
        ReadString(new nint(target));

    /// <summary>
    /// Converts any non-zero integer to a boolean.
    /// </summary>
    /// <param name="value">The integer to convert to a boolean.</param>
    /// <returns>The boolean value.</returns>
    public static bool ToBool(this int value) => value != 0;

    public static void* ToPointer(this INativeReference nativeReference) =>
        nativeReference.IsNull ? null : nativeReference.Address.ToPointer();
}
