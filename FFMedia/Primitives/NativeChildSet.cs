using System.Collections;

namespace FFMedia.Primitives;

/// <summary>
/// Base implementation that represents a set of
/// native references belonging to a parent data structure.
/// </summary>
/// <typeparam name="TParent">The type of object that owns the children.</typeparam>
/// <typeparam name="TChild">The type of the child objects.</typeparam>
public abstract unsafe class NativeChildSet<TParent, TChild> : IReadOnlyList<TChild>
    where TParent : INativeReference
    where TChild : INativeReference
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeChildSet{TParent, TChild}"/> class.
    /// </summary>
    /// <param name="parent">The parent owning the children.</param>
    protected NativeChildSet(TParent parent)
    {
        ArgumentNullException.ThrowIfNull(parent);
        if (parent.IsNull)
            throw new ArgumentNullException(nameof(parent));

        Parent = parent;
    }

    /// <summary>
    /// Gets the parent object.
    /// </summary>
    public TParent Parent { get; }

    /// <inheritdoc />
    public abstract TChild this[int index] { get; set; }

    /// <inheritdoc />
    public abstract int Count { get; }

    /// <summary>
    /// Gets the index location of the given element.
    /// Comparison is performed by <see cref="INativeReference.Address"/>.
    /// </summary>
    /// <param name="child">The child element.</param>
    /// <returns>The 0-based index or -1 if not found.</returns>
    public int IndexOf(TChild child)
    {
        if (child is null || child.IsNull)
            return -1;

        for (var i = 0; i < Count; i++)
        {
            if (this[i] is not null && this[i].Address == child.Address)
                return i;
        }

        return -1;
    }

    /// <inheritdoc />
    public IEnumerator<TChild> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    /// <summary>
    /// Swaps the elements between the 2 given 0-based index locations.
    /// This call is bound-checked.
    /// </summary>
    /// <param name="indexA">The first item index to swap.</param>
    /// <param name="indexB">The second item index to swap.</param>
    /// <remarks>Port of FFSWAP</remarks>
    public virtual void Swap(int indexA, int indexB)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(indexA, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(indexA, Count);
        ArgumentOutOfRangeException.ThrowIfLessThan(indexB, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(indexB, Count);

        if (indexA == indexB)
            return;

        (this[indexA], this[indexB]) = (this[indexB], this[indexA]);
    }


    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
