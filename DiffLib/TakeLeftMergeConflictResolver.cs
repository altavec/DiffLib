﻿// <copyright file="TakeLeftMergeConflictResolver.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This implementation of <see cref="IMergeConflictResolver{T}"/> always takes the left side.
/// </summary>
/// <typeparam name="T">The type of elements in the collections being merged.</typeparam>
public class TakeLeftMergeConflictResolver<T> : IMergeConflictResolver<T>
{
    /// <inheritdoc />
    public IEnumerable<T> Resolve(IList<T> commonBase, IList<T> left, IList<T> right) => left;
}