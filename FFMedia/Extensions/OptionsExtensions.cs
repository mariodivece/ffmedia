namespace FFMedia.Extensions;

/// <summary>
/// Provides options management for options-enabled objects as extension methods,
/// rather than having to implement each interface member individually for each
/// options-enabled class.
/// </summary>
public static unsafe class OptionsExtensions
{
    /// <summary>
    /// Attempts to find an existing option in the options-enabled object.
    /// </summary>
    /// <remarks>
    /// If search children is set to true, then the lookup occurs in the chilren first.
    /// Otherwise, the lookup occurs in the current object exclusively.
    /// </remarks>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The name of the option to look for.</param>
    /// <param name="searchChildren">The the option name is searched for in the child objects first.</param>
    /// <param name="option">The option if it was found.</param>
    /// <returns>True if the option lookup succeeds. False otherwise.</returns>
    public static bool TryFindOption(this IFFOptionsEnabled store, string optionName,
        bool searchChildren, [MaybeNullWhen(false)] out FFOption option)
    {
        option = default;
        if (store is null || store.IsNull)
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0;
        var avoption = ffmpeg.av_opt_find(store.ToPointer(), optionName, null, 0, searchFlags);
        if (avoption is not null)
        {
            option = new(avoption);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the option with the given name exists in either
    /// the options-enabled object (or in one of its children).
    /// </summary>
    /// <remarks>
    /// If search children is set to true, then the lookup occurs in the chilren first.
    /// Otherwise, the lookup occurs in the current object exclusively.
    /// </remarks>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The name of the option to look for.</param>
    /// <param name="searchChildren">The the option name is searched for in the child objects first.</param>
    /// <returns>Whether or not the option exists.</returns>
    public static bool HasOption(this IFFOptionsEnabled store, string optionName, bool searchChildren) =>
        store.TryFindOption(optionName, searchChildren, out _);

    /// <summary>
    /// Sets an option value. Will throw on error.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="value">The value to set the option to.</param>
    public static void SetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, string? value)
    {
        var searchFlags = store.PrepareOptionFlags(optionName, searchChildren);
        var resultCode = ffmpeg.av_opt_set(store.ToPointer(), optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    /// <summary>
    /// Sets an option value. Will throw on error.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="value">The value to set the option to.</param>
    public static void SetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, int value)
    {
        var searchFlags = store.PrepareOptionFlags(optionName, searchChildren);
        var resultCode = ffmpeg.av_opt_set_int(store.ToPointer(), optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    /// <summary>
    /// Sets an option to a list of values. Will throw on error.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="values">The values to set.</param>
    public static void SetOptionValues<T>(this IFFOptionsEnabled store, string optionName, bool searchChildren, T[] values)
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Length == 0) return;
        var searchFlags = store.PrepareOptionFlags(optionName, searchChildren);

        var pinnedValues = stackalloc T[values.Length];
        for (var i = 0; i < values.Length; i++)
            pinnedValues[i] = values[i];

        var resultCode = ffmpeg.av_opt_set_bin(store.ToPointer(), optionName, (byte*)pinnedValues, values.Length * sizeof(T), searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    /// <summary>
    /// Attempts to set an option value.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if succeeded. False otherwise.</returns>
    public static bool TrySetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, string? value)
    {
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(store.ToPointer(), optionName, value, searchFlags);
        return resultCode == 0;
    }

    /// <summary>
    /// Attempts to set an option value.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>True if succeeded. False otherwise.</returns>
    public static bool TrySetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, int value)
    {
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set_int(store.ToPointer(), optionName, value, searchFlags);
        return resultCode == 0;
    }

    /// <summary>
    /// Attempts to set an option value.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="values">The values to set.</param>
    public static bool TrySetOptionValues<T>(this IFFOptionsEnabled store, string optionName, bool searchChildren, T[] values)
        where T : unmanaged
    {
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName) || values is null || values.Length == 0)
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;

        var pinnedValues = stackalloc T[values.Length];
        for (var i = 0; i < values.Length; i++)
            pinnedValues[i] = values[i];

        var resultCode = ffmpeg.av_opt_set_bin(store.ToPointer(), optionName, (byte*)pinnedValues, values.Length * sizeof(T), searchFlags);
        return resultCode == 0;
    }

    /// <summary>
    /// Gets the value of an option. Will throw on error.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <returns>The option value.</returns>
    public static string GetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren)
    {
        var searchFlags = store.PrepareOptionFlags(optionName, searchChildren);
        byte* stringValuePointer;
        var resultCode = ffmpeg.av_opt_get(store.ToPointer(), optionName, searchFlags, &stringValuePointer);

        FFException.ThrowIfNegative(resultCode, $"Failed to get option value for '{optionName}'.");
        return NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
    }

    /// <summary>
    /// Attempts to get the value of an option.
    /// </summary>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The option name.</param>
    /// <param name="searchChildren">Searches for child options first.</param>
    /// <param name="value">The value of the option.</param>
    /// <returns>True if success. False otherwise.</returns>
    public static bool TryGetOptionValue(this IFFOptionsEnabled store, string optionName,
        bool searchChildren, [MaybeNullWhen(false)] out string? value)
    {
        value = null;
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(store.ToPointer(), optionName, searchFlags, &stringValuePointer);

        if (resultCode == 0)
        {
            value = NativeExtensions.ReadString(stringValuePointer);
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PrepareOptionFlags(this IFFOptionsEnabled store, string optionName, bool searchChildren)
    {
        ArgumentNullException.ThrowIfNull(store);
        ObjectDisposedException.ThrowIf(store.IsNull, store);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        return searchFlags;
    }
}
