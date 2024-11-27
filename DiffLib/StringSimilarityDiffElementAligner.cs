namespace DiffLib;

/// <summary>
/// This class implements a <see cref="IDiffElementAligner{T}"/> strategy that will try
/// to work out the best way to align two portions, depending on individual string
/// similarity.
/// </summary>
/// <remarks>
/// String similarity will apply a diff between the two strings and count how much of
/// the two strings were considered matching, in relation to the two strings in total.
/// </remarks>
public class StringSimilarityDiffElementAligner : IDiffElementAligner<string?>
{
    private readonly IDiffElementAligner<string?> _Aligner;

    /// <summary>
    /// Construct a new <see cref="StringSimilarity"/>.
    /// </summary>
    /// <param name="modificationThreshold">
    /// <para>A threshold value used to determine if aligned elements are considered replacements or modifications.</para>
    /// <para>If two items are more similar than the threshold specifies (similarity > threshold), then it results in a <see cref="DiffOperation.Modify"/>, otherwise it results in a <see cref="DiffOperation.Replace"/>.</para>
    /// </param>
    public StringSimilarityDiffElementAligner(double modificationThreshold = 0.3333) => this._Aligner = new ElementSimilarityDiffElementAligner<string?>(StringSimilarity, modificationThreshold);

    private static double StringSimilarity(string? element1, string? element2)
    {
        element1 ??= String.Empty;
        element2 ??= String.Empty;

        if (ReferenceEquals(element1, element2))
        {
            return 1.0;
        }

        if (element1.Length == 0 && element2.Length == 0)
        {
            return 1.0;
        }

        if (element1.Length == 0 || element2.Length == 0)
        {
            return 0.0;
        }

        var element1Array = element1.ToCharArray();
        var element2Array = element2.ToCharArray();

        var diffSections = Diff.CalculateSections(element1Array, element2Array, new DiffOptions()).ToArray();
        var matchLength = diffSections.Where(section => section.IsMatch).Sum(section => section.LengthInCollection1);
        return matchLength * 2.0 / (element1.Length + element2.Length + 0.0);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="collection1"/> is <see langword="null"/>.</para>
    /// <para>- or -</para>
    /// <para><paramref name="collection2"/> is <see langword="null"/>.</para>
    /// </exception>
    public IEnumerable<DiffElement<string?>> Align(IList<string?> collection1, int start1, int length1, IList<string?> collection2, int start2, int length2) => this._Aligner.Align(collection1, start1, length1, collection2, start2, length2);
}