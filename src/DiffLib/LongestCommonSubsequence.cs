// <copyright file="LongestCommonSubsequence.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal class LongestCommonSubsequence<T>(IList<T?> collection1, IList<T?> collection2, IEqualityComparer<T?> comparer)
{
    private readonly IList<T?> collection1 = collection1;
    private readonly IList<T?> collection2 = collection2;
    private readonly IEqualityComparer<T?> comparer = comparer;
    private readonly Dictionary<int, HashcodeOccurance> hashCodes2 = [];

    private bool firstHashCodes2 = true;
    private int hashCodes2Lower;
    private int hashCodes2Upper;

    public bool Find(int lower1, int upper1, int lower2, int upper2, out int position1, out int position2, out int length, int contextSize)
    {
        position1 = 0;
        position2 = 0;
        length = 0;

        this.EnsureHashCodes2(lower2, upper2);

        for (var index1 = lower1; index1 < upper1; index1++)
        {
            // Break early if there is no way we can find a better match
            if (index1 + length >= upper1)
            {
                break;
            }

            var hashcode = this.collection1[index1]?.GetHashCode(this.comparer) ?? 0;

            if (!this.hashCodes2.TryGetValue(hashcode, out var occurance))
            {
                continue;
            }

            while (occurance != null)
            {
                var index2 = occurance.Position;
                occurance = occurance.Next;

                if (index2 < lower2 || index2 + length >= upper2)
                {
                    continue;
                }

                // Don't bother with this if it doesn't match at the Nth element
                if (!this.comparer.Equals(this.collection1[index1 + length], this.collection2[index2 + length]))
                {
                    continue;
                }

                var matchLength = this.CountSimilarElements(index1, upper1, index2, upper2);
                if (matchLength > length && matchLength >= contextSize)
                {
                    position1 = index1;
                    position2 = index2;
                    length = matchLength;
                }

                if (index1 + length >= upper1)
                {
                    break;
                }
            }
        }

        return length > 0;
    }

    private int CountSimilarElements(int index1, int upper1, int index2, int upper2)
    {
        var count = 0;

        while (index1 < upper1 && index2 < upper2 && this.comparer.Equals(this.collection1[index1], this.collection2[index2]))
        {
            count++;
            index1++;
            index2++;
        }

        return count;
    }

    private void EnsureHashCodes2(int lower, int upper)
    {
        if (this.firstHashCodes2)
        {
            this.firstHashCodes2 = false;
            this.hashCodes2Lower = lower;
            this.hashCodes2Upper = upper;

            for (var index = lower; index < upper; index++)
            {
                this.AddHashCode2(index, this.collection2[index]?.GetHashCode(this.comparer) ?? 0);
            }

            return;
        }

        while (this.hashCodes2Lower > lower)
        {
            this.hashCodes2Lower--;
            this.AddHashCode2(this.hashCodes2Lower, this.collection2[this.hashCodes2Lower]?.GetHashCode(this.comparer) ?? 0);
        }

        while (this.hashCodes2Upper < upper)
        {
            this.AddHashCode2(this.hashCodes2Upper, this.collection2[this.hashCodes2Upper]?.GetHashCode(this.comparer) ?? 0);
            this.hashCodes2Upper++;
        }
    }

    private void AddHashCode2(int position, int hashcode)
    {
        if (this.hashCodes2.TryGetValue(hashcode, out var occurance))
        {
            occurance.Next = new HashcodeOccurance(position, occurance.Next);
        }
        else
        {
            this.hashCodes2[hashcode] = new HashcodeOccurance(position, null);
        }
    }
}