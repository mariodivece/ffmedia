using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FFMedia;

/// <summary>
/// Provides library helper methods.
/// </summary>
public static partial class Helper
{
    /// <summary>
    /// Given a set of sorted values, searches for a value that is equal or greater than
    /// the given search value. If the search value is less than the first value in the set,
    /// it outputs the first value. This method useas the binary search algorithm.
    /// </summary>
    /// <typeparam name="T">The type of items to work with.</typeparam>
    /// <param name="sortedValues">A list of items sorted in ascending order.</param>
    /// <param name="searchValue">The value to search for.</param>
    /// <param name="closestValue">The output value contained in the list.</param>
    /// <returns>The index of the closest value. Returns -1 when the sorted values is null or empty.</returns>
    public static int IndexOfClosest<T>(this IList<T> sortedValues, T searchValue, [MaybeNullWhen(false)] out T closestValue)
        where T : notnull, IComparable<T>
    {
        closestValue = default;

        if (sortedValues is null || sortedValues.Count <= 0)
            return -1;

        var spanValues = sortedValues is T[] array
            ? new Span<T>(array)
            : sortedValues is List<T> list
            ? CollectionsMarshal.AsSpan(list)
            : default;

        var isFastAccess = spanValues.Length > 0;
        var valueCount = isFastAccess ? spanValues.Length : sortedValues.Count;
        var firstIndex = 0;
        var lastIndex = valueCount - 1;
        var midIndex = (firstIndex + lastIndex) / 2;
        var currentValue = isFastAccess ? spanValues[midIndex] : sortedValues[midIndex];
        var compareResult = searchValue.CompareTo(currentValue);
        var iterationCount = 0;

        while (valueCount > 2)
        {
            if (compareResult >= 0)
                firstIndex = midIndex;

            if (compareResult <= 0)
                lastIndex = midIndex;

            midIndex = (firstIndex + lastIndex) / 2;
            valueCount = lastIndex - firstIndex + 1;
            currentValue = isFastAccess ? spanValues[midIndex] : sortedValues[midIndex];
            compareResult = searchValue.CompareTo(currentValue);
            iterationCount++;
        }

        var resultIndex = lastIndex;
        currentValue = isFastAccess ? spanValues[resultIndex] : sortedValues[resultIndex];
        if (searchValue.CompareTo(currentValue) < 0)
            resultIndex = firstIndex;

        closestValue = isFastAccess ? spanValues[resultIndex] : sortedValues[resultIndex];
        iterationCount++;

        Debug.WriteLine($"Fast Access: {isFastAccess}, Value Count: {sortedValues.Count}, Comparisons: {iterationCount}, Search: {searchValue}, Found: {closestValue}, Index {resultIndex}");
        return resultIndex;
    }
}
