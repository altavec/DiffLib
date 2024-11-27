// <copyright file="AlignmentKey.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal readonly struct AlignmentKey : IEquatable<AlignmentKey>
{
    public AlignmentKey(int position1, int position2)
    {
        this.Position1 = position1;
        this.Position2 = position2;
    }

    public int Position1 { get; }

    public int Position2 { get; }

    public readonly bool Equals(AlignmentKey other) => this.Position1 == other.Position1 && this.Position2 == other.Position2;

    public override bool Equals(object? obj) => obj is not null && obj is AlignmentKey alignmentKey && this.Equals(alignmentKey);

    public override readonly int GetHashCode() => unchecked((this.Position1 * 397) ^ this.Position2);
}