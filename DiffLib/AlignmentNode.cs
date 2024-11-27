// <copyright file="AlignmentNode.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal class AlignmentNode(DiffOperation operation, double similarity, int nodeCount, AlignmentNode? next)
{
    public DiffOperation Operation { get; } = operation;

    public double Similarity { get; } = similarity;

    public double AverageSimilarity => this.NodeCount == 0 ? 0.0 : this.Similarity / this.NodeCount;

    public int NodeCount { get; } = nodeCount;

    public AlignmentNode? Next { get; } = next;
}