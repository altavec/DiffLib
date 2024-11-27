// <copyright file="BasicInsertDeleteDiffElementAligner.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This class can be used as a parameter to <see cref="Diff.AlignElements{T}"/>. It will basically output anything present in the first collection
/// as a sequence of delete operations, and anything present in the second collection as a sequence of insert operations.
/// </summary>
/// <typeparam name="T">
/// The type of elements in the two collections.
/// </typeparam>
public class BasicInsertDeleteDiffElementAligner<T> : IDiffElementAligner<T>
{
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// <para><paramref name="collection1"/> is <see langword="null"/>.</para>
    /// <para>- or -.</para>
    /// <para><paramref name="collection2"/> is <see langword="null"/>.</para>
    /// </exception>
    public virtual IEnumerable<DiffElement<T>> Align(IList<T> collection1, int start1, int length1, IList<T> collection2, int start2, int length2)
    {
        return (collection1, collection2) switch
        {
            (null, _) => throw new ArgumentNullException(nameof(collection1)),
            (_, null) => throw new ArgumentNullException(nameof(collection2)),
            _ => AlignCore(collection1, start1, length1, collection2, start2, length2),
        };

        static IEnumerable<DiffElement<T>> AlignCore(IList<T> collection1, int start1, int length1, IList<T> collection2, int start2, int length2)
        {
            for (var index = 0; index < length1; index++)
            {
                yield return new DiffElement<T>(start1 + index, collection1[start1 + index], elementIndexFromCollection2: null, Option.None<T>(), DiffOperation.Delete);
            }

            for (var index = 0; index < length2; index++)
            {
                yield return new DiffElement<T>(elementIndexFromCollection1: null, Option.None<T>(), start2 + index, collection2[start2 + index], DiffOperation.Insert);
            }
        }
    }
}