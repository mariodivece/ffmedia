using System.Diagnostics;

namespace FFMedia;

/// <summary>
/// Provides library helper methods.
/// </summary>
public static partial class Helper
{
    /// <summary>
    /// Given a set of sorted values, searches for a value that is equal or greater than
    /// the given search value. If the search value is less than the first value in the set,
    /// it returns the first value. This method useas the binary search algorithm.
    /// </summary>
    /// <typeparam name="T">The type of items to work with.</typeparam>
    /// <param name="sortedValues">A list of items sorted in ascending order.</param>
    /// <param name="searchValue">The value to search for.</param>
    /// <param name="closestValue">The output value contained in the list.</param>
    /// <returns>True if the lookup succeeds, false otherwise.</returns>
    public static bool TrySlidingValueSearch<T>(this IList<T> sortedValues, T searchValue, [MaybeNullWhen(false)] out T closestValue)
        where T : notnull, IComparable<T>
    {
        closestValue = default;

        if (sortedValues is null || sortedValues.Count <= 0)
            return false;

        var valueCount = sortedValues.Count;
        var firstIndex = 0;
        var lastIndex = valueCount - 1;
        var midIndex = (firstIndex + lastIndex) / 2;
        var currentValue = sortedValues[midIndex];
        var compareResult = searchValue.CompareTo(currentValue);
        var iterationCount = 1;

        while (valueCount > 2)
        {
            if (compareResult >= 0)
                firstIndex = midIndex;

            if (compareResult <= 0)
                lastIndex = midIndex;

            midIndex = (firstIndex + lastIndex) / 2;
            valueCount = lastIndex - firstIndex + 1;
            currentValue = sortedValues[midIndex];
            compareResult = searchValue.CompareTo(currentValue);
            iterationCount++;
        }

        currentValue = sortedValues[lastIndex];
        closestValue = searchValue.CompareTo(currentValue) >= 0
            ? currentValue
            : sortedValues[firstIndex];

        Debug.WriteLine(iterationCount);
        return true;
    }
}
