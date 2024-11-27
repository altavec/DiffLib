namespace DiffLib;

internal class DiffSectionMergeComparer<T> : IEqualityComparer<DiffElement<T>>
{
    private readonly IEqualityComparer<T?> _Comparer;

    public DiffSectionMergeComparer(IEqualityComparer<T?> comparer)
    {
        this._Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public bool Equals(DiffElement<T> x, DiffElement<T> y)
    {
        return this._Comparer.Equals(this.GetElement(x), this.GetElement(y));
    }

    public int GetHashCode(DiffElement<T> obj)
    {
        return this._Comparer.GetHashCode(this.GetElement(obj));
    }

    private T? GetElement(DiffElement<T> diffElement)
    {
        if (diffElement.ElementFromCollection1.HasValue)
            return diffElement.ElementFromCollection1.Value;

        if (diffElement.ElementFromCollection2.HasValue)
            return diffElement.ElementFromCollection2.Value;

        return default(T);
    }
}