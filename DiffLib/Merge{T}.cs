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
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(commonBase);
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        ArgumentNullException.ThrowIfNull(aligner);
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(diffOptions);
        ArgumentNullException.ThrowIfNull(conflictResolver);
#else
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

        if (conflictResolver is null)
        {
            throw new ArgumentNullException(nameof(conflictResolver));
        }
#endif

        this.conflictResolver = conflictResolver;

        var commonBaseToLeft = Diff.AlignElements(commonBase, left, Diff.CalculateSections(commonBase, left, diffOptions, comparer), aligner).ToList();
        this.diffCommonBaseToLeft = commonBaseToLeft;

        var commonBaseToRight = Diff.AlignElements(commonBase, right, Diff.CalculateSections(commonBase, right, diffOptions, comparer), aligner).ToList();
        this.diffCommonBaseToRight = commonBaseToRight;

        var sections = Diff.CalculateSections(commonBaseToLeft, commonBaseToRight, diffOptions, new DiffSectionMergeComparer<T>(comparer)).ToList();
        this.mergeSections = sections;
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
        return (leftOp, rightOp) switch
        {
            (DiffOperation.Match, DiffOperation.Match) => [leftSide],
            (DiffOperation.Match, DiffOperation.Insert) => ThrowMergeConflictException(),
            (DiffOperation.Match, DiffOperation.Delete) => [],
            (DiffOperation.Match, DiffOperation.Replace or DiffOperation.Modify) => [rightSide],
            (DiffOperation.Match, _) => ThrowArgumentOutOfRangeException(rightOp),
            (DiffOperation.Insert, _) => ThrowMergeConflictException(),
            (DiffOperation.Delete, DiffOperation.Match) => [],
            (DiffOperation.Delete, DiffOperation.Insert) => ThrowMergeConflictException(),
            (DiffOperation.Delete, DiffOperation.Delete) => [],
            (DiffOperation.Delete, DiffOperation.Replace or DiffOperation.Modify) => this.conflictResolver.Resolve([commonBase], [], [rightSide]),
            (DiffOperation.Delete, _) => ThrowArgumentOutOfRangeException(rightOp),
            (DiffOperation.Replace or DiffOperation.Modify, DiffOperation.Match) => [leftSide],
            (DiffOperation.Replace or DiffOperation.Modify, DiffOperation.Insert) => ThrowMergeConflictException(),
            (DiffOperation.Replace or DiffOperation.Modify, DiffOperation.Delete) => this.conflictResolver.Resolve([commonBase], [leftSide], []),
            (DiffOperation.Replace or DiffOperation.Modify, DiffOperation.Replace or DiffOperation.Modify) => this.conflictResolver.Resolve([commonBase], [leftSide], [rightSide]),
            (DiffOperation.Replace or DiffOperation.Modify, _) => ThrowArgumentOutOfRangeException(rightOp),
            _ => ThrowMergeConflictException(),
        };

        IEnumerable<T?> ThrowMergeConflictException()
        {
            throw new MergeConflictException($"Unable to process {leftOp} vs. {rightOp}", [commonBase], [leftSide], [rightSide]);
        }

        static IEnumerable<T?> ThrowArgumentOutOfRangeException(DiffOperation rightOp)
        {
            throw new ArgumentOutOfRangeException(nameof(rightOp), rightOp, message: null);
        }
    }
}