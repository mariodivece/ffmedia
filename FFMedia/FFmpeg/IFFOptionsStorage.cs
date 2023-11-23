namespace FFmpeg;

/// <summary>
/// Defines the members for a <see cref="IFFOptionsDefined"/>
/// class that can get or set <see cref="AVOption"/> values
/// </summary>
public interface IFFOptionsStorage : IFFOptionsDefined
{
    /// <summary>
    /// Sets the value of an option to an array of the specified values.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Whether to also search for the option in child objects.</param>
    /// <param name="values">The option value.</param>
    /// <returns></returns>
    void SetOptionList<T>(string optionName, bool searchChildren, T[] values)
        where T : unmanaged;


    void SetOptionValue(string optionName, bool searchChildren, string value);


}
