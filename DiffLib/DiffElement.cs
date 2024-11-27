// <copyright file="DiffElement.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This struct holds a single aligned element from the two collections given to <see cref="Diff.AlignElements{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements from the two collections compared.</typeparam>
/// <param name="elementIndexFromCollection1">Index of <see cref="ElementFromCollection1"/> in <c>Collection1</c>.</param>
/// <param name="elementFromCollection1">The aligned element from the first collection, or <see cref="Option.None{T}"/> if an element from the second collection could not be aligned with anything from the first.</param>
/// <param name="elementIndexFromCollection2">Index of <see cref="ElementFromCollection2"/> in <c>Collection2</c>.</param>
/// <param name="elementFromCollection2">The aligned element from the second collection, or <see cref="Option.None{T}"/> if an element from the first collection could not be aligned with anything from the second.</param>
/// <param name="operation">A <see cref="DiffOperation"/> specifying how <paramref name="elementFromCollection1"/> corresponds to <paramref name="elementFromCollection2"/>.</param>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
public readonly struct DiffElement<T>(int? elementIndexFromCollection1, Option<T> elementFromCollection1, int? elementIndexFromCollection2, Option<T> elementFromCollection2, DiffOperation operation) : IEquatable<DiffElement<T>>
{
    /// <summary>
    /// Gets index of <see cref="ElementFromCollection1"/> in <c>Collection1</c>.
    /// </summary>
    public int? ElementIndexFromCollection1 { get; } = elementIndexFromCollection1;

    /// <summary>
    /// Gets the aligned element from the first collection, or <see cref="Option.None{T}"/> if an element from the second collection could not be aligned with anything from the first.
    /// </summary>
    public Option<T> ElementFromCollection1 { get; } = elementFromCollection1;

    /// <summary>
    /// Gets index of <see cref="ElementFromCollection2"/> in <c>Collection2</c>.
    /// </summary>
    public int? ElementIndexFromCollection2 { get; } = elementIndexFromCollection2;

    /// <summary>
    /// Gets the aligned element from the second collection, or <see cref="Option.None{T}"/> if an element from the first collection could not be aligned with anything from the second.
    /// </summary>
    public Option<T> ElementFromCollection2 { get; } = elementFromCollection2;

    /// <summary>
    /// Gets a <see cref="DiffOperation"/> specifying how <see cref="ElementFromCollection1"/> corresponds to <see cref="ElementFromCollection2"/>.
    /// </summary>
    public DiffOperation Operation { get; } = operation;

    /// <summary>
    /// Implements the equality operator.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="other">The other.</param>
    /// <returns>The result.</returns>
    public static bool operator ==(DiffElement<T> element, DiffElement<T> other) => element.Equals(other);

    /// <summary>
    /// Implements the inequality operator.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="other">The other.</param>
    /// <returns>The result.</returns>
    public static bool operator !=(DiffElement<T> element, DiffElement<T> other) => !element.Equals(other);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public readonly bool Equals(DiffElement<T> other) => this.ElementIndexFromCollection1.Equals(other.ElementIndexFromCollection1) &&
        this.ElementFromCollection1.Equals(other.ElementFromCollection1) &&
        this.ElementIndexFromCollection2.Equals(other.ElementIndexFromCollection2) &&
        this.ElementFromCollection2.Equals(other.ElementFromCollection2) &&
        this.Operation == other.Operation;

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <returns>
    /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
    /// </returns>
    /// <param name="obj">Another object to compare to. </param><filterpriority>2.</filterpriority>
    public override bool Equals(object? obj) => obj is DiffElement<T> diffElement && this.Equals(diffElement);

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
#if NETSTANDARD2_1_OR_GREATER
        var hashCode = default(HashCode);
        hashCode.Add(this.ElementFromCollection1);
        hashCode.Add(this.ElementFromCollection2);
        hashCode.Add(this.ElementIndexFromCollection1);
        hashCode.Add(this.ElementIndexFromCollection2);
        hashCode.Add(this.Operation);
        return hashCode.ToHashCode();
#else
        unchecked
        {
            var hashCode = this.ElementFromCollection1.GetHashCode();
            hashCode = (hashCode * 397) ^ this.ElementFromCollection2.GetHashCode();
            hashCode = (hashCode * 397) ^ this.ElementIndexFromCollection1.GetHashCode();
            hashCode = (hashCode * 397) ^ this.ElementIndexFromCollection2.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)this.Operation;
            return hashCode;
        }
#endif
    }

    /// <inheritdoc />
    public override readonly string ToString() => this.Operation switch
    {
        DiffOperation.Match => $"same: {this.ElementFromCollection1}",
        DiffOperation.Insert => $"insert: {this.ElementFromCollection2}",
        DiffOperation.Delete => $"delete: {this.ElementFromCollection1}",
        DiffOperation.Replace => $"replace: {this.ElementFromCollection1} with: {this.ElementFromCollection2}",
        DiffOperation.Modify => $"modify: {this.ElementFromCollection1} to: {this.ElementFromCollection2}",
        _ => $"? {this.Operation}: {this.ElementFromCollection1}, {this.ElementFromCollection2}",
    };
}