namespace DiffLib;

internal class Merge<T> : IEnumerable<T?>
{
    private readonly IMergeConflictResolver<T?> _ConflictResolver;
    private readonly List<DiffSection> _MergeSections;

    private readonly List<DiffElement<T>> _DiffCommonBaseToLeft;

    private readonly List<DiffElement<T>> _DiffCommonBaseToRight;

    public Merge(IList<T> commonBase, IList<T> left, IList<T> right, IDiffElementAligner<T> aligner, IMergeConflictResolver<T?> conflictResolver, IEqualityComparer<T?> comparer, DiffOptions diffOptions)
    {
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

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        if (diffOptions is null)
        {
            throw new ArgumentNullException(nameof(diffOptions));
        }

        this._ConflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));

        var diffCommonBaseToLeft = Diff.AlignElements(commonBase, left, Diff.CalculateSections(commonBase, left, diffOptions, comparer), aligner).ToList();
        this._DiffCommonBaseToLeft = diffCommonBaseToLeft;

        var diffCommonBaseToRight = Diff.AlignElements(commonBase, right, Diff.CalculateSections(commonBase, right, diffOptions, comparer), aligner).ToList();
        this._DiffCommonBaseToRight = diffCommonBaseToRight;

        var mergeSections = Diff.CalculateSections(diffCommonBaseToLeft!, diffCommonBaseToRight!, diffOptions, new DiffSectionMergeComparer<T>(comparer)).ToList();
        this._MergeSections = mergeSections;
    }

    public IEnumerator<T?> GetEnumerator()
    {
        var leftIndex = 0;
        var rightIndex = 0;
        foreach (var section in this._MergeSections)
        {
            if (section.IsMatch)
            {
                for (var index = 0; index < section.LengthInCollection1; index++)
                {
                    foreach (var item in this.ResolveMatchingElementFromBothSides(leftIndex++, rightIndex++))
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                foreach (var item in this.ProcessNonMatchingElementsFromBothSides(section, rightIndex, leftIndex))
                {
                    yield return item;
                }

                leftIndex += section.LengthInCollection1;
                rightIndex += section.LengthInCollection2;
            }
        }
    }

    private IEnumerable<T?> ProcessNonMatchingElementsFromBothSides(DiffSection section, int rightIndex, int leftIndex)
    {
        if (section.LengthInCollection1 == 0)
        {
            // right side inserted, right side wins
            for (var index = 0; index < section.LengthInCollection2; index++)
            {
                yield return this._DiffCommonBaseToRight[rightIndex + index].ElementFromCollection2.Value;
            }
        }
        else if (section.LengthInCollection2 == 0)
        {
            // left side inserted, left side wins
            for (var index = 0; index < section.LengthInCollection1; index++)
            {
                yield return this._DiffCommonBaseToLeft[leftIndex + index].ElementFromCollection2.Value;
            }
        }
        else
        {
            var leftSide = new List<T?>();
            for (var index = 0; index < section.LengthInCollection1; index++)
            {
                leftSide.Add(this._DiffCommonBaseToLeft[leftIndex + index].ElementFromCollection2.Value);
            }

            var rightSide = new List<T?>();
            for (var index = 0; index < section.LengthInCollection2; index++)
            {
                rightSide.Add(this._DiffCommonBaseToRight[rightIndex + index].ElementFromCollection2.Value);
            }

            foreach (var item in this._ConflictResolver.Resolve([], leftSide, rightSide))
            {
                yield return item;
            }
        }
    }

    private IEnumerable<T?> ResolveMatchingElementFromBothSides(int leftIndex, int rightIndex)
    {
        var commonBase = this._DiffCommonBaseToLeft[leftIndex].ElementFromCollection1.Value;

        var leftOp = this._DiffCommonBaseToLeft[leftIndex].Operation;
        if (leftOp == DiffOperation.Replace)
        {
            leftOp = DiffOperation.Modify;
        }

        var leftSide = this._DiffCommonBaseToLeft[leftIndex].ElementFromCollection2.GetValueOrDefault()!;

        var rightOp = this._DiffCommonBaseToRight[rightIndex].Operation;
        if (rightOp == DiffOperation.Replace)
        {
            rightOp = DiffOperation.Modify;
        }

        var rightSide = this._DiffCommonBaseToRight[rightIndex].ElementFromCollection2.GetValueOrDefault()!;

        var resolution = this.GetResolution(commonBase, leftOp, leftSide, rightOp, rightSide);
        return resolution;
    }

    private IEnumerable<T?> GetResolution(T? commonBase, DiffOperation leftOp, T? leftSide, DiffOperation rightOp, T? rightSide)
    {
        switch (leftOp)
        {
            case DiffOperation.Match:
                switch (rightOp)
                {
                    case DiffOperation.Match:
                        return [leftSide];
                    case DiffOperation.Insert:
                        break;
                    case DiffOperation.Delete:
                        return [];
                    case DiffOperation.Replace:
                    case DiffOperation.Modify:
                        return [rightSide];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rightOp), rightOp, null);
                }

                break;

            case DiffOperation.Insert:
                break;

            case DiffOperation.Delete:
                switch (rightOp)
                {
                    case DiffOperation.Match:
                        return [];
                    case DiffOperation.Insert:
                        break;
                    case DiffOperation.Delete:
                        return [];
                    case DiffOperation.Replace:
                    case DiffOperation.Modify:
                        return this._ConflictResolver.Resolve([commonBase], [], [rightSide]);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rightOp), rightOp, null);
                }

                break;

            case DiffOperation.Replace:
            case DiffOperation.Modify:
                switch (rightOp)
                {
                    case DiffOperation.Match:
                        return [leftSide];
                    case DiffOperation.Insert:
                        break;
                    case DiffOperation.Delete:
                        return this._ConflictResolver.Resolve([commonBase], [leftSide], []);
                    case DiffOperation.Replace:
                    case DiffOperation.Modify:
                        return this._ConflictResolver.Resolve([commonBase], [leftSide], [rightSide]);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rightOp), rightOp, null);
                }

                break;
        }

        throw new MergeConflictException($"Unable to process {leftOp} vs. {rightOp}", [commonBase], [leftSide], [rightSide]);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();
}