namespace DiffLib;

internal class LongestCommonSubsequence<T>
{
    private readonly IList<T> _Collection1;

    private readonly IList<T> _Collection2;

    private readonly IEqualityComparer<T> _Comparer;

    private readonly Dictionary<int, HashcodeOccurance> _HashCodes2 = [];

    private bool _FirstHashCodes2 = true;
    private int _HashCodes2Lower;
    private int _HashCodes2Upper;

    public LongestCommonSubsequence(IList<T> collection1, IList<T> collection2, IEqualityComparer<T> comparer)
    {
        this._Collection1 = collection1;
        this._Collection2 = collection2;

        this._Comparer = comparer;
    }

    public bool Find(int lower1, int upper1, int lower2, int upper2, out int position1, out int position2, out int length)
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

            var hashcode = this._Collection1[index1]?.GetHashCode(this._Comparer) ?? 0;

            if (!this._HashCodes2.TryGetValue(hashcode, out var occurance))
            {
                continue;
            }

            while (occurance is not null)
            {
                var index2 = occurance.Position;
                occurance = occurance.Next;

                if (index2 < lower2 || index2 + length >= upper2)
                {
                    continue;
                }

                // Don't bother with this if it doesn't match at the Nth element
                if (!this._Comparer.Equals(this._Collection1[index1 + length], this._Collection2[index2 + length]))
                {
                    continue;
                }

                var matchLength = this.CountSimilarElements(index1, upper1, index2, upper2);
                if (matchLength > length)
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

        while (index1 < upper1 && index2 < upper2 && this._Comparer.Equals(this._Collection1[index1], this._Collection2[index2]))
        {
            count++;
            index1++;
            index2++;
        }

        return count;
    }

    private void EnsureHashCodes2(int lower, int upper)
    {
        if (this._FirstHashCodes2)
        {
            this._FirstHashCodes2 = false;
            this._HashCodes2Lower = lower;
            this._HashCodes2Upper = upper;

            for (var index = lower; index < upper; index++)
            {
                this.AddHashCode2(index, this._Collection2[index]?.GetHashCode(this._Comparer) ?? 0);
            }

            return;
        }

        while (this._HashCodes2Lower > lower)
        {
            this._HashCodes2Lower--;
            this.AddHashCode2(this._HashCodes2Lower, this._Collection2[this._HashCodes2Lower]?.GetHashCode(this._Comparer) ?? 0);
        }

        while (this._HashCodes2Upper < upper)
        {
            this.AddHashCode2(this._HashCodes2Upper, this._Collection2[this._HashCodes2Upper]?.GetHashCode(this._Comparer) ?? 0);
            this._HashCodes2Upper++;
        }
    }

    private void AddHashCode2(int position, int hashcode)
    {
        if (this._HashCodes2.TryGetValue(hashcode, out var occurance))
        {
            occurance.Next = new HashcodeOccurance(position, occurance.Next);
        }
        else
        {
            this._HashCodes2[hashcode] = new HashcodeOccurance(position, null);
        }
    }
}