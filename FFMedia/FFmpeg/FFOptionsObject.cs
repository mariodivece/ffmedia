namespace FFMedia.FFmpeg;

/// <summary>
/// Represents a wrapper for  data structures with AV-options enabled functionality.
/// Such structures must have a <see cref="AVClass"/> pointer as their first defined element.
/// </summary>
public sealed unsafe class FFOptionsObject :
    NativeReference
{
    private static readonly Dictionary<Type, bool> MediaClassField = new(16);
    private static readonly object SyncRoot = new();

    private FFOptionsObject(void* target)
        : base(target)
    {
    }

    /// <summary>
    /// Gets the <see cref="INativeReference.Address"/> as a pointer.
    /// </summary>
    public void* Target => Address.ToPointer();

    public IReadOnlyList<FFOption> Options
    {
        get
        {
            if (IsNull)
                return Array.Empty<FFOption>();

            var options = new List<FFOption>();

            AVOption* currentOption = null;
            do
            {
                currentOption = ffmpeg.av_opt_next(Target, currentOption);
                if (currentOption is not null && currentOption->name is not null)
                    options.Add(new(currentOption));

            } while (currentOption is not null);
            return options;
        }
    }

    public IReadOnlyList<FFOptionsObject> Children
    {
        get
        {
            if (Target is null)
                return Array.Empty<FFOptionsObject>();

            var result = new List<FFOptionsObject>();
            void* currentChild = null;

            do
            {
                currentChild = ffmpeg.av_opt_child_next(Target, currentChild);

                if (currentChild is not null)
                    result.Add(new FFOptionsObject(currentChild));

            } while (currentChild is not null);

            return result;

        }
    }

    public static bool TryWrap<T>(INativeReference<T> obj,[MaybeNullWhen(false)] out FFOptionsObject optionsObject)
        where T : unmanaged
    {
        optionsObject = null;
        if (obj is null || obj.IsNull) return false;
        if (!CheckIsAVOptionsEnabled<T>())
            return false;

        optionsObject = new(obj.Address.ToPointer());
        return true;
    }

    public FFOption? FindOption(string optionName, bool searchChildren)
    {
        if (Target is null)
            return default;

        var searchFlags = (searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0);
        var option = ffmpeg.av_opt_find(Target, optionName, null, 0, searchFlags);
        return option is not null
            ? new(option)
            : default;
    }

    public bool HasOption(string optionName, bool searchChildren) =>
        FindOption(optionName, searchChildren) is not null;

    public void SetOptionValue(string optionName, bool searchChildren, string value)
    {
        ObjectDisposedException.ThrowIf(IsNull, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(Target, optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    public bool TrySetOptionValue(string optionName, bool searchChildren, string value)
    {
        if (IsNull || string.IsNullOrWhiteSpace(optionName) || string.IsNullOrWhiteSpace(value))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(Target, optionName, value, searchFlags);
        return resultCode == 0;
    }

    public string GetOptionValue(string optionName, bool searchChildren)
    {
        ObjectDisposedException.ThrowIf(IsNull, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(Target, optionName, searchFlags, &stringValuePointer);

        FFException.ThrowIfNegative(resultCode, $"Failed to get option value for '{optionName}'.");
        return NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
    }

    public bool TryGetOptionValue(string optionName, bool searchChildren, [MaybeNullWhen(false)] out string value)
    {
        value = null;
        if (IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(Target, optionName, searchFlags, &stringValuePointer);

        if (resultCode == 0)
        {
            value = NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
            return true;
        }

        return false;
    }

    private static bool CheckIsAVOptionsEnabled<TStruct>()
        where TStruct : unmanaged
    {
        lock (SyncRoot)
        {
            if (!MediaClassField.TryGetValue(typeof(TStruct), out var isValid))
            {
                var fields = typeof(TStruct).GetFields();
                isValid = fields.Length > 0 && fields[0].FieldType == typeof(AVClass*);
                MediaClassField[typeof(TStruct)] = isValid;
            }

            return isValid;
        }
    }
}
