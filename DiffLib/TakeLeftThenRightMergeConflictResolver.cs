// <copyright file="TakeLeftThenRightMergeConflictResolver.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This implementation of <see cref="IMergeConflictResolver{T}"/> takes the left side then takes the right side.
/// </summary>
/// <typeparam name="T">The type of elements in the collections being merged.</typeparam>
public class TakeLeftThenRightMergeConflictResolver<T> : IMergeConflictResolver<T>
{
    /// <inheritdoc />
    public IEnumerable<T> Resolve(IList<T> commonBase, IList<T> left, IList<T> right)
    {
        foreach (var item in left)
        {
            yield return item;
        }

        foreach (var item in right)
        {
            yield return item;
        }
    }
}