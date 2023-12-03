using System.Collections.Concurrent;

namespace FFmpeg;

/// <summary>
/// Represents a wrapper for  data structures with AV-options enabled functionality.
/// Such structures must have a <see cref="AVClass"/> pointer as their first defined element.
/// This is a proxy class for implementing the <see cref="IFFOptionsEnabled"/> interface.
/// </summary>
internal sealed unsafe class FFOptionsStore :
    NativeReference,
    IFFOptionsEnabled
{
    private static readonly ConcurrentDictionary<Type, bool> MediaClassFields = new();

    /// <summary>
    /// Creates an instance of the <see cref="FFOptionsStore"/> class.
    /// Use the <see cref="TryWrap{T}(INativeReference{T}, out IFFOptionsEnabled)"/> method
    /// to create and validate instances of this wrapper/proxy class.
    /// </summary>
    /// <param name="target">The pointer to the object.</param>
    public FFOptionsStore(void* target)
        : base(target)
    {
    }

    /// <summary>
    /// Gets an empty <see cref="FFOptionsStore"/>.
    /// </summary>
    public static FFOptionsStore Empty { get; } = new(null);

    /// <inheritdoc />
    public FFMediaClass MediaClass
    {
        get
        {
            if (IsNull)
                return FFMediaClass.Empty;

            var avClassPointer = (AVClass*)((nint*)Target)[0];
            return new(avClassPointer);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<FFOption> CurrentOptions
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

    /// <inheritdoc />
    public IReadOnlyList<IFFOptionsEnabled> CurrentChildren
    {
        get
        {
            if (IsNull)
                return Array.Empty<FFOptionsStore>();

            var result = new List<FFOptionsStore>();
            void* currentChild = null;

            do
            {
                currentChild = ffmpeg.av_opt_child_next(Target, currentChild);

                if (currentChild is not null)
                    result.Add(new FFOptionsStore(currentChild));

            } while (currentChild is not null);

            return result;

        }
    }

    /// <summary>
    /// Gets the <see cref="INativeReference.Address"/> as a pointer.
    /// </summary>
    private void* Target => Address.ToPointer();

    /// <summary>
    /// Attempts to wrap a <see cref="INativeReference{T}"/> as an options-enabled,
    /// <see cref="IFFOptionsEnabled"/> object. This validates that the target data
    /// structure has a <see cref="AVClass"/> pointer defined in its first field.
    /// </summary>
    /// <typeparam name="T">The data structure to wrap.</typeparam>
    /// <param name="obj">The data structure wrapper to cast as </param>
    /// <param name="optionsObject"></param>
    /// <returns></returns>
    public static bool TryWrap<T>(INativeReference<T> obj, [MaybeNullWhen(false)] out IFFOptionsEnabled optionsObject)
        where T : unmanaged
    {
        optionsObject = null;
        if (obj is null || obj.IsNull) return false;
        if (!IsAVOptionsEnabled<T>())
            return false;

        optionsObject = new FFOptionsStore(obj.ToPointer());
        return true;
    }

    /// <summary>
    /// Checks if the given structure type represents an AVOPtions-enabled
    /// structure. Uses a non-blocking cache for fast access.
    /// </summary>
    /// <typeparam name="TStruct">The structure to check for the <see cref="AVClass"/> field.</typeparam>
    /// <returns>True if compatible. Otherwise, false.</returns>
    private static bool IsAVOptionsEnabled<TStruct>()
        where TStruct : unmanaged
    {
        if (!MediaClassFields.TryGetValue(typeof(TStruct), out var isValid))
        {
            var fields = typeof(TStruct).GetFields();
            isValid = fields is not null
                && fields.Length > 0
                && fields[0].FieldType == typeof(AVClass*);

            MediaClassFields[typeof(TStruct)] = isValid;
        }

        return isValid;
    }
}
