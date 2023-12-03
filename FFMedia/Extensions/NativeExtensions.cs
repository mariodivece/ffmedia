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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ToBool(this int value) => value != 0;

    /// <summary>
    /// Reads an array of consecutive items of the given
    /// type at the starting address and until a value with
    /// the termination value is found. The item with the termination
    /// value is not included in the resulting list.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    /// <param name="firstItem">The pointer to the first item.</param>
    /// <param name="terminationValue">The value of the end of the array.</param>
    /// <returns>A list of the items that was read from memory.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<T> ExtractArray<T>(T* firstItem, T terminationValue)
        where T : unmanaged
    {
        if (firstItem is null)
            return Array.Empty<T>();

        var result = new List<T>(32);
        var currentItem = default(T);
        var reachedEnd = false;
        var offset = 0;
        do
        {
            currentItem = firstItem[offset++];
            reachedEnd = currentItem.Equals(terminationValue);
            if (!reachedEnd)
                result.Add(currentItem);

        } while (!reachedEnd);

        return result;
    }

    /// <summary>
    /// Determines if an integer storing certain flags contains one or more given flag values.
    /// </summary>
    /// <param name="flagsVariable">The flags variable to check.</param>
    /// <param name="flagValue">The flags or set of combined flags to match.</param>
    /// <returns>True if the flags variable contains one or more flags to match.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(this int flagsVariable, int flagValue) => (flagsVariable & flagValue) != 0;

    /// <summary>
    /// Sets a given integer flag for a certain variable containing flag values.
    /// Returns a new value with the specified flags set.
    /// </summary>
    /// <param name="flagsVariable">The flags value to set.</param>
    /// <param name="flagValue">The flags or set of combined flags to set.</param>
    /// <returns>The flags set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetFlag(this int flagsVariable, int flagValue) => flagsVariable |= flagValue;
}
