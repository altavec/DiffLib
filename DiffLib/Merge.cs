﻿// <copyright file="Merge.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// Static API class for the merge portion of DiffLib.
/// </summary>
public static class Merge
{
    /// <summary>
    /// Performs a merge using a 3-way merge, returning the final merged output.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections being merged.</typeparam>
    /// <param name="commonBase">The common base/ancestor of both <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <param name="left">The left side being merged with the <paramref name="right"/>.</param>
    /// <param name="right">The right side being merged with the <paramref name="left"/>.</param>
    /// <param name="aligner">A <see cref="IDiffElementAligner{T}"/> implementation that will be responsible for lining up common vs. left and common vs. right as well as left vs. right during the merge.</param>
    /// <param name="conflictResolver">A <see cref="IMergeConflictResolver{T}"/> implementation that will be used to resolve conflicting modifications between left and right.</param>
    /// <param name="comparer">A <see cref="IEqualityComparer{T}"/> implementation that will be used to compare elements of all the collections. If <see langword="null"/> is specified then <see cref="EqualityComparer{T}.Default"/> will be used.</param>
    /// <returns>The final merged collection of elements from <paramref name="left"/> and <paramref name="right"/>.</returns>
    /// <exception cref="MergeConflictException">The <paramref name="conflictResolver"/> threw a <see cref="MergeConflictException"/> to indicate a failure to resolve a conflict.</exception>
    public static IEnumerable<T?> Perform<T>(IList<T> commonBase, IList<T> left, IList<T> right, IDiffElementAligner<T> aligner, IMergeConflictResolver<T?> conflictResolver, IEqualityComparer<T?>? comparer = default) => Perform(commonBase, left, right, new DiffOptions(), aligner, conflictResolver, comparer);

    /// <summary>
    /// Performs a merge using a 3-way merge, returning the final merged output.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections being merged.</typeparam>
    /// <param name="commonBase">The common base/ancestor of both <paramref name="left"/> and <paramref name="right"/>.</param>
    /// <param name="left">The left side being merged with the <paramref name="right"/>.</param>
    /// <param name="right">The right side being merged with the <paramref name="left"/>.</param>
    /// <param name="diffOptions">A <see cref="DiffOptions"/> object specifying options to the diff algorithm, or <see langword="null"/> if defaults should be used.</param>
    /// <param name="aligner">A <see cref="IDiffElementAligner{T}"/> implementation that will be responsible for lining up common vs. left and common vs. right as well as left vs. right during the merge.</param>
    /// <param name="conflictResolver">A <see cref="IMergeConflictResolver{T}"/> implementation that will be used to resolve conflicting modifications between left and right.</param>
    /// <param name="comparer">A <see cref="IEqualityComparer{T}"/> implementation that will be used to compare elements of all the collections. If <see langword="null"/> is specified then <see cref="EqualityComparer{T}.Default"/> will be used.</param>
    /// <returns>The final merged collection of elements from <paramref name="left"/> and <paramref name="right"/>.</returns>
    /// <exception cref="MergeConflictException">The <paramref name="conflictResolver"/> threw a <see cref="MergeConflictException"/> to indicate a failure to resolve a conflict.</exception>
    public static IEnumerable<T?> Perform<T>(IList<T> commonBase, IList<T> left, IList<T> right, DiffOptions? diffOptions, IDiffElementAligner<T> aligner, IMergeConflictResolver<T?> conflictResolver, IEqualityComparer<T?>? comparer = default)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(commonBase);
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        ArgumentNullException.ThrowIfNull(aligner);
        ArgumentNullException.ThrowIfNull(conflictResolver);
#else
        if (commonBase is null)
        {
            throw new ArgumentNullException(nameof(commonBase));
        }

        if (left is null)
        {
            throw new ArgumentNullException(nameof(left));
        }

        if (right is null)
        {
            throw new ArgumentNullException(nameof(right));
        }

        if (aligner is null)
        {
            throw new ArgumentNullException(nameof(aligner));
        }

        if (conflictResolver is null)
        {
            throw new ArgumentNullException(nameof(conflictResolver));
        }
#endif

        diffOptions ??= new DiffOptions();
        comparer ??= EqualityComparer<T?>.Default;

        return new Merge<T>(commonBase, left, right, aligner, conflictResolver, comparer, diffOptions);
    }
}