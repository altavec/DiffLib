// <copyright file="Diff.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// Static API class for DiffLib.
/// </summary>
public static class Diff
{
    /// <summary>
    /// Calculate sections of differences from the two collections using the specified comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the two collections.</typeparam>
    /// <param name="collection1">The first collection.</param>
    /// <param name="collection2">The second collection.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use when determining if there is a match between <paramref name="collection1"/> and <paramref name="collection2"/>.</param>
    /// <returns>A collection of <see cref="DiffSection"/> values, containing the sections found.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="collection1"/> is <see langword="null"/>.</para>
    /// <para>- or -.</para>
    /// <para><paramref name="collection2"/> is <see langword="null"/>.</para>
    /// </exception>
    public static IEnumerable<DiffSection> CalculateSections<T>(IList<T> collection1, IList<T> collection2, IEqualityComparer<T>? comparer = default) => CalculateSections(collection1, collection2, new DiffOptions(), comparer);

    /// <summary>
    /// Calculate sections of differences from the two collections using the specified comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the two collections.</typeparam>
    /// <param name="collection1">The first collection.</param>
    /// <param name="collection2">The second collection.</param>
    /// <param name="options">A <see cref="DiffOptions"/> object specifying options to the diff algorithm, or <see langword="null"/> if defaults should be used.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use when determining if there is a match between <paramref name="collection1"/> and <paramref name="collection2"/>.</param>
    /// <returns>A collection of <see cref="DiffSection"/> values, containing the sections found.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="collection1"/> is <see langword="null"/>.</para>
    /// <para>- or -.</para>
    /// <para><paramref name="collection2"/> is <see langword="null"/>.</para>
    /// </exception>
    public static IEnumerable<DiffSection> CalculateSections<T>(IList<T> collection1, IList<T> collection2, DiffOptions? options, IEqualityComparer<T>? comparer = default)
    {
        if (collection1 is null)
        {
            throw new ArgumentNullException(nameof(collection1));
        }

        if (collection2 is null)
        {
            throw new ArgumentNullException(nameof(collection2));
        }

        comparer ??= EqualityComparer<T>.Default;

        options ??= new DiffOptions();

        return LongestCommonSubsectionDiff.Calculate(collection1, collection2, options, comparer);
    }

    /// <summary>
    /// Align the sections found by <see cref="CalculateSections{T}(IList{T},IList{T},DiffOptions,IEqualityComparer{T})"/> by trying to find out, within each section, which elements from one collection line up the best with elements from the other collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the two collections.</typeparam>
    /// <param name="collection1">The first collection.</param>
    /// <param name="collection2">The second collection.</param>
    /// <param name="diffSections">The section values found by <see cref="CalculateSections{T}(IList{T},IList{T},DiffOptions,IEqualityComparer{T})"/>.</param>
    /// <param name="aligner">An alignment strategy, provided through the <see cref="IDiffElementAligner{T}"/> interface.</param>
    /// <returns>A collection of <see cref="DiffElement{T}"/> values, specifying aligned elements on an element-by-element basis.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="collection1"/> is <see langword="null"/>.
    /// <para>- or -.</para>
    /// <para><paramref name="collection2"/> is <see langword="null"/>.</para>
    /// <para>- or -.</para>
    /// <para><paramref name="diffSections"/> is <see langword="null"/>.</para>
    /// <para>- or -.</para>
    /// <para><paramrsef name="aligner"/> is <see langword="null"/>.</para>
    /// </exception>
    public static IEnumerable<DiffElement<T>> AlignElements<T>(IList<T> collection1, IList<T> collection2, IEnumerable<DiffSection> diffSections, IDiffElementAligner<T> aligner) => (collection1, collection2, diffSections, aligner) switch
    {
        (null, _, _, _) => throw new ArgumentNullException(nameof(collection1)),
        (_, null, _, _) => throw new ArgumentNullException(nameof(collection2)),
        (_, _, null, _) => throw new ArgumentNullException(nameof(diffSections)),
        (_, _, _, null) => throw new ArgumentNullException(nameof(aligner)),
        _ => AlignElementsImplementation(collection1, collection2, diffSections, aligner),
    };

    private static IEnumerable<DiffElement<T>> AlignElementsImplementation<T>(IList<T> collection1, IList<T> collection2, IEnumerable<DiffSection> diffSections, IDiffElementAligner<T> aligner)
    {
        var start1 = 0;
        var start2 = 0;

        foreach (var section in diffSections)
        {
            if (section.IsMatch)
            {
                for (var index = 0; index < section.LengthInCollection1; index++)
                {
                    yield return new DiffElement<T>(start1, collection1[start1], start2, collection2[start2], DiffOperation.Match);
                    start1++;
                    start2++;
                }
            }
            else
            {
                foreach (var element in aligner.Align(collection1, start1, section.LengthInCollection1, collection2, start2, section.LengthInCollection2))
                {
                    yield return element;
                }

                start1 += section.LengthInCollection1;
                start2 += section.LengthInCollection2;
            }
        }
    }
}