
using System.Dynamic;

namespace FFMedia.Engine;

/// <summary>
/// Provides configuration options for a <see cref="MediaContainer"/>.
/// </summary>
public abstract class MediaOptionsBase : IMediaOptions
{
    private readonly ExpandoObject _options = new();

    private IDictionary<string, object?> Options => _options;

    /// <inheritdoc />
    public T? GetValueOrDefault<T>(T defaultValue, [CallerMemberName] string? propertyName = default) =>
        string.IsNullOrWhiteSpace(propertyName)
        ? throw new ArgumentNullException(nameof(propertyName))
        : TryGetOptionValue<T>(propertyName, out var value) ? value : defaultValue;

    /// <inheritdoc />
    public void SetOptionValue<T>(T value, [CallerMemberName] string? propertyName = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        Options[propertyName] = value;
    }

    /// <inheritdoc />
    public ICollection<string> Keys => Options.Keys;

    /// <inheritdoc />
    public bool ContainsKey(string key) => Options.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetOptionValue<T>(string key, out T? value)
    {
        value = default;

        if (string.IsNullOrWhiteSpace(key))
            return false;

        var result = Options.TryGetValue(key, out var objectValue);
        if (!result)
            return false;

        if (objectValue is null)
            return true;

        if (!typeof(T).IsAssignableFrom(objectValue.GetType()))
            return false;

        value =  (T)objectValue;
        return true;
    }

    /// <inheritdoc />
    public dynamic AsDynamic() => _options;

    /// <inheritdoc />
    public IDictionary<string, object?> AsDictionary() => _options;
}
