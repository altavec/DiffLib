namespace DiffLib;

using static System.Math;

/// <summary>
/// This class can be used as a parameter to <see cref="Diff.AlignElements{T}"/>.
/// It will output a number of replace operations, depending on overlap, and then anything leftover that is present in the first collection
/// as a sequence of delete operations, and in the second collection as a sequence of insert operations.
/// </summary>
/// <typeparam name="T">The type of elements in the two collections.</typeparam>
public class BasicReplaceInsertDeleteDiffElementAligner<T> : BasicInsertDeleteDiffElementAligner<T>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="collection1"/> is <see langword="null"/>.</para>
    /// <para>- or -</para>
    /// <para><paramref name="collection2"/> is <see langword="null"/>.</para>
    /// </exception>
    public override IEnumerable<DiffElement<T>> Align(IList<T> collection1, int start1, int length1, IList<T> collection2, int start2, int length2)
    {
        var replaceCount = Min(length1, length2);
        for (var index = 0; index < replaceCount; index++)
        {
            yield return new DiffElement<T>(start1 + index, collection1[start1 + index], start2 + index, collection2[start2 + index], DiffOperation.Replace);
        }

        foreach (var element in base.Align(collection1, start1 + replaceCount, length1 - replaceCount, collection2, start2 + replaceCount, length2 - replaceCount))
        {
            yield return element;
        }
    }
}