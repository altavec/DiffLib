// <copyright file="TakeRightThenLeftIfLeftDiffersFromRightMergeConflictResolver.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This implementation of <see cref="IMergeConflictResolver{T}"/> resolves a conflict by taking the left side and then taking the right side. In the case where both cases are identical, only the left side is taken.
/// </summary>
/// <typeparam name="T">The type of elements in the collections being merged.</typeparam>
/// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use when determining if elements of the left side of a conflict matches those on the right side. If <see langword="null"/> then <see cref="EqualityComparer{T}.Default"/> is used.</param>
public class TakeRightThenLeftIfLeftDiffersFromRightMergeConflictResolver<T>(IEqualityComparer<T>? equalityComparer = default) : IMergeConflictResolver<T>
{
    private readonly IEqualityComparer<T> equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

    /// <inheritdoc />
    public IEnumerable<T> Resolve(IList<T> commonBase, IList<T> left, IList<T> right)
    {
        foreach (var item in right)
        {
            yield return item;
        }

        if (left.SequenceEqual(right, this.equalityComparer))
        {
            yield break;
        }

        foreach (var item in left)
        {
            yield return item;
        }
    }
}