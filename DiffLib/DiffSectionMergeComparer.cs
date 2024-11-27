namespace DiffLib;

internal class DiffSectionMergeComparer<T> : IEqualityComparer<DiffElement<T>>
{
    private readonly IEqualityComparer<T?> _Comparer;

    public DiffSectionMergeComparer(IEqualityComparer<T?> comparer) => this._Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

    public bool Equals(DiffElement<T> x, DiffElement<T> y) => this._Comparer.Equals(this.GetElement(x), this.GetElement(y));

    public int GetHashCode(DiffElement<T> obj) => this._Comparer.GetHashCode(this.GetElement(obj));

    private T? GetElement(DiffElement<T> diffElement) => diffElement switch
    {
        { ElementFromCollection1: { HasValue: true } element } => element.Value,
        { ElementFromCollection2: { HasValue: true } element } => element.Value,
        _ => default,
    };
}