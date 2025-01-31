// <copyright file="StringSimilarityDiffElementAligner.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib.Alignment;

/// <summary>
/// This class implements a <see cref="IDiffElementAligner{T}"/> strategy that will try
/// to work out the best way to align two portions, depending on individual string
/// similarity.
/// </summary>
/// <remarks>
/// String similarity will apply a diff between the two strings and count how much of
/// the two strings were considered matching, in relation to the two strings in total.
/// </remarks>
/// <param name="modificationThreshold">
/// A threshold value used to determine if aligned elements are considered replacements or modifications. If
/// two items are more similar than the threshold specifies (similarity > threshold), then it results in
/// a <see cref="DiffOperation.Modify"/>, otherwise it results in a <see cref="DiffOperation.Replace"/>.
/// </param>
public class StringSimilarityDiffElementAligner(double modificationThreshold = 0.3333) : IDiffElementAligner<string?>
{
    private readonly ElementSimilarityDiffElementAligner<string> aligner = new(StringSimilarity, modificationThreshold);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="collection1"/> is <c>null</c>.</para>
    /// <para>- or -.</para>
    /// <para><paramref name="collection2"/> is <c>null</c>.</para>
    /// </exception>
    public IEnumerable<DiffElement<string?>> Align(IList<string?> collection1, int start1, int length1, IList<string?> collection2, int start2, int length2)
        => this.aligner.Align(collection1, start1, length1, collection2, start2, length2);

    private static double StringSimilarity(string? element1, string? element2)
    {
        element1 ??= string.Empty;
        element2 ??= string.Empty;

        if (ReferenceEquals(element1, element2))
        {
            return 1.0;
        }

        if (element1.Length is 0 && element2.Length is 0)
        {
            return 1.0;
        }

        if (element1.Length is 0 || element2.Length is 0)
        {
            return 0.0;
        }

        var element1Array = element1.ToCharArray();
        var element2Array = element2.ToCharArray();

        var diffSections = Diff.CalculateSections(element1Array, element2Array, new DiffOptions()).ToArray();
        var matchLength = diffSections.Where(section => section.IsMatch).Sum(section => section.LengthInCollection1);
        return matchLength * 2.0 / (element1.Length + element2.Length + 0.0);
    }
}