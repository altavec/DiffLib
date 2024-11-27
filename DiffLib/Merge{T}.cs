// <copyright file="Merge{T}.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal class Merge<T> : IEnumerable<T?>
{
    private readonly IMergeConflictResolver<T?> conflictResolver;
    private readonly List<DiffSection> mergeSections;

    private readonly List<DiffElement<T>> diffCommonBaseToLeft;

    private readonly List<DiffElement<T>> diffCommonBaseToRight;

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

        this.conflictResolver = conflictResolver ?? throw new ArgumentNullException(nameof(conflictResolver));

        var diffCommonBaseToLeft = Diff.AlignElements(commonBase, left, Diff.CalculateSections(commonBase, left, diffOptions, comparer), aligner).ToList();
        this.diffCommonBaseToLeft = diffCommonBaseToLeft;

        var diffCommonBaseToRight = Diff.AlignElements(commonBase, right, Diff.CalculateSections(commonBase, right, diffOptions, comparer), aligner).ToList();
        this.diffCommonBaseToRight = diffCommonBaseToRight;

        var mergeSections = Diff.CalculateSections(diffCommonBaseToLeft!, diffCommonBaseToRight!, diffOptions, new DiffSectionMergeComparer<T>(comparer)).ToList();
        this.mergeSections = mergeSections;
    }

    public IEnumerator<T?> GetEnumerator()
    {
        var leftIndex = 0;
        var rightIndex = 0;
        foreach (var section in this.mergeSections)
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

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

    private IEnumerable<T?> ProcessNonMatchingElementsFromBothSides(DiffSection section, int rightIndex, int leftIndex)
    {
        if (section.LengthInCollection1 == 0)
        {
            // right side inserted, right side wins
            for (var index = 0; index < section.LengthInCollection2; index++)
            {
                yield return this.diffCommonBaseToRight[rightIndex + index].ElementFromCollection2.Value;
            }
        }
        else if (section.LengthInCollection2 == 0)
        {
            // left side inserted, left side wins
            for (var index = 0; index < section.LengthInCollection1; index++)
            {
                yield return this.diffCommonBaseToLeft[leftIndex + index].ElementFromCollection2.Value;
            }
        }
        else
        {
            var leftSide = new List<T?>();
            for (var index = 0; index < section.LengthInCollection1; index++)
            {
                leftSide.Add(this.diffCommonBaseToLeft[leftIndex + index].ElementFromCollection2.Value);
            }

            var rightSide = new List<T?>();
            for (var index = 0; index < section.LengthInCollection2; index++)
            {
                rightSide.Add(this.diffCommonBaseToRight[rightIndex + index].ElementFromCollection2.Value);
            }

            foreach (var item in this.conflictResolver.Resolve([], leftSide, rightSide))
            {
                yield return item;
            }
        }
    }

    private IEnumerable<T?> ResolveMatchingElementFromBothSides(int leftIndex, int rightIndex)
    {
        var commonBase = this.diffCommonBaseToLeft[leftIndex].ElementFromCollection1.Value;

        var leftOp = this.diffCommonBaseToLeft[leftIndex].Operation;
        if (leftOp == DiffOperation.Replace)
        {
            leftOp = DiffOperation.Modify;
        }

        var leftSide = this.diffCommonBaseToLeft[leftIndex].ElementFromCollection2.GetValueOrDefault()!;

        var rightOp = this.diffCommonBaseToRight[rightIndex].Operation;
        if (rightOp == DiffOperation.Replace)
        {
            rightOp = DiffOperation.Modify;
        }

        var rightSide = this.diffCommonBaseToRight[rightIndex].ElementFromCollection2.GetValueOrDefault()!;

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
                        return this.conflictResolver.Resolve([commonBase], [], [rightSide]);
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
                        return this.conflictResolver.Resolve([commonBase], [leftSide], []);
                    case DiffOperation.Replace:
                    case DiffOperation.Modify:
                        return this.conflictResolver.Resolve([commonBase], [leftSide], [rightSide]);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rightOp), rightOp, null);
                }

                break;
        }

        throw new MergeConflictException($"Unable to process {leftOp} vs. {rightOp}", [commonBase], [leftSide], [rightSide]);
    }
}