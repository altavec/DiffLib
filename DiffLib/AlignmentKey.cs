// <copyright file="AlignmentKey.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
internal readonly struct AlignmentKey(int position1, int position2) : IEquatable<AlignmentKey>
{
    public int Position1 { get; } = position1;

    public int Position2 { get; } = position2;

    public readonly bool Equals(AlignmentKey other) => this.Position1 == other.Position1 && this.Position2 == other.Position2;

    public override bool Equals(object? obj) => obj is AlignmentKey alignmentKey && this.Equals(alignmentKey);

    public override readonly int GetHashCode() => unchecked((this.Position1 * 397) ^ this.Position2);
}