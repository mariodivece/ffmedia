namespace FFMedia.Primitives;

internal static unsafe class OptionsExtensions
{
    public static IReadOnlyList<IFFOptionsDefined> GetChildren(this IFFOptionsDefined options)
    {
        var result = new List<IFFOptionsDefined>();
        void* iterator = null;

        void* currentChild =  null;
        do
        {
            currentChild = ffmpeg.av_opt_child_next(options.ToPointer(), currentChild);
            if (currentChild is not null)
                result.Add(new FFMediaClass(currentChild));

        } while (currentChild is not null);

        return result;
    }


    public static void SetOptionValue(this IFFOptionsStorage options, string optionName, bool searchChildren, string value)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        if (options.IsNull)
            throw new ArgumentNullException(nameof(options));

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(options.ToPointer(), optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option '{optionName}'.");
    }

    public static bool TrySetOptionValue(this IFFOptionsStorage options, string optionName, bool searchChildren, string value)
    {
        if (options is null || options.IsNull || string.IsNullOrWhiteSpace(optionName) || string.IsNullOrWhiteSpace(value))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(options.ToPointer(), optionName, value, searchFlags);
        return resultCode == 0;
    }

    public static string GetOptionValue(this IFFOptionsStorage options, string optionName, bool searchChildren)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        if (options.IsNull)
            throw new ArgumentNullException(nameof(options));

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(options.ToPointer(), optionName, searchFlags, &stringValuePointer);

        FFException.ThrowIfNegative(resultCode, $"Failed to get option '{optionName}'.");
        return NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
    }

    public static bool TryGetOptionValue(this IFFOptionsStorage options, string optionName, bool searchChildren, [MaybeNullWhen(false)] out string value)
    {
        value = null;
        if (options is null || options.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(options.ToPointer(), optionName, searchFlags, &stringValuePointer);

        if (resultCode == 0)
        {
            value = NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
            return true;
        }

        return false;
    }
}
