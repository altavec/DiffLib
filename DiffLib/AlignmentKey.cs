namespace DiffLib;

internal readonly struct AlignmentKey : IEquatable<AlignmentKey>
{
    public AlignmentKey(int position1, int position2)
    {
        this.Position1 = position1;
        this.Position2 = position2;
    }

    public int Position1
    {
        get;
    }

    public int Position2
    {
        get;
    }

    public readonly bool Equals(AlignmentKey other) => this.Position1 == other.Position1 && this.Position2 == other.Position2;

    public override bool Equals(object? obj) => !ReferenceEquals(null, obj) && (obj is AlignmentKey && this.Equals((AlignmentKey)obj));

    public override readonly int GetHashCode() => unchecked((this.Position1 * 397) ^ this.Position2);
}