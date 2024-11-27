// <copyright file="DiffSectionMergeComparer.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal class DiffSectionMergeComparer<T>(IEqualityComparer<T?> comparer) : IEqualityComparer<DiffElement<T>>
{
    private readonly IEqualityComparer<T?> comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

    public bool Equals(DiffElement<T> x, DiffElement<T> y) => this.comparer.Equals(GetElement(x), GetElement(y));

    public int GetHashCode(DiffElement<T> obj) => GetElement(obj) is { } v
        ? this.comparer.GetHashCode(v)
        : default;

    private static T? GetElement(DiffElement<T> diffElement) => diffElement switch
    {
        { ElementFromCollection1: { HasValue: true } element } => element.Value,
        { ElementFromCollection2: { HasValue: true } element } => element.Value,
        _ => default,
    };
}